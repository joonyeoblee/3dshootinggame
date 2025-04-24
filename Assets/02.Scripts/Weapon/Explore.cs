using UnityEngine;
public class Explore : MonoBehaviour
{
    public GameObject ExploreVFX;

    public Damage Damage;

    private void Start()
    {
        Damage.DamageFrom = gameObject;
    }
    public void Explode()
    {
        Instantiate(ExploreVFX, transform.position, Quaternion.identity);

        int layerMask = LayerMask.GetMask("Enemy", "Player");


        Collider[] hits = Physics.OverlapSphere(transform.position, 10f, layerMask);

        foreach(Collider hit in hits)
        {
            IDamageable hitobject = hit.GetComponent<IDamageable>();
            hitobject.TakeDamage(Damage);
        }
    }
}
