using UnityEngine;
[CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObjects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public int MaxHealth = 100;
    [Header("이동")]
    public float RunSpeed = 12f;
    public float WalkSpeed = 7f;
    public float JumpPower = 5f;

    [Header("스태미너")]
    public float StaminaCost = 1f;
    public float StaminaRecovery = 2f;
    public float RollStaminaCost = 3f;

    [Header("구르기")]
    public float PushPower = 10f;
    public float PushDuration = 0.2f;

}
