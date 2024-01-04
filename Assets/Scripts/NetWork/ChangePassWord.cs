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
            Debug.Log("Password" + passwordInputField.text + "�ɐݒ肵�܂���");
            ingameLog.GenerateIngameLog("Password���u" + passwordInputField.text + "�v�ɐݒ肵�܂���");
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("leaderId"))
        {
            Debug.Log("leaderId�̃J�X�^���v���p�e�B�����݂��܂�");
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
