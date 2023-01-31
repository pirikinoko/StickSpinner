using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetHighScore : MonoBehaviour
{

    //ランキングの５位以内であればスコアを登録&並び替え
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
