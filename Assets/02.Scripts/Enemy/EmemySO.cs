using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public List<EnemyStat> EnemyStats;

    private Dictionary<EnemyType, EnemyStat> _enemyDict;

    public void Init()
    {
        _enemyDict = new Dictionary<EnemyType, EnemyStat>();
        foreach(EnemyStat data in EnemyStats)
        {
            if (!_enemyDict.ContainsKey(data.EnemyType))
                _enemyDict.Add(data.EnemyType, data);
        }
    }

    public EnemyStat GetData(EnemyType type)
    {
        if (_enemyDict == null)
            Init();

        if (_enemyDict.TryGetValue(type, out EnemyStat data))
            return data;

        Debug.LogWarning($"Weapon ID '{type}' not found!");
        return null;
    }
}
