using Fusion;
using UnityEngine;
public class ArcadeTeamChange : NetworkBehaviour
{
    [SerializeField] KeyCode[] keyLeft, keyRight;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY;
    int lastTeam;
    // Start is called before the first frame update
    void Start()
    {
        lastTeam = GameStartHandler.PlayerTeam[NetWorkMain.NetWorkId];
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStartHandler.GameMode1 == "Multi")
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
        int targetId = NetWorkMain.NetWorkId - 1;
        if (GameStartHandler.PlayerCount == 2) { return; }
        if (GameStartHandler.Stage == 1)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[0] > 0.1f || lastLstickX[0] < -0.1f || lastLstickY[0] > 0.1f || lastLstickY[0] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[0] > 0.5f || Input.GetKeyDown(keyRight[0]))
            {
                if (GameStartHandler.PlayerTeam[targetId] < 3)
                {
                    GameStartHandler.PlayerTeam[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (GameStartHandler.PlayerTeam[targetId] > 0)
                {
                    GameStartHandler.PlayerTeam[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/

            /*Lスティック縦*/
            if (ControllerInput.LstickY[0] > 0.5f)
            {
                if (GameStartHandler.PlayerTeam[targetId] > 1)
                {
                    GameStartHandler.PlayerTeam[targetId] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] += 2;
                    }
                }
            }
            else if (ControllerInput.LstickY[0] < -0.5f)
            {
                if (GameStartHandler.PlayerTeam[targetId] < 2)
                {
                    GameStartHandler.PlayerTeam[targetId] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] -= 2;
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
                if (GameStartHandler.PlayerTeam[targetId] < 1)
                {
                    GameStartHandler.PlayerTeam[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (GameStartHandler.PlayerTeam[targetId] > 0)
                {
                    GameStartHandler.PlayerTeam[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/
        }
        Debug.Log("GameStart.playerTeam[targetId]:" + GameStartHandler.PlayerTeam[targetId] + "   lastTeam:" + lastTeam);
        if (GameStartHandler.PlayerTeam[targetId] != lastTeam)
        {
            NetWorkMain.SetCustomProps<int[]>("playerTeam", GameStartHandler.PlayerTeam);
            SyncPlayerTeam();
            lastTeam = GameStartHandler.PlayerTeam[targetId];
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SyncPlayerTeam()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (NetWorkMain.GetCustomProps<int[]>("playerTeam", out int[] valueA))
        {
            GameStartHandler.PlayerTeam = valueA;
        }
    }
    void ArcadeControll()
    {
        if (GameStartHandler.PlayerCount == 2) { return; }

        //チーム選択
        for (int i = 0; i < GameStartHandler.PlayerCount; i++)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[i] > 0.1f || lastLstickX[i] < -0.1f || lastLstickY[i] > 0.1f || lastLstickY[i] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInput.LstickX[i] > 0.5f || Input.GetKeyDown(keyRight[i]))
            {
                if (GameStartHandler.PlayerTeam[i] < 3)
                {
                    GameStartHandler.PlayerTeam[i]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[i] -= 1;
                    }
                }
            }
            else if (ControllerInput.LstickX[i] < -0.5f || Input.GetKeyDown(keyLeft[i]))
            {
                if (GameStartHandler.PlayerTeam[i] > 0)
                {
                    GameStartHandler.PlayerTeam[i]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[i] += 1;
                    }
                }
            }
            /*Lスティック横*/
            if (GameStartHandler.Stage == 2)
            {
                //サッカーモードは2チームしかないため縦の入力を受け付けない
                return;
            }
            /*Lスティック縦*/
            if (ControllerInput.LstickY[i] > 0.5f)
            {
                if (GameStartHandler.PlayerTeam[i] > 1)
                {
                    GameStartHandler.PlayerTeam[i] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[i] += 2;
                    }
                }
            }
            else if (ControllerInput.LstickY[i] < -0.5f)
            {
                if (GameStartHandler.PlayerTeam[i] < 2)
                {
                    GameStartHandler.PlayerTeam[i] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[GameStartHandler.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        GameStartHandler.PlayerTeam[i] -= 2;
                    }
                }
            }
            /*Lスティック縦*/
        }
    }
}
