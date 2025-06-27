using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BillBoard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
