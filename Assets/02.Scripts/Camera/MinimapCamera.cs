using UnityEngine;
public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float YOffset = 10f;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (_camera.orthographicSize < 11f)
            {
                _camera.orthographicSize += 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (_camera.orthographicSize > 1f)
            {
                _camera.orthographicSize -= 1f;
            }
        }
    }
    private void LateUpdate()
    {
        Vector3 newPosition = Target.position;
        newPosition.y += YOffset;

        transform.position = newPosition;

        Vector3 newEulerAngles = Target.eulerAngles;
        newEulerAngles.x = 90;
        newEulerAngles.z = 0;
        transform.eulerAngles = newEulerAngles;

    }
}
