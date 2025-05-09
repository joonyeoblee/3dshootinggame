using System;
using System.Security.Cryptography;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class UI_InputField
{
    public TextMeshProUGUI ResultText;
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordConfirmInputField;
    public Button ConfirmButton;
}

public class UI_LoginScene : MonoBehaviour
{
    [Header("패널")]
    public GameObject LoginPanel;
    public GameObject ResisterPanel;
    [Header("로그인")]
    public UI_InputField LoginInputField;
    [Header("회원가입")]
    public UI_InputField SignUpInputField;

    private const string SALT = "903872727";
    private const string PREFIX = "aasas12132";

    private void Start()
    {
        LoginPanel.SetActive(true);
        ResisterPanel.SetActive(false);

        LoginCheck();
    }

    public void OnClickGoToResister()
    {
        LoginPanel.SetActive(false);
        ResisterPanel.SetActive(true);
    }

    public void OnClickGoToLogin()
    {
        LoginPanel.SetActive(true);
        ResisterPanel.SetActive(false);
    }


    // 회원가입
    public void Resister()
    {
        string id = SignUpInputField.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            SignUpInputField.ResultText.text = "아이디를 입력해주세요.";
            SignUpInputField.IDInputField.gameObject.transform.DOShakePosition(
                1f, // 지속 시간
                3f, // 흔들림 강도 (Vector3도 가능)
                40 // 점점 줄어들며 종료
            );
            return;
        }

        string pw = SignUpInputField.PasswordInputField.text;
        string pwConfirm = SignUpInputField.PasswordConfirmInputField.text;

        if (string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(pwConfirm))
        {
            GameObject nullobj = string.IsNullOrEmpty(pw) ? SignUpInputField.PasswordInputField.gameObject : SignUpInputField.PasswordInputField.gameObject;
            SignUpInputField.ResultText.text = "비밀번호와 확인을 입력해주세요.";
            nullobj.transform.DOShakePosition(
                1f, // 지속 시간
                3f, // 흔들림 강도 (Vector3도 가능)
                40 // 점점 줄어들며 종료
            );
            return;
        }

        if (pw != pwConfirm)
        {
            SignUpInputField.ResultText.text = "비밀번호 확인이 다릅니다";
            SignUpInputField.PasswordConfirmInputField.gameObject.transform.DOShakePosition(
                1f, // 지속 시간
                3f, // 흔들림 강도 (Vector3도 가능)
                40 // 점점 줄어들며 종료
            );
            return;
        }

        PlayerPrefs.SetString(PREFIX + id, EncryptAES(pw + SALT));
        PlayerPrefs.Save();

        OnClickGoToLogin();
    }

    private string EncryptAES(string plainText)
    {
        // 해시 암호화 알고리즘 인스턴스를 생성한다.
        SHA256 sha256 = SHA256.Create();

        // 운영체제 혹은 프로그래밍 언어별로 string 표현하는 방식이 다 다르므로
        // UTF8 버전 바이트로 배열을 바꿔야한다.
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(encrypted);
    }

    public void Login()
    {
        // 1. 아이디 입력을 확인한다.
        string id = LoginInputField.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            SignUpInputField.ResultText.text = "아이디를 입력해주세요.";
            SignUpInputField.IDInputField.gameObject.transform.DOShakePosition(
                1f, // 지속 시간
                3f, // 흔들림 강도 (Vector3도 가능)
                40 // 점점 줄어들며 종료
            );
            return;
        }
        // 2. 비밀번호 입력을 확인한다.
        string pw = LoginInputField.PasswordInputField.text;

        if (string.IsNullOrEmpty(pw))
        {
            SignUpInputField.ResultText.text = "비밀번호와 확인을 입력해주세요.";
            LoginInputField.PasswordInputField.transform.DOShakePosition(
                1f, // 지속 시간
                3f, // 흔들림 강도 (Vector3도 가능)
                40 // 점점 줄어들며 종료
            );
            return;
        }
        // 3. PlayerPrefs.Get을 사용해 확인
        if (!PlayerPrefs.HasKey(PREFIX + id))
        {
            LoginInputField.ResultText.text = "아이디와 비밀번호를 확인해주세요.";
            return;
        }
        string hashedPassword = PlayerPrefs.GetString(PREFIX + id);
        if (hashedPassword != EncryptAES(pw + SALT))
        {
            LoginInputField.ResultText.text = "아이디와 비밀번호를 확인해주세요.";
            return;
        }

        // 4. 맞다면 로그인
        Debug.Log("로그인 성공!");
    }

    public void LoginCheck()
    {
        string id = LoginInputField.IDInputField.text;
        string password = LoginInputField.PasswordInputField.text;

        LoginInputField.ConfirmButton.enabled = !(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password));
    }
}
