using UnityEngine;
public class Player : MonoBehaviour
{
    public PlayerSO PlayerData;

    public GameObject Model;

    public Animator Animator;

    private void Start()
    {
        Animator = Model.GetComponent<Animator>();
    }

}
