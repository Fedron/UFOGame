using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour {
    [SerializeField] Animator laser = default;
    [SerializeField] ParticleSystem laserParticles = default;
    [SerializeField] float laserSuckSpeed = 2f;
    [SerializeField] GameObject characterDestroyEffect = default;
    [SerializeField] CameraShakeProfile shakeProfile = default;
    [SerializeField] AudioSource laserSound = default;
    [SerializeField] AudioClip pickupSound = default;

    private bool laserActive = false;
    private List<GameObject> charactersInLaser = new List<GameObject>();

    private void Update() {
        if (!laserActive && Input.GetMouseButtonDown(0)) {
            laserActive = true;
            laser.SetTrigger("TurnOn");
            laser.ResetTrigger("TurnOff");
            laserParticles.Play();
            laserSound.Play();
        } else if (laserActive && Input.GetMouseButtonUp(0)) {
            laserActive = false;
            laser.SetTrigger("TurnOff");
            laser.ResetTrigger("TurnOn");
            laserParticles.Stop();
            laserSound.Stop();
            foreach (GameObject c in charactersInLaser) {
                if (!c) continue;
                c.transform.SetParent(null);
                c.GetComponent<ICollectable>().ToggleMovement(true);
            }
            charactersInLaser.Clear();
        }

        // Move characters up the laser
        if (laserActive && charactersInLaser.Count > 0) {
            List<int> toRemove = new List<int>();
            for (int i = 0; i < charactersInLaser.Count; i++) {
                if (!charactersInLaser[i]) continue;
                charactersInLaser[i].transform.localPosition =
                    Vector3.MoveTowards(charactersInLaser[i].transform.localPosition, new Vector3(
                        0f,
                        2.25f,
                        0f
                    ), laserSuckSpeed * Time.deltaTime);

                // The character has reached the top of the laser
                if (charactersInLaser[i].transform.localPosition.y == 2.25f) {
                    Destroy(
                        Instantiate(characterDestroyEffect, charactersInLaser[i].transform.position, Quaternion.identity, transform.parent)
                    , 1f);
                    CameraShake.ShakeOnce(shakeProfile);
                    SoundManager.Instance.Play(pickupSound);
                    charactersInLaser[i].GetComponent<ICollectable>().Destroy();
                    charactersInLaser[i] = null;
                    toRemove.Add(i);
                }
            }
            foreach (int index in toRemove) {
                charactersInLaser.RemoveAt(index);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (laserActive && other.TryGetComponent<ICollectable>(out ICollectable c)) {
            other.transform.SetParent(transform);
            c.ToggleMovement(false);
            charactersInLaser.Add(other.gameObject);
        }
    }
}
