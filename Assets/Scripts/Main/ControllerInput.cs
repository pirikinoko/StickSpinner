using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class ControllerInput : MonoBehaviour
{
    string[] controller = new string[4];
    int[] controllerNumber = new int[4];
    int connected = 0;                             //接続されているコントローラーの数
    string[] joystickNames;
    public static bool usingController = false;
    //入力 ４台まで
    public static float[] LstickX = new float[4];
    public static float[] LstickY = new float[4];
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
    //ボタン選択
    GameObject buttonObject;
    Vector2 cursorPos;
    float lastLstickX, lastLstickY;
    string currentObject;
    void Start()
    {
        connected = 0;
        if (controller[0] == "")
        {
            usingController = false;
        }
        for (int i = 0; i < 4; i++)
        {
            controller[i] = null;
            joystickNames = null;
            jump[i] = false;
            next[i] = false;
            back[i] = false;
            plus[i] = false;
            minus[i] = false;
            start[i] = false;
            nextHold[i] = false;
            backHold[i] = false;

        }


    }

    void Update()
    {
        getControllerType();
        InputControllerButton();
        CheckControllerState();
    
        lastLstickX = LstickX[0];
        lastLstickY = LstickY[0];
    }

    //コントローラー1-4の種類を判別
    private void getControllerType()
    {
        string[] joystickNames = Input.GetJoystickNames();

        connected = joystickNames.Length;
        int num1 = 0;
        int num2 = 0;
        for (int i = 0; i < joystickNames.Length; i++)
        {
            if (joystickNames[i] != "")  //接続されたコントローラーがUnity上で何番目と認識されているか調べる
            {
                controllerNumber[num1] = i;
                num1++;
            }
        }
        for (int j = 0; j < joystickNames.Length; j++)
        {
            if (joystickNames[j] != "")  //Unity上で認識されているコントローラー番号の低い順から1,2,3,4の番号をつける
            {
                joystickNames[num2] = joystickNames[j];
                if (j != num2) { joystickNames[j] = ""; }
                num2++;
            }
        }
        for (int i = joystickNames.Length - 1; i >= 0; i--)
        {
            if (joystickNames[i] == "")　 //配列内で空になったインデックスの分左に詰める
            {
                Array.Resize<string>(ref joystickNames, joystickNames.Length - 1);
                connected--;
            }

        }


        for (int i = 0; i < joystickNames.Length; i++)
        {
            controller[i] = CheckControllerName(joystickNames[i]);  //コントローラーの種類を検知
                                                                    // Debug.Log(" "+connected + "台接続 " +(i+1)+" 台目:" +joystickNames[i] + " " + controller[i]);
        }

    }

    //コントローラーの名前によって種類を判別
    string CheckControllerName(string ControllerName)
    {
        if (ControllerName.ToLower().Contains("f310") || ControllerName.ToLower().Contains("logicool"))
        {
            return "Logicool";
        }
        else if (ControllerName.ToLower().Contains("xbox") || ControllerName.ToLower().Contains("game"))
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

    void InputControllerButton()　//コントローラー入力(種類別)
    {
        for (int i = 0; i < connected; i++)
        {
            if (controller[i] == "XBOX" || controller[i] == "OTHER")
            {
                LstickX[i] = Input.GetAxis("Horizontal_" + (controllerNumber[i] + 1).ToString());
                LstickY[i] = Input.GetAxis("Vertical_" + (controllerNumber[i] + 1).ToString());
                crossX[i] = Input.GetAxis("XCrossX_" + (controllerNumber[i] + 1).ToString());
                crossY[i] = Input.GetAxis("XCrossY_" + (controllerNumber[i] + 1).ToString());
                jump[i] = Input.GetButtonDown("XJump_" + (controllerNumber[i] + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (controllerNumber[i] + 1).ToString());
                back[i] = Input.GetButtonDown("XAButton_" + (controllerNumber[i] + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (controllerNumber[i] + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (controllerNumber[i] + 1).ToString());
                start[i] = Input.GetButtonDown("XStart_" + (controllerNumber[i] + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (controllerNumber[i] + 1).ToString());
                backHold[i] = Input.GetButton("XAButton_" + (controllerNumber[i] + 1).ToString());
            }

            if (controller[i] == "PS")
            {
                LstickX[i] = Input.GetAxis("Horizontal_" + (controllerNumber[i] + 1).ToString());
                LstickY[i] = Input.GetAxis("Vertical_" + (controllerNumber[i] + 1).ToString());
                crossX[i] = Input.GetAxis("PSCrossX_" + (controllerNumber[i] + 1).ToString());
                crossY[i] = Input.GetAxis("PSCrossY_" + (controllerNumber[i] + 1).ToString());
                jump[i] = Input.GetButtonDown("PSJump_" + (controllerNumber[i] + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (controllerNumber[i] + 1).ToString());
                back[i] = Input.GetButtonDown("XAButton_" + (controllerNumber[i] + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (controllerNumber[i] + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (controllerNumber[i] + 1).ToString());
                start[i] = Input.GetButtonDown("PSStart_" + (controllerNumber[i] + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (controllerNumber[i] + 1).ToString());
                backHold[i] = Input.GetButton("XAButton_" + (controllerNumber[i] + 1).ToString());
            }

            if (controller[i] == "Logicool")
            {
                LstickX[i] = Input.GetAxis("Horizontal_" + (controllerNumber[i] + 1).ToString());
                LstickY[i] = Input.GetAxis("Vertical_" + (controllerNumber[i] + 1).ToString());
                crossX[i] = Input.GetAxis("LogiCrossX_" + (controllerNumber[i] + 1).ToString());
                crossY[i] = Input.GetAxis("LogiCrossY_" + (controllerNumber[i] + 1).ToString());
                jump[i] = Input.GetButtonDown("Logi/PSJump_" + (controllerNumber[i] + 1).ToString());
                next[i] = Input.GetButtonDown("Next_" + (controllerNumber[i] + 1).ToString());
                back[i] = Input.GetButtonDown("XAButton_" + (controllerNumber[i] + 1).ToString());
                plus[i] = Input.GetButtonDown("Plus_" + (controllerNumber[i] + 1).ToString());
                minus[i] = Input.GetButtonDown("Minus_" + (controllerNumber[i] + 1).ToString());
                start[i] = Input.GetButtonDown("Logi/PSStart_" + (controllerNumber[i] + 1).ToString());
                nextHold[i] = Input.GetButton("Next_" + (controllerNumber[i] + 1).ToString());
                backHold[i] = Input.GetButton("XAButton_" + (controllerNumber[i] + 1).ToString());

            }
        }
    }

    void CheckControllerState()　//コントローラー使用中かクリックで操作中か調べる
    {
        bool anyButtonInput = false;
        bool keyORMouseInput = false;
        for (int i = 0; i < connected; i++)
        {
            if (start[i] || back[i] || next[i] || jump[i] || plus[i] || minus[i] || crossX[i] != 0 || crossY[i] != 0 || LstickX[i] != 0)
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
        else if (keyORMouseInput)
        {
            usingController = false;
        }

    }

   

    
}
