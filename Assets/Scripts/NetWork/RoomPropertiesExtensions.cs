using ExitGames.Client.Photon;
using Photon.Realtime;

public static class RoomPropertiesExtensions
{
    private const string RankKey = "Rank";

    // Hashtableにプレイヤーのランクを設定する
    public static void SetPlayerRank(this Hashtable hashtable, Player player)
    {
        hashtable[RankKey] = player.GetRank();
    }

    // ロビーから取得できるカスタムプロパティのキーの配列を返す
    public static string[] KeysForLobby(this Hashtable hashtable)
    {
        return new[] { RankKey };
    }
}