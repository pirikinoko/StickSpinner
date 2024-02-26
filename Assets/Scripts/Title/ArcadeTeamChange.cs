using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ArcadeTeamChange : MonoBehaviour
{
    [SerializeField] KeyCode[] keyLeft, keyRight;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ArcadeControll();
    }


    [PunRPC]
    void TeamChange()
    {
        //チーム選択
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[i] > 0.5f)
            {
                if (GameStart.playerTeam[i] < 3)
                {
                    GameStart.playerTeam[i]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[i] < -0.5f)
            {
                if (GameStart.playerTeam[i] > 0)
                {
                    GameStart.playerTeam[i]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] += 1;
                    }
                }
            }
            /*Lスティック横*/

            /*Lスティック縦*/
            if (ControllerInput.LstickY[i] > 0.5f)
            {
                if (GameStart.playerTeam[i] > 1)
                {
                    GameStart.playerTeam[i] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] += 2;
                    }
                }
            }
            else if (ControllerInput.LstickY[i] < -0.5f)
            {
                if (GameStart.playerTeam[i] < 2)
                {
                    GameStart.playerTeam[i] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[i] -= 2;
                    }
                }
            }
            /*Lスティック縦*/
        }
    }



    void ArcadeControll()
    {
        if(GameStart.PlayerNumber == 2) { return; }
        if(GameStart.Stage == 1)
        {
            //チーム選択
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                /*ボタン選択（縦）*/
                if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
                /*Lスティック横*/
                if (ControllerInput.LstickX[i] > 0.5f || Input.GetKeyDown(keyRight[i]))
                {
                    if (GameStart.playerTeam[i] < 3)
                    {
                        GameStart.playerTeam[i]++;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] -= 1;
                        }
                    }
                }
                else if (ControllerInput.LstickX[i] < -0.5f || Input.GetKeyDown(keyLeft[i]))
                {
                    if (GameStart.playerTeam[i] > 0)
                    {
                        GameStart.playerTeam[i]--;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] += 1;
                        }
                    }
                }
                /*Lスティック横*/

                /*Lスティック縦*/
                if (ControllerInput.LstickY[i] > 0.5f)
                {
                    if (GameStart.playerTeam[i] > 1)
                    {
                        GameStart.playerTeam[i] -= 2;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] += 2;
                        }
                    }
                }
                else if (ControllerInput.LstickY[i] < -0.5f)
                {
                    if (GameStart.playerTeam[i] < 2)
                    {
                        GameStart.playerTeam[i] += 2;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] -= 2;
                        }
                    }
                }
                /*Lスティック縦*/
            }

        }

        else
        {
            //チーム選択
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                /*ボタン選択（縦）*/
                if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
                /*Lスティック横*/
                if (ControllerInput.LstickX[i] > 0.5f || Input.GetKeyDown(keyRight[i]))
                {
                    if (GameStart.playerTeam[i] < 1)
                    {
                        GameStart.playerTeam[i]++;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] -= 1;
                        }
                    }
                }
                else if (ControllerInput.LstickX[i] < -0.5f || Input.GetKeyDown(keyLeft[i]))
                {
                    if (GameStart.playerTeam[i] > 0)
                    {
                        GameStart.playerTeam[i]--;
                        SoundEffect.soundTrigger[3] = 1;
                        if (GameStart.teamSize[GameStart.playerTeam[i]] > GameStart.PlayerNumber - 2)
                        {
                            GameStart.playerTeam[i] += 1;
                        }
                    }
                }
                /*Lスティック横*/
            }

        }
    }
}
