using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RoomNameVisible : MonoBehaviourPunCallbacks
{
    Text roomNameText;
    string hiddenTextStr;
    [SerializeField]
    Button toggleVisibleButton;
    // Start is called before the first frame update
    void Start()
    {
        roomNameText = this.GetComponent<Text>();
        toggleVisibleButton.onClick.AddListener(toggleVisible);

        int roomNameLength = PhotonNetwork.CurrentRoom.Name.Length;
        for (int i = 0; i < roomNameLength; i++)
        {
            hiddenTextStr = hiddenTextStr + "*";
        }
        roomNameText.text = hiddenTextStr;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void toggleVisible() 
    {

        if (roomNameText.text == PhotonNetwork.CurrentRoom.Name)
        {
            roomNameText.text = hiddenTextStr;
        }
        else 
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        }
    }
}
