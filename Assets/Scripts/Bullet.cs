using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] GameObject impact = default;

    private void OnDestroy() {
        Destroy(
            Instantiate(impact, transform.position, transform.rotation)
        , 1f);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Destroy(gameObject);
    }
}
