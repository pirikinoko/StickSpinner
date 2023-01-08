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
            if (Goal.goaledPlayer[0] == "")
            {
                Goal.goaledPlayer[0] = "player";
            }
            for (int j = 0; j < 5; j++)
            {
                if (Goal.clearTime[0] < ShowHighScore.topScore[i , j])
                {                  
                    ShowHighScore.topScore[i, j] = Goal.clearTime[0];
                    ShowHighScore.topName[i, j] = Goal.goaledPlayer[0];
                }
            }
        }
        else
        {
            if (BattleMode.plasement[0] == "")
            {
                BattleMode.plasement[0] = "player";
            }
            for (int j = 0; j < 5; j++)
            {
                if (BattleMode.pointsInOrder[0] < ShowHighScore.topScore[i , j])
                {
                    ShowHighScore.topScore[i, j] = BattleMode.pointsInOrder[0];
                    ShowHighScore.topName[i, j] = BattleMode.plasement[0];
                }
            }
        }
      
    }   
}
