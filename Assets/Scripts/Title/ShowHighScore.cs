using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHighScore : MonoBehaviour
{

    public Text[,] ranking = new Text[4, 5]; //　4はステージ、5は5位までの意味
    public static string[,] topName = new string[4, 5];
    public static float[,] topScore = new float[4, 5];
    string[] unit = { "秒", "秒", "秒", "P" };

    // FrontCanvas を無効にした後だと GameObject.Find で見つけられないので Awake で処理する
    void Awake()
    {
        int num = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                num++;
                ranking[i, j] = GameObject.Find("HighScore" + num.ToString()).GetComponent<Text>();
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                ranking[i, j].text = (j + 1).ToString() + "位: " + topName[i, j] + " " + topScore[i, j].ToString() + unit[i];
                if (topScore[i, j] == 0)
                {
                    ranking[i, j].text = "";
                }
            }
        }
    }
}

