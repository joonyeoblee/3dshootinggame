using Redcode.Pools;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum EGameState
{
    Ready,
    Run,
    Pause,
    Over
}


public class GameManager : Singleton<GameManager>
{
    public PoolManager PoolManager;

    public bool IsPlaying;
    public GameObject Player;

    private EGameState _gameState;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        _gameState = EGameState.Pause;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;

        PopupManager.Instance.Open(EPopupType.UI_OptionPopup, Continue);
    }

    public void Continue()
    {
        _gameState = EGameState.Run;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {

        _gameState = EGameState.Run;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);


        // 다시시작을 했더니 게임이 망가지는 경우가 있다...
        // 싱글톤 처리를 잘못했을경우 망가진다.
    }

}
