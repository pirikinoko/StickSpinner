using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    public static string TypedTextToString;
    string TypedText;   
    public Text Typedtext;


    // Update is called once per frame
    void Update()
    {
        TypedTextToString = Typedtext.text;
    }
    void KeyInput()//基本のキー入力
    {
        //タイプしたテキストを表示
        Typedtext.text = TypedText;
        TypedTextToString = Typedtext.text.ToString();
    }
}
