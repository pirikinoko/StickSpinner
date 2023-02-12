using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject SettingPanel, TLFrame;
    public static bool SettingPanelActive = false, inSetting = false;
    bool InputCrossX, InputCrossY;
    int Selected = 0;
    float[] settingStages = new float[2];
    int max, min = 0;
    public GameObject[] item;
    Vector2[] itemPos = new Vector2[2];
    public static float[] rotStage = { 10, 10, 10, 10 };　//感度を保存しておく
    Controller controller;
    void Start()
    {
        max = settingStages.Length - 1;
        Selected = 0;
        SettingPanelActive = false;
        inSetting = false;
    }
    void Update()
    {
        if (SettingPanelActive)
        {
            SettingPanel.gameObject.SetActive(true);
        }
        else
        {
            SettingPanel.gameObject.SetActive(false);
        }
        SettingControl();
    }
  
   
    //@@一旦保留
    void SettingControl()
    {

            //設定項目の割り当て
            settingStages[0] = BGM.BGMStage;
            settingStages[1] = SoundEffect.SEStage;

            itemPos[0] = item[0].transform.position;
            itemPos[1] = item[1].transform.position;
            Transform TLFrameTransform = TLFrame.transform;
            Vector2 TLFramePos = TLFrameTransform.position;
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (inSetting)
            {
                if (ControllerInput.crossX[0] == 0) { InputCrossX = false; }
                if (ControllerInput.crossY[0] == 0) { InputCrossY = false; }

                /*設定項目の選択*/
                if (InputCrossY == false)
                {
                    if (ControllerInput.crossY[0] >= 0.1f) { Selected++; InputCrossY = true; SoundEffect.BunTrigger = 1; }
                    else if (ControllerInput.crossY[0] <= -0.1f) { Selected--; InputCrossY = true; SoundEffect.BunTrigger = 1; }
                    //上限下限の設定
                    Selected = Mathf.Clamp(Selected, min, max);
                }
                /*設定項目の選択*/

                /*数値変更*/
                if (InputCrossX == false)
                {
                    if (ControllerInput.crossX[0] >= 0.1f) { settingStages[Selected]++; InputCrossX = true; SoundEffect.BunTrigger = 1; }
                    else if (ControllerInput.crossX[0] <= -0.1f) { settingStages[Selected]--; InputCrossX = true; SoundEffect.BunTrigger = 1; }
                }
                /*数値変更*/

            }
            //上限下限の設定
            rotStage[i] = System.Math.Min(rotStage[i], 20);
            rotStage[i] = System.Math.Max(rotStage[i], 1);
        }

        //変更した数値を各スクリプトに反映
        BGM.BGMStage = settingStages[0];
        SoundEffect.SEStage = settingStages[1];


        TLFramePos.y = itemPos[Selected].y;
        TLFrameTransform.position = TLFramePos;

    }
}
