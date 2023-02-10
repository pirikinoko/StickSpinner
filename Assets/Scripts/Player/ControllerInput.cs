using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ControllerInput : MonoBehaviour
{ 
    string[] controller = new string[4];
    int connected = 0;                             //接続されているコントローラーの数
    string[] joystickNames;
    public static bool usingController = true;
    //入力 ４台まで
    public static float[] Lstick = new float [4];
    public static float[] crossX = new float[4];
    public static float[] crossY = new float[4];
    public static bool[] jump = new bool[4];
    public static bool[] next = new bool[4];
    public static bool[] back = new bool[4];
    public static bool[] plus = new bool[4];
    public static bool[] minus = new bool[4];
    public static bool[] start = new bool[4];
    public static bool[] nextHold = new bool[4];
    public static bool[] backHold = new bool[4];
    void Start()
    {
        if (controller[0] == "")
        {
            usingController = false;
        }
        for(int i = 0; i < 4; i++)
        {
            controller[i] = null;
        }
    }

    void Update()
    {
        getControllerType();
        InputControllerButton();
        CheckControllerState();
    }

    //コントローラー1-4の種類を判別
    private void getControllerType()
    {
        string[] joystickNames = Input.GetJoystickNames();
        connected = joystickNames.Length;
        int num = 0;
        for (int j = 0; j < joystickNames.Length; j++)
        {
            if (joystickNames[j] != "")
            {
                joystickNames[num] = joystickNames[j];
                joystickNames[j] = "";
                num++;
            }
        }
        for (int i = joystickNames.Length - 1; i >= 0; i --)
        {
            if (joystickNames[i] == "")
            {
                Array.Resize<string>(ref joystickNames, joystickNames.Length - 1);
                connected--;
            }

        }


        for (int i = 0; i < joystickNames.Length; i++)
        {
            controller[i] = CheckControllerName(joystickNames[i]);  
            Debug.Log(" "+connected + "台接続 " + joystickNames[i] + " " + controller[i]);
        }
      

    }

    //コントローラーの名前によって種類を判別
    string CheckControllerName(string ControllerName)
    {
        if (ControllerName.ToLower().Contains("f310"))
        {
            return "Logicool";
        }
        else if(ControllerName.ToLower().Contains("xbox") || ControllerName.ToLower().Contains("game"))
        {
            return "XBOX";
        }
        else if (ControllerName.ToLower().Contains("playstation") || ControllerName.ToLower().Contains("wireless"))
        {
            return "PS";
        }      
        else
        {
            return "OTHER";
        }
    }

    void InputControllerButton()
    {
        for (int i = 0; i < 4; i++)
        {
            if(controller[i] ==  "XBOX" || controller[i] == "OTHER")
            {
                Lstick[i] = Input.GetAxis("Horizontal_" + (i + 1).ToString());
                crossX[i] = Input.GetAxis("XCrossX_" + (i + 1).ToString());
                crossY[i] = Input.GetAxis("XCrossY_" + (i + 1).ToString());
                jump[i] = Input.GetButtonDown("XJump_" + (i + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (i + 1).ToString());
                back[i] = Input.GetButtonDown("XBack_" + (i + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (i + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (i + 1).ToString());
                start[i] = Input.GetButtonDown("XStart_" + (i + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (i + 1).ToString());
                backHold[i] = Input.GetButton("XBack_" + (i + 1).ToString());
            }

            if (controller[i] == "PS")
            {
                Lstick[i] = Input.GetAxis("Horizontal_" + (i + 1).ToString());
                crossX[i] = Input.GetAxis("PSCrossX_" + (i + 1).ToString());
                crossY[i] = Input.GetAxis("PSCrossY_" + (i + 1).ToString());
                jump[i] = Input.GetButtonDown("Logi/PSJump_" + (i + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (i + 1).ToString());
                back[i] = Input.GetButtonDown("Logi/PSBack_" + (i + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (i + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (i + 1).ToString());
                start[i] = Input.GetButtonDown("Logi/PSStart_" + (i + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (i + 1).ToString());
                backHold[i] = Input.GetButton("Logi/PSBack_" + (i + 1).ToString());
            }

            if (controller[i] == "Logicool")
            {
                Lstick[i] = Input.GetAxis("Horizontal_" + (i + 1).ToString());
                crossX[i] = Input.GetAxis("LogiCrossX_" + (i + 1).ToString());
                crossY[i] = Input.GetAxis("LogiCrossY_" + (i + 1).ToString());
                jump[i] = Input.GetButtonDown("Logi/PSJump_" + (i + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (i + 1).ToString());
                back[i] = Input.GetButtonDown("Logi/PSBack_" + (i + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (i + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (i + 1).ToString());
                start[i] = Input.GetButtonDown("Logi/PSStart_" + (i + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (i + 1).ToString());
                backHold[i] = Input.GetButton("Logi/PSBack_"  + (i + 1).ToString());
            }
        }
    }

    void CheckControllerState()
    {
        bool anyButtonInput = false;
        bool keyORMouseInput = false;
        for (int i = 0; i < connected; i++)
        {
            if (start[i] || back[i] || next[i] || jump[i] || plus[i] || minus[i] || crossX[i] != 0 || crossY[i] != 0 || Lstick[i] != 0)
            {
                anyButtonInput = true;
                keyORMouseInput = false;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            keyORMouseInput = true;
            anyButtonInput = false;
        }

        if (anyButtonInput)
        {
            usingController = true;
        }
        else if(keyORMouseInput)
        {
            usingController = false;
        }

    }

/*
// スティック
public float GetLStick()
{
    float holizontal = Input.GetAxis("Horizontal_" + id.ToString());
    return holizontal;
}
public float GetCrossX()
{
    float crossx = 0;
    if (controller == "XBOX" || controller == "OTHER")
    {
        crossx = Input.GetAxis("XCrossX_" + id.ToString());
    }
    if (controller == "PS")
    {
        crossx = Input.GetAxis("PSCrossX_" + id.ToString());
    }
    if (controller == "Logicool")
    {
        crossx = Input.GetAxis("LogiCrossX_" + id.ToString());
    }
    return crossx;
}
// 押した瞬間
public bool GetNextButtonDown()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "Next_";
    }
    if (controller == "PS")
    {
        inputName = "Next_";
    }
    if (controller == "Logicool")
    {
        inputName = "Next_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetBackButtonDown()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XBack_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSBack_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSBack_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetStartButtonDown()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XStart_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSStart_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSStart_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetJumpButtonDown()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XJump_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSJump_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSJump_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}

//押している間
public bool GetNextButtonHold()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "Next_";
    }
    if (controller == "PS")
    {
        inputName = "Next_";
    }
    if (controller == "Logicool")
    {
        inputName = "Next_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetBackButtonHold()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XBack_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSBack_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSBack_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}

public bool GetStartButtonHold()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XStart_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSStart_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSStart_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetJumpButtonHold()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XJump_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSJump_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSJump_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}


//離したとき
public bool GetNextButtonUp()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "Next_";
    }
    if (controller == "PS")
    {
        inputName = "Next_";
    }
    if (controller == "Logicool")
    {
        inputName = "Next_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetBackButtonUp()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XBack_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSBack_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSBack_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetStartButtonUp()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XStart_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSStart_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSStart_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
public bool GetJumpButtonUp()
{
    string inputName = null;
    if (controller == "XBOX" || controller == "OTHER")
    {
        inputName = "XJump_";
    }
    if (controller == "PS")
    {
        inputName = "Logi/PSJump_";
    }
    if (controller == "Logicool")
    {
        inputName = "Logi/PSJump_";
    }
    return Input.GetButtonDown(inputName + id.ToString());
}
*/
}
