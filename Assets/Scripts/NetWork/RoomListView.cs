using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomListView : MonoBehaviourPunCallbacks
{
    private const int MaxElements = 20;

    [SerializeField]
    private RoomListViewElement elementPrefab = default;

    private RoomList roomList = new RoomList();
    private List<RoomListViewElement> elementList = new List<RoomListViewElement>(MaxElements);
    private ScrollRect scrollRect;
    public int quickRoomCount = 0;
    private IngameLog ingameLog = new IngameLog();
    public void Init(MatchmakingView parentView)
    {
        scrollRect = GetComponent<ScrollRect>();

        // ���[�����X�g�v�f�i���[���Q���{�^���j�𐶐����ď���������
        for (int i = 0; i < MaxElements; i++)
        {
            var element = Instantiate(elementPrefab, scrollRect.content);
            element.Init(parentView);
            element.Hide();
            elementList.Add(element);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
       // ingameLog.GenerateIngameLog("OnRoomListUpdate is Triggered");
        roomList.Update(changedRoomList);

        // ���݂��郋�[���̐��������[�����X�g�v�f��\������
        int index = 0;
        quickRoomCount = 0;
        foreach (var roomInfo in roomList)
        {
            if (!roomInfo.Name.Contains("!Locked!") && !roomInfo.Name.Contains("!Quick!"))
            {
                elementList[index++].Show(roomInfo);
                Debug.Log($"Room Name: {roomInfo.Name}");
            }
            if (roomInfo.Name.Contains("!Quick!")) 
            {
                quickRoomCount++;
            }

        }
        Debug.Log("�N�C�b�N�}�b�`���[���̐���" + quickRoomCount + "�ł��B");
        // �c��̃��[�����X�g�v�f���\���ɂ���
        for (int i = index; i < MaxElements; i++)
        {
            elementList[i].Hide();
        }
    }

    
}