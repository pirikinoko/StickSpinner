using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour　//デバッグ用スクリプト
{
    [SerializeField] int targetStage;
    [SerializeField] int playerNumber;
    [SerializeField] string targetMode1;
    [SerializeField] string targetMode2;
    // Start is called before the first frame update
    void Start()
    {
        GameStart.gameMode1 = targetMode1;
        GameStart.gameMode2 = targetMode2;
        GameStart.Stage = targetStage;
        GameStart.PlayerNumber = playerNumber;
    }

}
