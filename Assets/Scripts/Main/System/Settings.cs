using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject SettingPanel, TLFrame , item1, item2;
    public static bool SettingPanelActive = false, inSetting = false;
    bool InputCrossX, InputCrossY;
    int Selected = 1;
    const int max = 2, min = 1;
    Vector2[] itemPos = new Vector2[2];
    Controller controller;
    void Start()
    {
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
  
   

    void SettingControl()
    {
        if (inSetting)
        {
            itemPos[0] = item1.transform.position;
            itemPos[1] = item2.transform.position;
            Transform TLFrameTransform = TLFrame.transform;
            Vector2 TLFramePos = TLFrameTransform.position;
            for(int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (inSetting)
                {
                    /*設定項目の選択*/
                    if (controller.playerKey.Y == 0) { InputCrossY = false; }                
                    if (controller.playerKey.Y >= 0.1f && InputCrossY == false) { Selected++; InputCrossY = true; SoundEffect.BunTrigger = 1; }
                    if (controller.playerKey.Y <= -0.1f && InputCrossY == false) { Selected--; InputCrossY = true; SoundEffect.BunTrigger = 1; }
                    /*/設定項目の選択*/

                    /*音量変更*/
                    if (controller.playerKey.X == 0) { InputCrossX = false; }
                    if (controller.playerKey.X >= 0.1f && InputCrossX == false && inSetting)
                    {
                        if (Selected == 1)
                        {
                            BGM.BGMStage++;

                        }
                        else if (Selected == 2)
                        {
                            SoundEffect.SEStage++;
                        }
                        InputCrossX = true;
                        SoundEffect.BunTrigger = 1;
                    }
                    if (controller.playerKey.X <= -0.1f && InputCrossX == false && inSetting)
                    {
                        if (Selected == 1)
                        {
                            BGM.BGMStage--;
                        }
                        else if (Selected == 2)
                        {
                            SoundEffect.SEStage--;
                        }
                        InputCrossX = true;
                        SoundEffect.BunTrigger = 1;
                    }
                    /*/音量変更*/
                }
            }               
            Selected = Mathf.Clamp(Selected, min, max);
            TLFramePos.y = itemPos[Selected - 1].y;
            TLFrameTransform.position = TLFramePos;
        }  
    }
}
