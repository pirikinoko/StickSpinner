using Fusion;
using UnityEngine;
public class TeamChangeController : NetworkBehaviour
{
    private SessionManager _sessionManager;
    [SerializeField] KeyCode[] keyLeft, keyRight;
    float[] lastLstickX = new float[4], lastLstickY = new float[4];
    float lastCrossX, lastCrossY;
    int lastTeam;

    void Start()
    {
        _sessionManager = SessionManager.Instance;
        lastTeam = _sessionManager.PlayerTeam[Runner.LocalPlayer.PlayerId];
    }

    void Update()
    {
        if (_sessionManager.PlayingType == "Multi")
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
        int targetId = Runner.LocalPlayer.PlayerId - 1;
        if (GameStartHandler.PlayerCount == 2) { return; }
        if (_sessionManager.Stage == 1)
        {
            /*ボタン選択（縦）*/
            if (lastLstickX[0] > 0.1f || lastLstickX[0] < -0.1f || lastLstickY[0] > 0.1f || lastLstickY[0] < -0.1f) { return; }
            /*Lスティック横*/
            if (ControllerInputManager.LstickX[0] > 0.5f || Input.GetKeyDown(keyRight[0]))
            {
                if (_sessionManager.PlayerTeam[targetId] < 3)
                {
                    _sessionManager.PlayerTeamToSend[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInputManager.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (_sessionManager.PlayerTeam[targetId] > 0)
                {
                    _sessionManager.PlayerTeamToSend[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/

            /*Lスティック縦*/
            if (ControllerInputManager.LstickY[0] > 0.5f)
            {
                if (_sessionManager.PlayerTeam[targetId] > 1)
                {
                    _sessionManager.PlayerTeamToSend[targetId] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] += 2;
                    }
                }
            }
            else if (ControllerInputManager.LstickY[0] < -0.5f)
            {
                if (_sessionManager.PlayerTeam[targetId] < 2)
                {
                    _sessionManager.PlayerTeamToSend[targetId] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] -= 2;
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
            if (ControllerInputManager.LstickX[0] > 0.5f || Input.GetKeyDown(keyRight[0]))
            {
                if (_sessionManager.PlayerTeam[targetId] < 1)
                {
                    _sessionManager.PlayerTeamToSend[targetId]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] -= 1;
                    }
                }
            }
            else if (ControllerInputManager.LstickX[0] < -0.5f || Input.GetKeyDown(keyLeft[0]))
            {
                if (_sessionManager.PlayerTeam[targetId] > 0)
                {
                    _sessionManager.PlayerTeamToSend[targetId]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[targetId]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[targetId] += 1;
                    }
                }
            }
            /*Lスティック横*/
        }

        Debug.Log("GameStart.playerTeam[targetId]:" + _sessionManager.PlayerTeam[targetId] + "   lastTeam:" + lastTeam);
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
            if (ControllerInputManager.LstickX[i] > 0.5f || Input.GetKeyDown(keyRight[i]))
            {
                if (_sessionManager.PlayerTeam[i] < 3)
                {
                    _sessionManager.PlayerTeamToSend[i]++;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[i] -= 1;
                    }
                }
            }
            else if (ControllerInputManager.LstickX[i] < -0.5f || Input.GetKeyDown(keyLeft[i]))
            {
                if (_sessionManager.PlayerTeam[i] > 0)
                {
                    _sessionManager.PlayerTeamToSend[i]--;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[i] += 1;
                    }
                }
            }
            /*Lスティック横*/
            if (_sessionManager.Stage == 2)
            {
                //サッカーモードは2チームしかないため縦の入力を受け付けない
                return;
            }
            /*Lスティック縦*/
            if (ControllerInputManager.LstickY[i] > 0.5f)
            {
                if (_sessionManager.PlayerTeam[i] > 1)
                {
                    _sessionManager.PlayerTeamToSend[i] -= 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[i] += 2;
                    }
                }
            }
            else if (ControllerInputManager.LstickY[i] < -0.5f)
            {
                if (_sessionManager.PlayerTeam[i] < 2)
                {
                    _sessionManager.PlayerTeamToSend[i] += 2;
                    SoundEffect.soundTrigger[3] = 1;
                    if (GameStartHandler.TeamSize[_sessionManager.PlayerTeam[i]] > GameStartHandler.PlayerCount - 2)
                    {
                        _sessionManager.PlayerTeamToSend[i] -= 2;
                    }
                }
            }
            /*Lスティック縦*/
        }
    }
}
