using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
   public int[] singleHighScore = new int[10], multiHighScore = new int[10], singleArcadeHighScore = new int[10], multiArcadeHighScore = new int[10];
    public float  BGM, SE;
    public int languageNum;
}