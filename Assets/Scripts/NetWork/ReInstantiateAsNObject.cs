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
            // オブジェクトをネットワーク上で生成
            GameObject instantiatedObject = PhotonNetwork.Instantiate(name, pos, Quaternion.identity);
            // 生成されたオブジェクトの親を設定
            instantiatedObject.transform.parent = parent;
        }

        // PhotonView がアタッチされているか確認し、なければオブジェクトを破棄
        if (this.GetComponent<PhotonView>() == null)
        {
            Destroy(gameObject);
        }
    }

}
