using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] GameObject impact = default;
    [SerializeField] AudioClip destroySound = default;
    [SerializeField] CameraShakeProfile shakeProfile = default;

    private void OnDestroy() {
        Destroy(
            Instantiate(impact, transform.position, transform.rotation)
        , 1f);
        CameraShake.ShakeOnce(shakeProfile);
        if (destroySound) SoundManager.Instance.Play(destroySound);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Destroy(gameObject);
    }
}
