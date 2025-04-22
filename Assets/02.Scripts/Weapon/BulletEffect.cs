using UnityEngine;
public class BulletEffect : MonoBehaviour
{
    public PoolItem PoolItem;

    private void Start()
    {
        PoolItem = GetComponent<PoolItem>();
    }


    private void OnDisable()
    {
        PoolItem.ReturnToPoolAs<BulletEffect>();
    }
}
