using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [HideInInspector] public Vector3 movementOffset = default;

    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 defaultOffset = default;
    [SerializeField] float movementOffsetMult = 3f;

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

        Vector3 desiredPosition = player.position + defaultOffset + (movementOffset * movementOffsetMult);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
