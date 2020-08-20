using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : Bullet {
    [SerializeField] float speed = default;
    [SerializeField] float rotateSpeed = default;

    new private Rigidbody2D rigidbody;
    private Transform player;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    private void FixedUpdate() {
        if (player == null) return;

        Vector2 direction = (Vector2)player.position - rigidbody.position;
		direction.Normalize();
		float rotateAmount = Vector3.Cross(direction, transform.up).z;
		rigidbody.angularVelocity = -rotateAmount * rotateSpeed;
		rigidbody.velocity = transform.up * speed;
    }
}
