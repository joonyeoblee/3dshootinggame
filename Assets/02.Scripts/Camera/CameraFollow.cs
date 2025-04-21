using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;

    void Update()
    {
        // interpoling, smoothing 기법
        transform.position = Target.position;
    }
}
