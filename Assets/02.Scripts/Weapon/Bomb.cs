using UnityEngine;
public class Bomb : MonoBehaviour
{
    public Explore Explore;
    public PoolItem PoolItem;
    private void Start()
    {
        Explore = GetComponent<Explore>();
        PoolItem = GetComponent<PoolItem>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Explore.Explode();
        // 풀로 반환
        PoolItem.ReturnToPoolAs<Bomb>();

    }

    public void OnGettingFromPool()
    {
        // 풀에서 꺼내올 때 호출 (폭탄 리셋 작업 가능)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


}
