using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour　//デバッグ用スクリプト
{
    [SerializeField] int targetStage;
    [SerializeField] int playerNumber;
    [SerializeField] int timeLimit;
    [SerializeField] int[] teamSize;
    [SerializeField] string targetMode1;
    [SerializeField] string targetMode2;
    [SerializeField] string teamMode;
    // Start is called before the first frame update
    void Awake()
    {
        GameStartHandler.GameMode1 = targetMode1;
        GameStartHandler.GameMode2 = targetMode2;
        GameStartHandler.Stage = targetStage;
        GameStartHandler.FlagTimeLimit  = timeLimit;
        GameStartHandler.PlayerCount = playerNumber;
        GameStartHandler.TeamMode = teamMode;
        GameStartHandler.TeamSize = teamSize;
    }

}
