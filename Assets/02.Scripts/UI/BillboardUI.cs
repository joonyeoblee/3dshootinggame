using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (_mainCamera == null) return;

        // 카메라를 향하도록 회전
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }
}
