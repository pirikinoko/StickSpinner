using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetHighScore : MonoBehaviour
{
    private void Update()
    {
        if (GameMode.Finished || GameMode.Goaled)
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
                ShowHighScore.singleHighScore[GameStart.Stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.singleHighScore[GameStart.Stage - 1]));
            }
            else
            {
                ShowHighScore.singleArcadeHighScore[GameStart.Stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.singleArcadeHighScore[GameStart.Stage - 1]));
            }
        }
        else
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                ShowHighScore.multiHighScore[GameStart.Stage - 1] = Mathf.Min((int)(GameMode.clearTime[0]), (int)(ShowHighScore.multiHighScore[GameStart.Stage - 1]));
            }
            else
            {
                ShowHighScore.multiArcadeHighScore[GameStart.Stage - 1] = Mathf.Max((int)(GameMode.topPoint), (int)(ShowHighScore.multiHighScore[GameStart.Stage - 1]));
            }
        }
    }
}
