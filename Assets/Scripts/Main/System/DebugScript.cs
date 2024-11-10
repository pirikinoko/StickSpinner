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
        GameStart.gameMode1 = targetMode1;
        GameStart.gameMode2 = targetMode2;
        GameStart.stage = targetStage;
        GameStart.flagTimeLimit  = timeLimit;
        GameStart.PlayerCount = playerNumber;
        GameStart.teamMode = teamMode;
        GameStart.teamSize = teamSize;
    }

}
