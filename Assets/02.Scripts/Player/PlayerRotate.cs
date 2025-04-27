using UnityEngine;

public class PlayerRotate : PlayerBase
{
    public float RotationSpeed = 200f;

    float _xRotation = 0f; // 상하 회전값 누적 저장
    float _yRotation = 0f;

    void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        // 1. 마우스 입력을 받는다.
        float mouseX = Input.GetAxis("Mouse X");
        // float mouseY = Input.GetAxis("Mouse Y");

        // 2. 회전한 양만큼 누적시켜 나간다.
        _xRotation += mouseX * RotationSpeed * Time.deltaTime;
        // _yRotation += -mouseY * RotationSpeed * Time.deltaTime;
        // _yRotation = Mathf.Clamp(_yRotation, minX, maxX);

        // 3. 회전 방향으로 회전시킨다
        transform.eulerAngles = new Vector3(0f, _xRotation, 0f);
        // transform.localRotation = Quaternion.Euler(-_yRotation, _xRotation, 0f);
    }
}
