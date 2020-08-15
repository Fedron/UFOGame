using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour {
    [SerializeField, Min(0.1f)] float animationMoveSpeed = 1f;

    Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetFloat("animationSpeed", animationMoveSpeed);
    }

    public void MovingAnimation(bool playing) {
        animator.SetBool("moving", playing);
    }
}
