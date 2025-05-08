using System;
using System.Collections.Generic;
using UnityEngine;
public enum EPopupType
{
    UI_OptionPopup,
    UI_CreditPopup
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;


    [Header("팝업 UI 참조")]
    public Stack<UI_Popup> Popups; // 모든 팝업을 관리하는데

    private readonly Stack<UI_Popup> _openedPopups = new Stack<UI_Popup>(); // null은 아니지만 비어있는 리스트
    // 1. 
    private void Awake()
    {
        Instance = this;
    }

    public void Open(EPopupType type, Action closeCallback = null)
    {
        Open(type.ToString(), closeCallback);
    }

    private void Open(string popupName, Action closeCallback)
    {
        foreach(UI_Popup popup in Popups)
        {
            if (popup.gameObject.name == popupName)
            {
                popup.Open(closeCallback);
                // 팝업을 열 때마다 담는다.
                _openedPopups.Push(popup);
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_openedPopups.Count > 0)
            {
                while (true)
                {
                    UI_Popup popup = _openedPopups.Pop();

                    bool opend = popup.isActiveAndEnabled;

                    popup.Close();

                    if (opend || _openedPopups.Peek() == null)
                    {
                        break;
                    }
                }
            }
            else
            {
                GameManager.Instance.Pause();
            }
        }
    }

    public void OpenCreditPopup()
    {
        Open(EPopupType.UI_CreditPopup);
    }
}
