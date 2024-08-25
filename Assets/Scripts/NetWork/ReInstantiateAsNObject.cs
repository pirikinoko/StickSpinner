using Photon.Pun;
using System;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ReInstantiateAsNObject : MonoBehaviour
{
    string name;
    Vector2 pos;
    Transform parent;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameStart.gameMode1 != "Online" || this.GetComponent<PhotonView>() != null)
        {
            return;
        }

        parent = transform.parent;
        pos = transform.position;
        name = gameObject.name;

        if(NetWorkMain.leaderId == NetWorkMain.netWorkId) 
        {
            // �I�u�W�F�N�g���l�b�g���[�N��Ő���
            GameObject instantiatedObject = PhotonNetwork.Instantiate(name, pos, Quaternion.identity);
            // �������ꂽ�I�u�W�F�N�g�̐e��ݒ�
            instantiatedObject.transform.parent = parent;
        }

        // PhotonView ���A�^�b�`����Ă��邩�m�F���A�Ȃ���΃I�u�W�F�N�g��j��
        if (this.GetComponent<PhotonView>() == null)
        {
            Destroy(gameObject);
        }
    }

}
