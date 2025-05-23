using UnityEngine;
public class CameraRotate : MonoBehaviour
{
    public GameObject Pivot;

    public float RotationSpeed = 200f;
    public float minY = -60f;
    public float maxY = 60f;

    private float _yaw; // 좌우 회전
    private float _pitch; // 상하 회전

    private float _recoilOffset;
    public float recoilRecoverySpeed = 20f;

    public GameObject TopView;

    public CameraFollow Follow;

    void Update()
    {
        if(!GameManager.Instance.IsPlaying) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _yaw += mouseX * RotationSpeed * Time.deltaTime;
        _pitch -= mouseY * RotationSpeed * Time.deltaTime;

        _pitch -= _recoilOffset;
        _recoilOffset = Mathf.MoveTowards(_recoilOffset, 0f, recoilRecoverySpeed * Time.deltaTime);

        _pitch = Mathf.Clamp(_pitch, minY, maxY);

        if (Follow.TOPMode)
        {
            // TopView.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
        else if (Follow.FPSMode)
        {
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
        else
        {
            Pivot.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

    }

    public void AddRecoil(float amount)
    {
        _recoilOffset += amount;
    }
}
