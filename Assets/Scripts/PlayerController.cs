using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float moveSpeed = 5f;

    [Header("Dashing")]
    [SerializeField] float dashAmount = 10f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] ParticleSystem dashEffect = default;
    [Space, SerializeField] Transform ufo = default;

    [Header("UI")]
    [SerializeField] GameObject firstLife = default;
    [SerializeField] GameObject secondLife = default;

    new private Rigidbody2D rigidbody;
    private float horizontalInput, verticalInput;
    private bool canDash = true;
    private bool shouldDash;
    private int lives = 2;
    new private FollowCamera camera;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = FindObjectOfType<FollowCamera>();
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        camera.movementOffset = new Vector3(horizontalInput, verticalInput, 0f);

        // Rotate UFO to match X Velocity
        ufo.rotation = Quaternion.Euler(0f, 0f, horizontalInput * -10f);
        if (canDash &&Input.GetMouseButtonDown(1)) {
            canDash = false;
            shouldDash = true;
            dashEffect.Play();
            Invoke("CanDash", dashCooldown);
        }
    }

    private void FixedUpdate() {
        Vector3 velocity = new Vector3(horizontalInput * moveSpeed, verticalInput * moveSpeed, 0f);
        transform.position += velocity * Time.fixedDeltaTime;

        if (shouldDash) {
            rigidbody.AddForce(new Vector2(horizontalInput, verticalInput) * dashAmount, ForceMode2D.Impulse);
            shouldDash = false;
        }
    }

    private void CanDash() => canDash = true;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Bullet")) {
            lives--;

            if (lives == 1) firstLife.SetActive(false);
            if (lives <= 0) {
                secondLife.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
