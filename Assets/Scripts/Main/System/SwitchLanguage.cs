using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLanguage : MonoBehaviour
{
    [SerializeField] string[] texts;
    [SerializeField] int[] fontSize;
    Text targetText;
    // Start is called before the first frame update
    void Start()
    {
        targetText = this.gameObject.GetComponent<Text>();
        targetText.text = texts[Settings.languageNum];
        targetText.fontSize = fontSize[Settings.languageNum];
    }

    // Update is called once per frame
    void Update()
    {
        targetText.text = texts[Settings.languageNum];
        targetText.fontSize = fontSize[Settings.languageNum];
    }
}
