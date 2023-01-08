using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleMode : MonoBehaviour
{
    public static float KillLogTimer;
    public static float[] points = new float[4], pointsInOrder = new float[4];
    private Text P1Text, P2Text, P3Text, P4Text;
    float[] pointCut;
    public Text[] pointText;
    public static string[] plasement;
    public static Text killer, died;
    public Text[] resultText;
    public Text KillLogText = null;
    public GameObject ResultPanel, ResultPanelFront, InputField, KillLogBack, Plus1, Plus5;
    public static byte[] playParticle;
    public GameObject[] players;
    public GameObject[] sticks;
    public GameObject[] nameTag;
    public GameObject[] pointBox;
    public GameObject[] pointTextGO;
    byte count = 0;
    public static bool Finished;
    private Vector2[] particlePos;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            plasement[i] = null;
            nameTag[i] = GameObject.Find("P"+ (i + 1).ToString() + "Text");
            players[i] = GameObject.Find("Player" +  (i + 1).ToString());
            sticks[i] = GameObject.Find("Stick"+ (i + 1).ToString());
            points[i] = 0;
            pointsInOrder[i] = 0;
        }
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);
        count = 0;
        KillLogTimer = 0;
        Finished = false;
        //画面上部スコアプレイヤー数分表示
        for (int i = 0; i < GameStart.PlayerNumber; i++) { pointBox[i].gameObject.SetActive(true); pointTextGO[i].gameObject.SetActive(true); }
        for (int i = 4; i > GameStart.PlayerNumber; i--) { pointBox[i].gameObject.SetActive(false); pointTextGO[i].gameObject.SetActive(false); }
    }
    // Update is called once per frame
    void Update()
    {
        KillLog();
        PointDisplay();
        checkResult();
        PlayParticle();
        ShowResult();
    }

    void KillLog()
    {
        KillLogText.text = killer + "　→　" + died;
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
    void PointDisplay() //ポイント小数点以下切り捨て＆表示
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
                nameTag[i].gameObject.SetActive(false);

            }
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);


        }
        if (count == 0)
        {
            //ポイント並び替え
            pointsInOrder = points;
            Array.Sort(pointsInOrder);
            //順位計測
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
        //パーティクル再生
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            particlePos[i] = nameTag[i].gameObject.transform.position;
            particlePos[i].y += 0.3f;
            if (playParticle[i] == 1)
            {
                Instantiate(Plus1, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
                playParticle[i] = 0;
            }
            if (playParticle[i] == 2)
            {
                SoundEffect.KirarinTrigger = 1;
                Instantiate(Plus5, particlePos[i], Quaternion.identity); //パーティクル用ゲームオブジェクト生成
                playParticle[i] = 0;
            }
        }
    }
    void ShowResult()
    {
        //リザルト表示
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            resultText[i].text = "1位: " + plasement[i] + " " + points[0] + "ポイント";
        }
    }
}
