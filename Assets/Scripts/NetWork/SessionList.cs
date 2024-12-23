using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IEnumerable<SessionInfo>インターフェースを実装して、foreachでルーム情報を列挙できるようにする
public class SessionList : IEnumerable<SessionInfo>
{
    private Dictionary<string, SessionInfo> dictionary = new Dictionary<string, SessionInfo>();

    public void Update(List<SessionInfo> changedSessionList)
    {
        foreach (var info in changedSessionList)
        {
            if (info.IsValid)
            {
                dictionary[info.Name] = info;
            }
            else
            {
                dictionary.Remove(info.Name);
            }
        }
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    // 指定したルーム名のルーム情報があれば取得する
    public bool TryGetSessionInfo(string sessionName, out SessionInfo sessionInfo)
    {
        return dictionary.TryGetValue(sessionName, out sessionInfo);
    }

    public IEnumerator<SessionInfo> GetEnumerator()
    {
        foreach (var kvp in dictionary)
        {
            yield return kvp.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void LogAllSessionInfo()
    {
        foreach (var kvp in dictionary)
        {
            Debug.Log($"SessionName: {kvp.Key}, SessionInfo: {kvp.Value}");
        }
    }
}
