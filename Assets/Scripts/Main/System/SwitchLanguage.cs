using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLanguage : MonoBehaviour
{
    [SerializeField] string[] texts;
    Text targetText;
    int lastlanguageNum;
    // Start is called before the first frame update
    void Start()
    {
        targetText = this.gameObject.GetComponent<Text>();
        lastlanguageNum = Settings.languageNum;
        targetText.text = texts[Settings.languageNum];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Settings.languageNum != lastlanguageNum)
        {
            targetText.text = texts[Settings.languageNum];
        }
    }
}
