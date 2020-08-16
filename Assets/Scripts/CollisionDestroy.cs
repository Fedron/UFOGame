using UnityEngine;

public class CollisionDestroy : MonoBehaviour {
    private void Start() {
        Invoke("DestroySelf", 0.1f);
    }

    private void DestroySelf() {
        Destroy(GetComponent<CompositeCollider2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(this);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }
}
