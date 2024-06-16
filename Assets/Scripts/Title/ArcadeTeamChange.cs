using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
public class ArcadeTeamChange : MonoBehaviourPunCallbacks
{
    [SerializeField] KeyCode[] keyLeft, keyRight;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY;
    int lastTeam;
    // Start is called before the first frame update
    void Start()
    {
        lastTeam = GameStart.playerTeam[NetWorkMain.netWorkId];
    }

    // Update is called once per frame
    void Update()
    {
        if(GameStart.gameMode1 == "Multi") 
        {
            ArcadeControll();
        }
        else 
        {
            TeamChange();
        }
    }


    void TeamChange()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int targetId = NetWorkMain.netWorkId - 1;
        if (GameStart.PlayerNumber == 2) { return; }
        if (GameStart.stage == 1)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[0] > 0.1f || lastLstickX[0] < -0.1f || lastLstickY[0] > 0.1f || lastLstickY[0] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[0] > 0.5f || Input.GetKeyDown(keyRight[0]))
            {
                if (GameStart.playerTeam[targetId] < 3)
                {
                    GameStart.playerTeam[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (GameStart.playerTeam[targetId] > 0)
                {
                    GameStart.playerTeam[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/

            /*Lスティック縦*/
            if (ControllerInput.LstickY[0] > 0.5f)
            {
                if (GameStart.playerTeam[targetId] > 1)
                {
                    GameStart.playerTeam[targetId] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] += 2;
                    }
                }
            }
            else if (ControllerInput.LstickY[0] < -0.5f)
            {
                if (GameStart.playerTeam[targetId] < 2)
                {
                    GameStart.playerTeam[targetId] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] -= 2;
                    }
                }
            }
            /*Lスティック縦*/
        }

        else
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[0] > 0.1f || lastLstickX[0] < -0.1f || lastLstickY[0] > 0.1f || lastLstickY[0] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[0] > 0.5f || Input.GetKeyDown(keyRight[0]))
            {
                if (GameStart.playerTeam[targetId] < 1)
                {
                    GameStart.playerTeam[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (GameStart.playerTeam[targetId] > 0)
                {
                    GameStart.playerTeam[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStart.teamSize[GameStart.playerTeam[targetId]] > GameStart.PlayerNumber - 2)
                    {
                        GameStart.playerTeam[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/
        }
        Debug.Log("GameStart.playerTeam[targetId]:" + GameStart.playerTeam[targetId] + "   lastTeam:" + lastTeam);
        if (GameStart.playerTeam[targetId] != lastTeam) 
        {
            NetWorkMain.SetCustomProps<int[]>("playerTeam", GameStart.playerTeam);
            photonView.RPC(nameof(SyncPlayerTeam), RpcTarget.All);
            lastTeam = GameStart.playerTeam[targetId];
        }
   
    }

    [PunRPC]
    void SyncPlayerTeam()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (NetWorkMain.GetCustomProps<int[]>("playerTeam", out int[] valueA))
        {
            GameStart.playerTeam = valueA;
        }
    }
    void ArcadeControll()
    {
        if(GameStart.PlayerNumber == 2) { return; }

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
            if (GameStart.stage == 2)
            {
                //サッカーモードは2チームしかないため縦の入力を受け付けない
                return;
            }
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
}
