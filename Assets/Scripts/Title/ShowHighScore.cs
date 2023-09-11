using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CI.QuickSave;

public class ShowHighScore : MonoBehaviour
{
    QuickSaveSettings settings = new QuickSaveSettings();
    QuickSaveWriter writer = QuickSaveWriter.Create("Data");
    QuickSaveReader reader = QuickSaveReader.Create("Data");
    public Text highScoreText;
    public static int[] singleHighScore = new int[10], multiHighScore = new int[10], singleArcadeHighScore = new int[10], multiArcadeHighScore = new int[10];
    string[] text = { "ハイスコア", "HighScore" };
    string[] unit = { "秒", "Sec" };

    private void Awake()
    {
        //データロード
        for (int i = 0; i < 10; i++)
        {
            singleHighScore[i] = reader.Read<int>("SingleHighScore" + i.ToString());
            multiHighScore[i] = reader.Read<int>("MultiHighScore" + i.ToString());
            singleArcadeHighScore[i] = reader.Read<int>("SingleArcadeHighScore" + i.ToString());
            multiArcadeHighScore[i] = reader.Read<int>("MultiArcadeHighScore" + i.ToString());
        }
    }

    private void OnApplicationQuit()
    {
        //データ書き込み
        for (int i = 0; i < 10; i++)
        {
            writer.Write("SingleHighScore" + i.ToString(), singleHighScore[i]);
            writer.Write("MultiHighScore" + i.ToString(), multiHighScore[i]);
            writer.Write("SingleArcadeHighScore" + i.ToString(), singleArcadeHighScore[i]);
            writer.Write("MultiArcadeHighScore" + i.ToString(), multiArcadeHighScore[i]);
        }
        writer.Commit();
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
    }
}

