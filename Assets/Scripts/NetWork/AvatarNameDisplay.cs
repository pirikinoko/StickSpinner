using Photon.Pun;
using TMPro;
using UnityEngine.UI;

// MonoBehaviourPunCallbacks���p�����āAphotonView�v���p�e�B���g����悤�ɂ���
public class AvatarNameDisplay : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        var nameLabel = GetComponent<TextMeshPro>();
        // �v���C���[���ƃv���C���[ID��\������
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
        // �v���C���[���ƃv���C���[ID�ƃv���C���[�̃����N��\������
        var nickName = photonView.Owner.NickName;
        var id = photonView.OwnerActorNr;
        var rank = photonView.Owner.GetRank();
        nameLabel.text = $"{nickName}({id})[{rank}]";
    }
}