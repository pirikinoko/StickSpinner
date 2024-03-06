using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSwicher : MonoBehaviour
{
    public int num;
    [SerializeField]
    string[] textsJapanese,  textsEnglish;  
    // Start is called before the first frame update
    void Start()
    {
        if(Settings.languageNum == 0)
        {
            this.GetComponent<Text>().text = textsJapanese[num];
        }
        else if(Settings.languageNum == 1)
        {
            this.GetComponent<Text>().text = textsEnglish[num];
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
