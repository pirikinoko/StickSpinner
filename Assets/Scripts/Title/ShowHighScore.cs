using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHighScore : MonoBehaviour
{

    public Text[,] ranking = new Text[4, 5]; //�@4�̓X�e�[�W�A5��5�ʂ܂ł̈Ӗ�
    public static string[,] topName = new string[4, 5];
    public static float[,] topScore = new float[4, 5];
    string[] unit = { "�b", "�b", "�b", "P" };

    // FrontCanvas �𖳌��ɂ����ゾ�� GameObject.Find �Ō������Ȃ��̂� Awake �ŏ�������
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
                ranking[i, j].text = (j + 1).ToString() + "��: " + topName[i, j] + " " + topScore[i, j].ToString() + unit[i];
                if (topScore[i, j] == 0)
                {
                    ranking[i, j].text = "";
                }
            }
        }
    }
}

