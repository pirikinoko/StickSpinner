using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CI.QuickSave;

public class ShowHighScore : MonoBehaviour
{

    public Text highScoreText;
    public static int[] singleHighScore = new int[10], multiHighScore = new int[10], singleArcadeHighScore = new int[10], multiArcadeHighScore = new int[10];
    string[] text = { "ハイスコア", "HighScore" };
    string[] unit = { "秒", "Sec" };
    SaveData data;
    private void Start()
    {
        data = GetComponent<DataManager>().data;
        for (int i = 0; i < 10; i++)
        {
            if (singleHighScore[i] == 0)
            {
                singleHighScore[i] = data.singleHighScore[i];
            }
            if (multiHighScore[i] == 0)
            {
                multiHighScore[i] = data.multiHighScore[i];
            }
            if (singleArcadeHighScore[i] == 0)
            {
                singleArcadeHighScore[i] = data.singleArcadeHighScore[i];
            }
            if (multiArcadeHighScore[i] == 0)
            {
                multiArcadeHighScore[i] = data.multiArcadeHighScore[i];
            }
        }
    }
    private void Update()
    {
        if(GameStart.Stage < 1) { return; }
        if (GameStart.gameMode1 == "Single")
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                highScoreText.text = text[Settings.languageNum] + ":" + singleHighScore[GameStart.Stage - 1] + unit[Settings.languageNum];
            }
            else
            {
                highScoreText.text = text[Settings.languageNum] + ":" + singleArcadeHighScore[GameStart.Stage - 1] + unit[Settings.languageNum];
            }
        }
        else
        {
            if (GameStart.gameMode2 == "Nomal")
            {
                highScoreText.text = text[Settings.languageNum] + ":" + multiHighScore[GameStart.Stage - 1] + unit[Settings.languageNum];
            }
            else
            {
                highScoreText.text = text[Settings.languageNum] + ":" + multiArcadeHighScore[GameStart.Stage - 1] + "P";
            }
        }
        for (int i = 0; i < 10; i++)
        {
            data.singleHighScore[i] = singleHighScore[i];
        }
    }
}

