using UnityEditor;
public class UI_OptionPopup : UI_Popup
{
    public void OnClickContinueButton()
    {
        GameManager.Instance.Pause();

        gameObject.SetActive(false);
    }

    public void OnClickRetryButton()
    {
        GameManager.Instance.Restart();
    }

    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickCreditButton()
    {
        PopupManager.Instance.Open(EPopupType.UI_CreditPopup);
    }
}
