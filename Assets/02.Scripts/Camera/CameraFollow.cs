using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform FPSCamera;
    public bool FPSMode;
    public Transform TPSCamera;
    public bool TPSMode;
    public Transform TOPCamera;
    public bool TOPMode;

    private Transform _povotTransform;
    private void Start()
    {
        FPSMode = true;
        _povotTransform = GetComponent<CameraRotate>().Pivot.transform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TPSMode = true;
            FPSMode = false;
            TOPMode = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FPSMode = true;
            TPSMode = false;
            TOPMode = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            TOPMode = true;
            FPSMode = false;
            TPSMode = false;
        }

        if (TPSMode)
        {
            transform.position = TPSCamera.position;
            transform.rotation = _povotTransform.rotation;

        }
        else if (FPSMode)
        {
            // interpoling, smoothing 기법
            transform.position = FPSCamera.position;
            transform.rotation = _povotTransform.rotation;
        }
        else if (TOPMode)
        {
            transform.position = TOPCamera.position;
            transform.rotation = TOPCamera.rotation;
        }
    }
}
