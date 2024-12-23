using Fusion;
using UnityEngine;
using UnityEngine.UI;
public class RoomNameVisible : NetworkBehaviour
{
    Text roomNameText;
    string roomName;
    string hiddenText;
    [SerializeField]
    Button toggleVisibleButton;
    string lockSymbol = "!Locked!";
    // Start is called before the first frame update
    void Start()
    {
        roomName = PhotonNetwork.CurrentRoom.Name;
        roomNameText = this.GetComponent<Text>();
        if (roomNameText.text.Contains("!Locked!"))
        {
            roomName.Replace(lockSymbol, "");
        }

        toggleVisibleButton.onClick.AddListener(toggleVisible);

        int roomNameLength = roomName.Length;
        for (int i = 0; i < roomNameLength; i++)
        {
            hiddenText = hiddenText + "*";
        }
        roomNameText.text = hiddenText;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void toggleVisible()
    {
        roomNameText.text = roomNameText.text == roomName ? hiddenText : roomName;
    }
}
