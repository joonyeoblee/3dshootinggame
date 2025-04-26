using System;
using Redcode.Pools;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    public PoolManager PoolManager;

    public bool IsPlaying { get; private set; } = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPlaying = !IsPlaying;
        }
    }
    public void StartGame()
    {
        IsPlaying = true;
    }

    public void PauseGame()
    {
        IsPlaying = false;
    }

    public void EndGame()
    {
        UI_Main.Instance.EndGame();
        IsPlaying = false;
        Time.timeScale = 0;
    }
}
