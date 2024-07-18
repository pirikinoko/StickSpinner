using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int[] singleHighScore = new int[10];
    public int[] multiHighScore = new int[10];
    public int[] singleArcadeHighScore = new int[10];
    public int[] multiArcadeHighScore = new int[10];

    public float BGM = 10;
    public float SE = 5;

    public int languageNum = 0;
    public int screenModeNum = 0;

}