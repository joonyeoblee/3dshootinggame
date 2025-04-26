using System;
using UnityEngine;
using Random = UnityEngine.Random;
public class EnemySpawner : MonoBehaviour
{
    public float SpawnTimes = 1f;
    private float _currentTime;

    private void Update()
    {
        if(!GameManager.Instance.IsPlaying) return;

        _currentTime += Time.deltaTime;
        if (_currentTime >= SpawnTimes)
        {
            _currentTime = 0f;

            Enemy enemy = GameManager.Instance.PoolManager.GetFromPool<Enemy>();
            Vector3 randomVector = new Vector3(Random.Range(-1.0f, 1.0f), 0f, Random.Range(-1.0f, 1.0f));
            enemy.transform.position = transform.position + randomVector;
        }
    }

}
