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
        inputField.text = null; //èâä˙âª
    }
    public void GetText()
    {
        GameMode.goaledPlayer[0] = inputField.text;
        SoundEffect.PironTrigger = 1;
        
    }
    public void GetText2()
    {
        GameMode.plasement[0] = inputField.text;
        SoundEffect.PironTrigger = 1;
    }

}
