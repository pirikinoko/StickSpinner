using ExitGames.Client.Photon;
using Photon.Realtime;

public static class RoomPropertiesExtensions
{
    private const string RankKey = "Rank";

    // Hashtable�Ƀv���C���[�̃����N��ݒ肷��
    public static void SetPlayerRank(this Hashtable hashtable, Player player)
    {
        hashtable[RankKey] = player.GetRank();
    }

    // ���r�[����擾�ł���J�X�^���v���p�e�B�̃L�[�̔z���Ԃ�
    public static string[] KeysForLobby(this Hashtable hashtable)
    {
        return new[] { RankKey };
    }
}