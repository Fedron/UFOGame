using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(CharacterAnimator))]
[RequireComponent(typeof(Seeker))]
public class AggressiveCharacter : MonoBehaviour, ICollectable {
    private enum AIState {
        Fleeing,
        Chasing,
        Shooting,
    }

    [HideInInspector] public bool canMove = true;
    [SerializeField] int scoreValue = default;
    [Header("AI")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float runRange = 5f;
    [SerializeField] float shootRange = 8f;
    [SerializeField] float nextWaypointDistance = 3;
    [SerializeField] float repathRate = 0.25f;

    [Header("Shooting")]
    [SerializeField] Transform weapon = default;
    [SerializeField] SpriteRenderer weaponSprite = default;
    [SerializeField] Transform[] firepoints = default;
    [SerializeField, Min(0f)] float fireRate = default;
    [SerializeField] GameObject bullet = default;

    new private Rigidbody2D rigidbody;
    private CharacterAnimator animator;
    new private Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private Seeker seeker;
    private Transform player;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private float lastRepath = float.NegativeInfinity;
    private AIState state = AIState.Chasing;
    private bool canShoot = true;
    public int ScoreValue { get; set; }

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<CharacterAnimator>();
        collider = GetComponent<Collider2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        
        ScoreValue = scoreValue;
    }

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (!canMove || player == null) {
            animator.MovingAnimation(false);
            return;
        }
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        animator.MovingAnimation(true);

        if (distToPlayer > shootRange) {
            state = AIState.Chasing;
            Pathfind();
        } else if (distToPlayer < runRange) {
            state = AIState.Fleeing;
            Pathfind();
        } else if (distToPlayer > runRange && distToPlayer < shootRange) {
            state = AIState.Shooting;
            animator.MovingAnimation(false);
            AttackPlayer();
        }
    }

    private void Pathfind() {
        if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            lastRepath = Time.time;
            Vector3 dir = player.position - transform.position;
            Vector3 targetPos = player.position;

            if (state == AIState.Fleeing) targetPos = transform.position + (dir.normalized * -runRange);

            seeker.StartPath(transform.position, targetPos, OnPathComplete);
        }

        if (path == null) return;

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }

        float speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
        Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = direction * moveSpeed * speedFactor;

        transform.position += velocity * Time.deltaTime;
        if (velocity.x < 0) {
            spriteRenderer.flipX = true;
        } else {
            spriteRenderer.flipX = false;
        }
    }

    private void OnPathComplete(Path p) {
        p.Claim(this);
        if (!p.error) {
            if (path != null) path.Release(this);
            path = p;
            currentWaypoint = 0;
        } else {
            p.Release(this);
        }
    }

    private void AttackPlayer() {
        Vector3 vectorToTarget = player.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        weapon.rotation = Quaternion.Slerp(weapon.rotation, q, Time.deltaTime * 50f);

        if (vectorToTarget.x < 0) weaponSprite.flipY = true;
        else weaponSprite.flipY = false;

        if (canShoot) {
            canShoot = false;
            Invoke("CanShoot", fireRate);
            Shoot();
        }
    }

    private void Shoot() {
        foreach (Transform firepoint in firepoints) {
            GameObject b = Instantiate(bullet, firepoint.position, firepoint.parent.rotation);
            b.GetComponent<Rigidbody2D>().AddForce(firepoint.right * 800f);
            Destroy(b, 1.5f);
        }
    }
    
    private void CanShoot() => canShoot = true;

    public void ToggleMovement(bool val) {
        collider.enabled = val;
        animator.MovingAnimation(val);
        canMove = val;
    }

    public void Destroy() {
        ScoreManager.Instance.AddScore(ScoreValue);
        FindObjectOfType<SpawnerManager>().spawnedEnemies--;
        Destroy(gameObject);
    }
}
