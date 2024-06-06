using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFirstName : MonoBehaviour
{
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField.text = null; //初期化
    }
    public void GetText()   //1位は名前を変更可能
    {
        if(GameStart.stage == 4)
        {
            GameMode.playerNameByRank[0] = inputField.text;
        }
        else
        {
            GameMode.goaledPlayer[0] = inputField.text;
        }
       
        SoundEffect.soundTrigger[2] = 1;
    }
}
