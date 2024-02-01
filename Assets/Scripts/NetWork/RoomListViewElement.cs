using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListViewElement : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI nameLabel = default;
    [SerializeField]
    private TextMeshProUGUI playerCounter = default;

    RoomList roomList = new RoomList();
    private MatchmakingView matchmakingView;
    private Button button;
    bool isOpen;
    int maxPlayers = 4;
    void Start() 
    {
        button = this.GetComponent<Button>();
    }
    void Update() 
    {
        int playerCount = 1;
        if (roomList.TryGetRoomInfo(nameLabel.text, out RoomInfo room))
        {
            playerCount = room.PlayerCount;

            isOpen = true;  
            if (room.CustomProperties.TryGetValue("Password", out object passwordObj) && passwordObj is string correctPassword)
            {
                // �p�X���[�h���ݒ肳��Ă���ꍇ��isOpen = false
                if (correctPassword != "")
                {
                    isOpen = false;
                }
            }
        }
    
        button.interactable = (playerCount < maxPlayers);
    }
    public void Init(MatchmakingView parentView)
    {
        matchmakingView = parentView;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
            PhotonNetwork.JoinRoom(nameLabel.text);
    }
     
    public void Show(RoomInfo roomInfo)
    {
        nameLabel.text = roomInfo.Name;
        playerCounter.SetText("{0} / {1}", roomInfo.PlayerCount, roomInfo.MaxPlayers);

        // ���[���������łȂ����̂݁A�Q���{�^����������悤�ɂ���
        button.interactable = (roomInfo.PlayerCount < roomInfo.MaxPlayers);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}