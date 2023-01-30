using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameMode : MonoBehaviour
{
    //��{
    public static string[] goaledPlayer{ get; set;} = new string[GameStart.MaxPlayer];
    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks   = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    GameObject[] resultTextGO = new GameObject[GameStart.MaxPlayer];
    Text[] resultText = new Text[GameStart.MaxPlayer];
    GameObject ResultPanel;
    GameObject ResultPanelFront, InputField;
    //�ʏ�X�e�[�W
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    //�o�g�����[�h
    public GameObject KillLogBack, Plus1, Plus5;
    GameObject[] pointTextGO = new GameObject[4], pointBox = new GameObject[4];
    public static float[] points = new float[4], pointsInOrder = new float[4];
    public static float KillLogTimer;
    float[] pointCut = new float[4];
    public static string[] plasement = new string [4];
    Text[] pointText = new Text[4];
    public static Text killer, died;
    public Text KillLogText = null;
    public static byte[] playParticle = new byte[4];
    byte count = 0;
    public static bool Finished;
    private Vector2[] particlePos = new Vector2[4];


    void Start()
    {   //��{
        for (int i = 0; i < GameStart.MaxPlayer; i++) //����������
        {        
            nameTags[    i] = GameObject.Find("P" +      (i + 1).ToString() + "Text");
            players[     i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[      i] = GameObject.Find("Stick"  + (i + 1).ToString());
            resultTextGO[i] = GameObject.Find("Result" + (i + 1).ToString());
            resultText[  i] = resultTextGO[i].GetComponent<Text>();
        }
   
        ResultPanel = GameObject.Find("ResultPanel");
        ResultPanelFront = GameObject.Find("ResultPanelFront");
        InputField = GameObject.Find("InputField");
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);

        //�ʏ�X�e�[�W
        if(GameStart.Stage != 4)
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++) //����������
            {
                clearTime[i] = 0;
                goaledPlayer[i] = null;
            }
            Goaled = false;
            Goals = 0;
        }


        //�o�g�����[�h
        if (GameStart.Stage == 4)
        {          
            for (int i = 0; i < 4; i++)
            {
                plasement[i] = null;
                points[i] = 0;
                pointsInOrder[i] = 0;
                pointBox[i] = GameObject.Find("PointFrame" + (i + 1).ToString());
                pointTextGO[i] = GameObject.Find("P" + (i + 1).ToString() +"Point");
                pointText[i] = pointTextGO[i].GetComponent<Text>();

            }

            count = 0;
            KillLogTimer = 0;
            Finished = false;
           
            //��ʏ㕔�X�R�A�v���C���[�����\��
            for (int i = 0; i < GameStart.PlayerNumber; i++) { pointBox[i].gameObject.SetActive(true); pointTextGO[i].gameObject.SetActive(true); }
            for (int i = 3; i >= GameStart.PlayerNumber; i--) { pointBox[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
        }
          
    }

    void Update()
    {

        //�ʏ�X�e�[�W
        if (GameStart.Stage != 4)
        {
            CheckFinish();
        }
        //�o�g�����[�h
        if (GameStart.Stage == 4)
        {
            KillLog();
            PointDisplay();
            checkResult();
            PlayParticle();
            ShowResult();
        }

    }

    //�ʏ�X�e�[�W
    void CheckFinish()
    {
        if (Goals == GameStart.PlayerNumber)
        {
            Goaled = true;
            GameSetting.Playable = false;
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);
            // �Q���v���C���[�����^�C���\��
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                resultText[i].text = (i + 1).ToString() + "��: " + goaledPlayer[i] + " �^�C��:" + clearTime[i] + "�b";
            }
        }
    }
    public void GoalProcess(int playerid)
    {
        // �S�[�������v���C���[��\������
        clearTime[Goals] = GameSetting.PlayTime;
        goaledPlayer[Goals] = "Player" + playerid.ToString();
        SoundEffect.PironTrigger = 1;
        Goals++;
        players[playerid - 1].gameObject.SetActive(false);
        sticks[playerid - 1].gameObject.SetActive(false);
        nameTags[playerid - 1].gameObject.SetActive(false);
    }

    //�o�g�����[�h
    void KillLog()
    {
        KillLogText.text = killer + "�@���@" + died;
        if (KillLogTimer > 0)
        {
            KillLogBack.gameObject.SetActive(true);
            KillLogTimer -= Time.deltaTime;
        }
        if (KillLogTimer <= 0)
        {
            KillLogBack.gameObject.SetActive(false);
            KillLogText.text = null;
        }
    }
    void PointDisplay() //�|�C���g�����_�ȉ��؂�̂ā��\��
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            pointCut[i] = Mathf.Floor(points[i]);
            pointText[i].text = pointCut[i].ToString();
        }
    }
    void checkResult()
    {

        if (GameSetting.PlayTime <= 0)
        {
            Finished = true;
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                players[i].gameObject.SetActive(false);
                sticks[i].gameObject.SetActive(false);
                nameTags[i].gameObject.SetActive(false);

            }
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);
        }
        if (count == 0)
        {
            //�|�C���g���ёւ�
            pointsInOrder = points;
            Array.Sort(pointsInOrder);
            //���ʌv��
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                for (int j = 0; j < GameStart.PlayerNumber; j++)
                {
                    if (points[i] != 0 && pointsInOrder[j] == points[i])
                    {
                        plasement[j] = "Player" + (i + 1).ToString();
                    }
                }

            }
            count = 1;
        }
    }

    void PlayParticle()
    {
        //�p�[�e�B�N���Đ�
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            particlePos[i] = nameTags[i].gameObject.transform.position;
            particlePos[i].y += 0.3f;
            if (playParticle[i] == 1)
            {
                Instantiate(Plus1, particlePos[i], Quaternion.identity); //�p�[�e�B�N���p�Q�[���I�u�W�F�N�g����
                playParticle[i] = 0;
            }
            if (playParticle[i] == 2)
            {
                SoundEffect.KirarinTrigger = 1;
                Instantiate(Plus5, particlePos[i], Quaternion.identity); //�p�[�e�B�N���p�Q�[���I�u�W�F�N�g����
                playParticle[i] = 0;
            }
        }
    }
    void ShowResult()
    {
        //���U���g�\��
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            resultText[i].text = "1��: " + plasement[i] + " " + points[0] + "�|�C���g";
        }
    }
}


