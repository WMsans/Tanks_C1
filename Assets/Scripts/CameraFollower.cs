using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform following;
    [SerializeField] private float smoothSpeed = 0.125f; 
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        if (offset.sqrMagnitude < 0.1f)
        {
            offset = transform.position - following.position;
        }
    }

    private void LateUpdate()
    {
        if (following == null)
        {
            return;
        }

        var desiredPosition = following.position + offset;
        var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
