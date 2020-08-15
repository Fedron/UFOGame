using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] SpriteRenderer laser = default;
    [SerializeField] float laserSuckSpeed = 2f;

    new Rigidbody2D rigidbody;
    float horizontalInput, verticalInput;
    bool laserActive = false;
    List<GameObject> charactersInLaser = new List<GameObject>();

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (!laserActive && Input.GetMouseButtonDown(0)) {
            laserActive = true;
            laser.size = new Vector2(laser.size.x, 2f);
        } else if (laserActive && Input.GetMouseButtonUp(0)) {
            laserActive = false;
            laser.size = new Vector2(laser.size.x, 0f);
            foreach (GameObject c in charactersInLaser) {
                c.transform.SetParent(null);
            }
            charactersInLaser.Clear();
        }

        // Move characters up the laser
        if (laserActive && charactersInLaser.Count > 0) {
            List<int> toRemove = new List<int>();
            for (int i = 0; i < charactersInLaser.Count; i++) {
                charactersInLaser[i].transform.localPosition =
                    Vector3.MoveTowards(charactersInLaser[i].transform.localPosition, new Vector3(
                        charactersInLaser[i].transform.localPosition.x,
                        0f,
                        0f
                    ), laserSuckSpeed * Time.deltaTime);

                if (charactersInLaser[i].transform.localPosition.y == 0f) {
                    Destroy(charactersInLaser[i]);
                    charactersInLaser[i] = null;
                    toRemove.Add(i);
                }
            }
            foreach (int index in toRemove) {
                charactersInLaser.RemoveAt(index);
            }
        }
    }

    private void FixedUpdate() {
        rigidbody.velocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (laserActive && other.CompareTag("Collectable")) {
            other.transform.SetParent(transform);
            charactersInLaser.Add(other.gameObject);
        }
    }
}
