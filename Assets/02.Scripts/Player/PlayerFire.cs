using UnityEngine;
public class PlayerFire : MonoBehaviour
{
    public GameObject FirePosition;
    public GameObject BombPrefab;

    private Camera _mainCamera;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        // 0 왼쪽, 1: 오른쪽 2: 휠
        if (Input.GetMouseButtonDown(1))
        {
            GameObject bomb = Instantiate(BombPrefab);
            bomb.transform.position = FirePosition.transform.position;

            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(_mainCamera.transform.forward * 15f);
            bombRigidbody.AddTorque(Vector3.one);
        }
    }
}
