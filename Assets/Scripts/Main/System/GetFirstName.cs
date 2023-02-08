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
    public void GetText()
    {
        if(GameStart.Stage == 4)
        {
            GameMode.plasement[0] = inputField.text;
        }
        else
        {
            GameMode.goaledPlayer[0] = inputField.text;
        }
       
        SoundEffect.PironTrigger = 1;
    }
}
