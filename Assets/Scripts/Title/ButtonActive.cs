using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
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
        if (GameStart.gameMode1 ==  "Online" && this.gameObject.activeSelf)
        {
        
            if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
            {
                button.interactable = false;
            }
            else { button.interactable = true; }
        }
     

    }
}
