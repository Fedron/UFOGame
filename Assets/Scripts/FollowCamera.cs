using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset = default;

    private new Camera camera;
    private float defaultZoom;
    private Transform player;

    private void Awake() {
        camera = GetComponent<Camera>();
        defaultZoom = camera.orthographicSize;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {
        if (!player) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
