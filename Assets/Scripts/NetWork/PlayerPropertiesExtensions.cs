using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
public static class PlayerPropertiesExtensions
{
    private const string ScoreKey = "Score";

    private static readonly Hashtable propsToSet = new Hashtable();

    // プレイヤーのスコアを取得する
    public static int GetScore(this Player player)
    {
        return (player.CustomProperties[ScoreKey] is int score) ? score : 0;
    }

    // プレイヤーのスコアを加算する
    public static void AddScore(this Player player, int value)
    {
        propsToSet[ScoreKey] = player.GetScore() + value;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }



    private const string RankKey = "Rank";

    // プレイヤーのランクの配列
    private static readonly string[] ranks = { "A", "B", "C" };

    // プレイヤーのランクを取得する
    public static string GetRank(this Player player)
    {
        if (player.CustomProperties[RankKey] is string rank)
        {
            return rank;
        }
        else
        {
            return ranks[ranks.Length - 1];
        }
    }

    // プレイヤーのランクをランダムに設定する
    public static void SetRandomRank(this Player player)
    {
        propsToSet[RankKey] = ranks[Random.Range(0, ranks.Length)];
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }


}