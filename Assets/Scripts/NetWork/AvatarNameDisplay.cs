using Photon.Pun;
using TMPro;
using UnityEngine.UI;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarNameDisplay : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        var nameLabel = GetComponent<TextMeshPro>();
        // プレイヤー名とプレイヤーIDを表示する
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
        // プレイヤー名とプレイヤーIDとプレイヤーのランクを表示する
        var nickName = photonView.Owner.NickName;
        var id = photonView.OwnerActorNr;
        var rank = photonView.Owner.GetRank();
        nameLabel.text = $"{nickName}({id})[{rank}]";
    }
}