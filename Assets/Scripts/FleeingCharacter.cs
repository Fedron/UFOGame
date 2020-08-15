using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
public class FleeingCharacter : MonoBehaviour {
    [HideInInspector] public bool canMove = true;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float runRange = 5f;

    new Rigidbody2D rigidbody;
    CharacterAnimator animator;
    Transform player;
    bool playerInRange;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<CharacterAnimator>();     
    }

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, player.position) < runRange) {
            playerInRange = true;
            animator.MovingAnimation(true);
        } else {
            playerInRange = false;
            animator.MovingAnimation(false);
        }
    }

    private void FixedUpdate() {
        if (canMove && playerInRange) {
            rigidbody.velocity = (transform.position - player.position).normalized * moveSpeed;
        } else {
            rigidbody.velocity = Vector2.zero;
        }
    }
}
