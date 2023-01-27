using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetHighScore : MonoBehaviour
{

    //ƒ‰ƒ“ƒLƒ“ƒO‚Ì‚TˆÊˆÈ“à‚Å‚ ‚ê‚ÎƒXƒRƒA‚ð“o˜^&•À‚Ñ‘Ö‚¦
    public static void ToSetHighscore()
    {
        int i = GameStart.Stage - 1;
        if (GameStart.Stage != 4)
        {
            if (GameMode.goaledPlayer[0] == "")
            {
                GameMode.goaledPlayer[0] = "player";
            }
            for (int j = 0; j < 5; j++)
            {
                if (GameMode.clearTime[0] < ShowHighScore.topScore[i , j])
                {                  
                    ShowHighScore.topScore[i, j] = GameMode.clearTime[0];
                    ShowHighScore.topName[i, j] = GameMode.goaledPlayer[0];
                }
            }
        }
        else
        {
            if (GameMode.plasement[0] == "")
            {
                GameMode.plasement[0] = "player";
            }
            for (int j = 0; j < 5; j++)
            {
                if (GameMode.pointsInOrder[0] < ShowHighScore.topScore[i , j])
                {
                    ShowHighScore.topScore[i, j] = GameMode.pointsInOrder[0];
                    ShowHighScore.topName[i, j] = GameMode.plasement[0];
                }
            }
        }
      
    }   
}
