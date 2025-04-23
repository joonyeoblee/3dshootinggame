using Redcode.Pools;
using UnityEngine;
public class Bomb : MonoBehaviour, IPoolObject
{
    public GameObject ExplosionEffectPrefab;

    public PoolItem PoolItem;
    private void Start()
    {
        PoolItem = GetComponent<PoolItem>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 이펙트 생성
        GameObject effectObject = Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.identity);

        // 풀로 반환
        PoolItem.ReturnToPoolAs<Bomb>();

    }

    public void OnCreatedInPool()
    {
        // 최초 생성 시 호출 (선택적 초기화)
    }

    public void OnGettingFromPool()
    {
        // 풀에서 꺼내올 때 호출 (폭탄 리셋 작업 가능)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


}
