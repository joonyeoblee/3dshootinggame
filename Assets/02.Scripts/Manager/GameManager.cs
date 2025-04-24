using Redcode.Pools;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public PoolManager PoolManager;

    protected override void Start()
    {
        base.Start();
        Cursor.lockState = CursorLockMode.Locked;
    }
}
