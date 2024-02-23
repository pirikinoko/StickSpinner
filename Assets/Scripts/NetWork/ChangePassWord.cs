using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class ChangePassWord : MonoBehaviourPunCallbacks
{
     InputField passwordInputField;
    [SerializeField]
    Button updatePasswordButton, switchButton;
    private IngameLog ingameLog = new IngameLog();
    void Start()
    {
        updatePasswordButton.onClick.AddListener(UpdatePassword);
        switchButton.onClick.AddListener(ToggleInputMode);
        passwordInputField = this.GetComponent<InputField>();
        passwordInputField.contentType = InputField.ContentType.Password;

        passwordInputField.onValueChanged.AddListener(OnPasswordValueChanged);
    }

    void OnPasswordValueChanged(string newValue)
    {
        Debug.Log("Password changed: " + newValue);
    }

    void UpdatePassword() 
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("Password"))
        {
            customProps["Password"] = passwordInputField.text;
            Debug.Log("Password" + passwordInputField.text + "に設定しました");
            ingameLog.GenerateIngameLog("Passwordを「" + passwordInputField.text + "」に設定しました");
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("leaderId"))
        {
            Debug.Log("leaderIdのカスタムプロパティが存在します");
        }
    }

    void SetPasswordInputMode()
    {
        passwordInputField.contentType = InputField.ContentType.Password;
        passwordInputField.ForceLabelUpdate();
    }

    void SetStandardInputMode()
    {
        passwordInputField.contentType = InputField.ContentType.Standard;
        passwordInputField.ForceLabelUpdate();
    }

    public void ToggleInputMode()
    {
        if (passwordInputField.contentType == InputField.ContentType.Password)
        {
            SetStandardInputMode();
        }
        else
        {
            SetPasswordInputMode();
        }
    }
}
