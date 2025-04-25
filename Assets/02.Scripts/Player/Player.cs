using UnityEngine;
public class Player : MonoBehaviour, IDamageable
{
    public PlayerSO PlayerData;

    public GameObject Model;

    public Animator Animator;

    public int Health { get; set; }

    private void Start()
    {
        Health = 100;
        Animator = Model.GetComponent<Animator>();
    }

    public void TakeDamage(Damage damage)
    {
        Health -= damage.Value;
        UI_Main.Instance.RefreshHealthSlider(Health);
        Debug.Log($"{name} damage dealt from {damage.Value} to {Health}");
    }
}
