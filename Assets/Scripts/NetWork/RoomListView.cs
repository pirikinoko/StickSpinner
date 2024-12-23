using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListView : NetworkBehaviour
{
    private const int MaxElements = 20;

    [SerializeField]
    private SessionListViewElement elementPrefab = default;

    private SessionList sessionList = new SessionList();
    private List<SessionListViewElement> elementList = new List<SessionListViewElement>(MaxElements);
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

    public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // ingameLog.GenerateIngameLog("OnSessionListUpdated is Triggered");
        this.sessionList.Update(sessionList);

        // 存在するルームの数だけルームリスト要素を表示する
        int index = 0;
        quickRoomCount = 0;
        foreach (var sessionInfo in this.sessionList)
        {
            if (!sessionInfo.Name.Contains("!Locked!") && !sessionInfo.Name.Contains("!Quick!"))
            {
                elementList[index++].Show(sessionInfo);
                Debug.Log($"Room Name: {sessionInfo.Name}");
            }
            if (sessionInfo.Name.Contains("!Quick!"))
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

