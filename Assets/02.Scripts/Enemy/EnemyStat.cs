using System;
using UnityEngine;
[Serializable]
public class EnemyStat
{
    public EnemyType EnemyType;
    public float MoveSpeed = 3.3f;
    public float AttackCoolTime = 1f;
    public float MaxHealth = 100f;

    [Header("# About States")]
    public float FindDistance = 7f;
    public float AttackDistance = 2.5f;
    public float ReturnDistance = 0.1f;
    public float DamagedTime = 0.5f; // 경직 시간
    public float IdleToPatrolTime = 3f;
}
