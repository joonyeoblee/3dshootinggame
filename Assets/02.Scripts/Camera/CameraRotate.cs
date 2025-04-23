using UnityEngine;
public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 200f;
    public float minY = -60f;
    public float maxY = 60f;

    private float _yaw; // 좌우 회전
    private float _pitch; // 상하 회전

    private float _recoilOffset;
    public float recoilRecoverySpeed = 20f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _yaw += mouseX * RotationSpeed * Time.deltaTime;
        _pitch -= mouseY * RotationSpeed * Time.deltaTime;

        _pitch -= _recoilOffset;
        _recoilOffset = Mathf.MoveTowards(_recoilOffset, 0f, recoilRecoverySpeed * Time.deltaTime);

        _pitch = Mathf.Clamp(_pitch, minY, maxY);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void AddRecoil(float amount)
    {
        _recoilOffset += amount;
    }
}
