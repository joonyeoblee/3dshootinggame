using UnityEngine;
public class Bullet : MonoBehaviour
{
    public PoolItem PoolItem;

    private void Start()
    {
        PoolItem = GetComponent<PoolItem>();
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log(other.gameObject.name);
    //     PoolItem.ReturnToPoolAs<Bullet>();
    // }
}
