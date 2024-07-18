using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Text.RegularExpressions;
public class ButtonActive : MonoBehaviourPunCallbacks
{
    Button button;
    Image image;
    Color imageColor;
    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
        image = this.gameObject.GetComponent<Image>();
        imageColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.InRoom) 
        {
            return;
        }
        if (this.gameObject.name.Contains("Star") && !this.gameObject.name.Contains("Game"))
        {
            int starID = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

            // オリジナルのマテリアルをコピー
            Material iconMat = new Material(this.GetComponent<Image>().material);

            Color color = iconMat.color;

            color.a = 0.2f;
            if (starID == NetWorkMain.leaderId)
            {
                color.a = 1;
            }

            iconMat.color = color;

            // 新しいマテリアルをImageコンポーネントに設定
            this.GetComponent<Image>().material = iconMat;
        }
        else if (this.gameObject.name.Contains("Ready")) 
        {
            if (NetWorkMain.GetCustomProps<bool[]>("isReady", out var valueArrayA)) 
            {
                int myId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));

                Material iconMat = new Material(this.GetComponent<Image>().material);
                Color color = iconMat.color;
                color.a = 0.2f;
                if (valueArrayA[myId - 1] == true)
                {
                    color.a = 1;
                }
                iconMat.color = color;
                this.GetComponent<Image>().material = iconMat;
            }
        }
        else 
        {
            if (GameStart.gameMode1 == "Online" && this.gameObject.activeSelf)
            {

                if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
                {
                    button.interactable = false;
                }
                else { button.interactable = true; }
            }
        }


        //チームセレクトボタンのアクティブ
        if (this.name.Contains("TeamSelect")) 
        {
            if(GameStart.gameMode2 == "Arcade" && GameStart.PlayerNumber > 1) 
            {
                button.interactable = true;
            }
            else { button.interactable = false; }
        }



    }
}
