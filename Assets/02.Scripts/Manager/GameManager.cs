using Redcode.Pools;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PoolManager PoolManager;

    private void Awake()
    {
        Instance = this;
    }
}
