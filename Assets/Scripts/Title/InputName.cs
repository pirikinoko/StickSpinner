using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    int NumberOfLetters = 0;
    public static string TypedTextToString;
    string TypedText;
    string[] TypedKeys;
    float DeleteCoolDown = 0.1f; const float DefaultDeleteCD = 0.1f;
    bool DelteCDFlag, Pass, inputMode, clickVoid;
    public Text Typedtext;
    const int maxLetters = 10;
    [SerializeField]
    Button inputFrame;
    // Start is called before the first frame update
    void Start()
    {
        inputMode = false;
        clickVoid = false;
        inputFrame.onClick.AddListener(ToggleInputMode);
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        if (Input.GetMouseButton(0))
        {
            inputMode = false;
        }
    }
    void KeyInput()//基本のキー入力
    {
        if (inputMode) 
        {
            if (Input.anyKeyDown && NumberOfLetters < maxLetters)
            {
                if (!(Input.GetKeyDown(KeyCode.Backspace)) && !Input.GetKey(KeyCode.Return))
                {
                    string ATypedKey = Input.inputString;
                    TypedKeys = new string[NumberOfLetters + 1];
                    TypedKeys[NumberOfLetters] = ATypedKey;
                    TypedText = TypedText + TypedKeys[NumberOfLetters];

                    NumberOfLetters = TypedText.Length;
                    Debug.Log(TypedText + NumberOfLetters);
                }
            }


            if (Input.GetKey(KeyCode.Backspace))
            {
                if (NumberOfLetters > 0 && DeleteCoolDown == DefaultDeleteCD)
                {
                    TypedText = TypedText.Remove(NumberOfLetters - 1, 1);
                    DelteCDFlag = true;
                }
                NumberOfLetters = TypedText.Length;
                Debug.Log(TypedText + NumberOfLetters);
            }

            if (DelteCDFlag)
            {
                DeleteCoolDown -= Time.deltaTime;
                if (DeleteCoolDown < 0)
                {
                    DelteCDFlag = false;
                    DeleteCoolDown = DefaultDeleteCD;
                }
            }
        }
      

        //タイプしたテキストを表示
        Typedtext.text = TypedText;
        TypedTextToString = Typedtext.text.ToString();


    }
    void ResetText()
    {
        TypedText = null;
    }

    public void ToggleInputMode() 
    {
        inputMode = true;
    }
}
