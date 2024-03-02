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
    IngameLog ingameLog;
    void Start() 
    {
        ingameLog = GameObject.Find("Systems").GetComponent<IngameLog>();
    }
    public void Init(MatchmakingView parentView)
    {
        scrollRect = GetComponent<ScrollRect>();

        // ルームリスト要素（ルーム参加ボタン）を生成して初期化する
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

        // 存在するルームの数だけルームリスト要素を表示する
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
        Debug.Log("クイックマッチルームの数は" + quickRoomCount + "個です。");
        // 残りのルームリスト要素を非表示にする
        for (int i = index; i < MaxElements; i++)
        {
            elementList[i].Hide();
        }
    }

    
}