using System;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // 카메라 회전 스크립트
    // 목표ㅣ 마우스를 조작하면 카메라를 그 방향으로 회전시키고 싶다.
    // 구현순서
    public float RotationSpeed = 200f;

    public float minX = -60f; // 아래로 회전 제한
    public float maxX = 60f;  // 위로 회전 제한

    float _xRotation = 0f; // 상하 회전값 누적 저장
    float _yRotation = 0f;

    void Update()
    {
        // 1. 마우스 입력을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        // Debug.Log($"Mouse X: {mouseX} Mouse Y: {mouseY}");

        // 2. 회전한 양만큼 누적시켜 나간다.
        _xRotation += mouseX * RotationSpeed * Time.deltaTime;
        _yRotation += -mouseY * RotationSpeed * Time.deltaTime;
        _yRotation = Mathf.Clamp(_yRotation, minX, maxX);

        // 3. 회전 방향으로 회전시킨다
        transform.eulerAngles = new Vector3(_yRotation, _xRotation, 0f);
        // transform.localRotation = Quaternion.Euler(-_yRotation, _xRotation, 0f);
    }
}
