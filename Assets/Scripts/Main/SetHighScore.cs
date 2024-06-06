using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetHighScore : MonoBehaviour
{
    private void Update()
    {
        if (GameMode.Finished || GameMode.Goaled || GameMode.isGameOver)
        {
            if (ControllerInput.jump[0] || Input.GetKeyDown(KeyCode.Return) )
            {
                ToSetHighScore();
            }
        }
    }
    public void ToSetHighScore()
    {
        if (GameStart.gameMode1 == "Single")
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                if(ShowHighScore.singleHighScore[GameStart.stage - 1] == 0) { ShowHighScore.singleHighScore[GameStart.stage - 1] = (int)(GameMode.clearTime[0]); }
                ShowHighScore.singleHighScore[GameStart.stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.singleHighScore[GameStart.stage - 1]));
            }
            else
            {
                if (ShowHighScore.singleHighScore[GameStart.stage - 1] == 0) { ShowHighScore.singleArcadeHighScore[GameStart.stage - 1] = (int)(GenerateStage.maxHeight); }
                ShowHighScore.singleArcadeHighScore[GameStart.stage - 1] = Mathf.Max((int)(GenerateStage.maxHeight), (int)(ShowHighScore.singleArcadeHighScore[GameStart.stage - 1]));
            }
        }
        else
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                if (ShowHighScore.multiHighScore[GameStart.stage - 1] == 0) { ShowHighScore.multiHighScore[GameStart.stage - 1] = (int)(GameMode.clearTime[0]); }
                ShowHighScore.multiHighScore[GameStart.stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.multiHighScore[GameStart.stage - 1]));
            }
            else
            {
                ShowHighScore.multiArcadeHighScore[GameStart.stage - 1] = Mathf.Max((int)(GameMode.pointsInOrder[0]), (int)(ShowHighScore.multiHighScore[GameStart.stage - 1]));
            }
        }
    }
}
