using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(CharacterAnimator))]
[RequireComponent(typeof(Seeker))]
public class FleeingCharacter : MonoBehaviour, ICollectable {
    [HideInInspector] public bool canMove = true;
    [SerializeField] int scoreValue = default;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float runRange = 5f;
    [SerializeField] float nextWaypointDistance = 3;
    [SerializeField] float repathRate = 0.25f;

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
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (canMove && distToPlayer < runRange) {
            animator.MovingAnimation(true);
            Pathfind();
        } else {
            animator.MovingAnimation(false);
        }
    }

    private void Pathfind() {
        if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            lastRepath = Time.time;
            Vector3 dir = player.position - transform.position;
            seeker.StartPath(transform.position, transform.position + (dir.normalized * -runRange), OnPathComplete);
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

    public void ToggleMovement(bool val) {
        collider.enabled = val;
        animator.MovingAnimation(val);
        canMove = val;
    }
}
