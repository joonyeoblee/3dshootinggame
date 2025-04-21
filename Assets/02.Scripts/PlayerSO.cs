using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObjects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [Header("이동")]
    public float RunSpeed = 12f;
    public float WalkSpeed = 7f;
    public float JumpPower = 5f;

    [Header("스태미너")]
    public float StaminaCost = 0.05f;
    public float StaminaRecovery = 0.4f;
    public float RollStaminaCost = 0.3f;

    [Header("구르기")]
    public float PushPower = 10f;
    public float PushDuration = 0.2f;

}
