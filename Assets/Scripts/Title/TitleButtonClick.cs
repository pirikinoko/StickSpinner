using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Photon.Pun;
public class TitleButtonClick : MonoBehaviourPunCallbacks　//クリック用ボタン
{
    const int Title = 0;
    const int SelectStage = 1;
    const int SelectPNumber = 2;
    const int Stage1 = 1;
    const int Stage2 = 2;
    const int Stage3 = 3;
    const int Stage4 = 4;
    public static int[] sensChange = new int[4];
    TitleButton titleButton;
    GameStart gameStart;
    private IngameLog ingameLog = new IngameLog();
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            gameStart = GameObject.Find("Systems").GetComponent<GameStart>();
            titleButton = GameObject.Find("Systems").GetComponent<TitleButton>();
        }
   
    }
   
    void Update() 
    {

    }

        //ゲーム終了ボタン
        public void ExitGame()
    {
        Settings.exitPanelActive = true;
    }
    public void yesExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        #else
        Application.Quit();//ゲームプレイ終了
        #endif
    }
    public void noBack()
    {
        Settings.exitPanelActive = false;
    }
    //次の画面(シングルプレイ)
    public void NextPhaseSingle()
    {
        GameStart.gameMode1 = "Single";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    //次の画面(マルチプレイ)
    public void NextPhase()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
        titleButton.targetNum = 0;
    }
  
    public void SetModeMulti() 
    {
        GameStart.gameMode1 = "Multi";
    }
    public void GoOnlineMode() 
    {
        GameStart.gameMode1 = "Online";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
        titleButton.targetNum = 0;
    }
    public void GoStageSelect()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 5;
    }
    public void GoTeamSelect()
    {
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase = 7;
    }
    public void NextPhaseOnline()
    {
        GameStart.gameMode1 = "Online";
        if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
        titleButton.targetNum = 0;
    }
    //モード変更
    public void setNomalMode() 
    {
        GameStart.gameMode2 = "Nomal";
        NetWorkMain.UpdateRoomStats(1);
        photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
        NetWorkMain.UpdateGameMode("Nomal");
        photonView.RPC(nameof(SetCustomPropsGameMode), RpcTarget.All);
    }
    public void setArcadeMode()
    {
        GameStart.gameMode2 = "Arcade";
        NetWorkMain.UpdateRoomStats(1);
        photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
        NetWorkMain.UpdateGameMode("Arcade");
        photonView.RPC(nameof(SetCustomPropsGameMode), RpcTarget.All);
    }

    public void StartGame()
    {
        if(GameStart.gameMode1 == "Online") 
        {
            photonView.RPC(nameof(RPCStartGame), RpcTarget.All);
            return;
        }
        Debug.Log("ゲームを開始します。");
        SceneManager.LoadScene("Stage");
    }
    [PunRPC]
    void RPCStartGame() 
    {
        SceneManager.LoadScene("Stage");
    }
    public void syncStage()
    {
        if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        NetWorkMain.UpdateRoomStats(GameStart.Stage);
        photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
    }
    //次の画面(通常モード)
    public void NextPhaseNomal()
    {
        GameStart.gameMode2 = "Nomal";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    //次の画面(ミニゲーム)
    public void NextPhaseArcade()
    {
        GameStart.gameMode2 = "Arcade";
        SoundEffect.soundTrigger[2] = 1;
        GameStart.phase++;
    }
    public void PrevPhase()
    {
        GameStart.phase--;
        SoundEffect.soundTrigger[2] = 1;
    }
    
    //部屋を抜ける
    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
    }


    //ステージ変更
    public void NextStage()
    {
        titleButton.targetNum++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void PrevStage()
    {
        titleButton.targetNum--;
        SoundEffect.soundTrigger[3] = 1;
    }

    //プレイヤー数増減
    public void PlusButton()
    {
        if (GameStart.PlayerNumber < 4)
        {
            titleButton.targetNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void MinusButton()
    {
        bool bProcessed = false;

        //ステージ4
        if (GameStart.Stage == Stage4 && GameStart.PlayerNumber > 2)
        {
            bProcessed = true;
        }
        //ステージ4以外
        else if (GameStart.Stage != Stage4 && GameStart.PlayerNumber > 1)
        {
            bProcessed = true;
        }
        if (bProcessed)
        {
            titleButton.targetNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void OpenInfo()
    {
        gameStart.stageInfoActive = true;
    }

    public void CloseInfo()
    {
        gameStart.stageInfoActive = false;
    }
  
    [PunRPC]
     void SetCustomPropsStage() 
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("stage"))
        {
            int stageTmp;
            if (int.TryParse(customProps["stage"].ToString(), out stageTmp))
            {
                GameStart.Stage = stageTmp;
                Debug.Log("GameStart.Stageを" + stageTmp + "に設定しました");
            }
        }
    }

    [PunRPC]
    void SetCustomPropsGameMode()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (customProps.ContainsKey("gameMode"))
        {
                GameStart.gameMode2 = customProps["gameMode"].ToString();
            Debug.Log("GameModeを" + customProps["gameMode"].ToString() + "に設定しました");
        }
    }

    public void SetCustomPropsStageButton() 
    {
        if(GameStart.gameMode1 == "Online") 
        {
            NetWorkMain.UpdateRoomStats(GameStart.Stage);
            photonView.RPC(nameof(SetCustomPropsStage), RpcTarget.All);
        }
      
    }
    public void OpenSetting()    //設定画面の表示
    {
        Settings.SettingPanelActive = !(Settings.SettingPanelActive);
        Settings.inSetting = !(Settings.inSetting);
    }

    public void PlusFlagTime()
    {
        SoundEffect.soundTrigger[3] = 1;
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.netWorkId)
        {
            photonView.RPC(nameof(RPCPlusFlagTime), RpcTarget.All);
            return;
        }
       
        GameStart.flagTimeLimit += 10;
        GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
    }
    public void MinusFlagTime()
    {
        SoundEffect.soundTrigger[3] = 1;
        if (PhotonNetwork.InRoom && NetWorkMain.leaderId == NetWorkMain.netWorkId)
        {
            photonView.RPC(nameof(RPCMinusFlagTime), RpcTarget.All);
            return;
        }
    
        GameStart.flagTimeLimit -= 10;
        GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);

    }
    [PunRPC]
    void RPCPlusFlagTime() 
    {
        GameStart.flagTimeLimit += 10;
        GameStart.flagTimeLimit = System.Math.Min(GameStart.flagTimeLimit, 150);
    }
    [PunRPC]
    void RPCMinusFlagTime()
    {
        GameStart.flagTimeLimit -= 10;
        GameStart.flagTimeLimit = System.Math.Max(40, GameStart.flagTimeLimit);
    }
    //感度変更ボタン
    public void GainSensP1()
    {
        Settings.rotStage[0] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP2()
    {
        Settings.rotStage[1] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP3()
    {
        Settings.rotStage[2] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSensP4()
    {
        Settings.rotStage[3] += 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP1()
    {
        Settings.rotStage[0] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP2()
    {
        Settings.rotStage[1] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP3()
    {
        Settings.rotStage[2] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSensP4()
    {
        Settings.rotStage[3] -= 1;
        SoundEffect.soundTrigger[3] = 1;
    }


    //設定画面のボタン
    public void GainBGMVol()
    {
        BGM.BGMStage++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseBGMVol()
    {
        BGM.BGMStage--;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void GainSEVol()
    {
        SoundEffect.SEStage++;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void LoseSEVol()
    {
        SoundEffect.SEStage--;
        SoundEffect.soundTrigger[3] = 1;
    }
    public void NextLanguage()
    {
        if (Settings.languageNum < 1) 
        {
            Settings.languageNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevLanguage()
    {    
        if (Settings.languageNum > 0)
        {
            Settings.languageNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void NextScreenMode()
    {
        if (Settings.screenModeNum < 1)
        {
            Settings.screenModeNum++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevScreenMode()
    {
        if (Settings.screenModeNum > 0)
        {
            Settings.screenModeNum--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void NextGuideMode()
    {
        if (Settings.guideMode < 1)
        {
            Settings.guideMode++;
            SoundEffect.soundTrigger[3] = 1;
        }
    }
    public void PrevGuideMode()
    {
        if (Settings.guideMode > 0)
        {
            Settings.guideMode--;
            SoundEffect.soundTrigger[3] = 1;
        }
    }

   
    [PunRPC]
    public void RPCsyncLeader()
    {
        photonView.RPC(nameof(syncLeader), RpcTarget.All);
    }
    [PunRPC]
    public void RPCSetTeam()
    {
        SoundEffect.soundTrigger[3] = 1;
        photonView.RPC(nameof(SetTeam), RpcTarget.All, NetWorkMain.netWorkId);
    }
    [PunRPC]
    public void RPCSetLeader()
    {
        if (NetWorkMain.netWorkId != NetWorkMain.leaderId)
        {
            return;
        }
        photonView.RPC(nameof(SetLeader), RpcTarget.All);
    }

    [PunRPC]
    void syncLeader()
    {
        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int idTmp = 1;
        if (customProps.ContainsKey("leaderID"))
        {
            if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
            {
                NetWorkMain.leaderId = idTmp;
            }
        }
    }
    [PunRPC]
    void SetLeader()
    {
        int newLeaderId = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", ""));
        NetWorkMain.leaderId = newLeaderId;

        ExitGames.Client.Photon.Hashtable customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        int idTmp = 0;
        if (customProps.ContainsKey("leaderId"))
        {
            if (int.TryParse(customProps["leaderId"].ToString(), out idTmp))
            {
                customProps["leaderId"] = newLeaderId;
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }
    [PunRPC]
    void SetTeam(int netWorkID)
    {
        int newTeamNum = int.Parse(Regex.Replace(this.gameObject.name, @"[^0-9]", "")) - 1;
        GameStart.playerTeam[netWorkID - 1] = newTeamNum;
    }
    //再接続
    public void ReconnectToLobby() 
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else 
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public void DisconnectLobby()
    {
        PhotonNetwork.LeaveLobby();
    }
}
