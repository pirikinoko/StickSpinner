using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameSetting : MonoBehaviour
{
    //基本
    [SerializeField]
    Text countDown, playTimeTx;
    [SerializeField] float timeLimit;
    [SerializeField] GameObject canvas, frontCanvas;
    GameObject[] players = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    GameObject[] deadTimer = new GameObject[GameStart.MaxPlayer];
    Vector2[] nameTagPos = new Vector2[GameStart.MaxPlayer];
    Vector2[,] startPos = new Vector2[GameStart.MaxPlayer, GameStart.MaxPlayer];
    string[] startText = { "スタート", "Start" };
    public static bool Playable = false;
    float elapsedTime;
    public static float playTime;
    public static float startTime = 6;
    float SoundTime = 1f;
    bool StartFlag;

    //ステージ切り替え用
    [SerializeField] GameObject[] stageObjectSingle, stageObjectSingleArcade, stageObjectMulti, stageObjectMultiArcade;
    [SerializeField] GameObject[] keyBoardMouseUI, controllerUI, battleModeUI;
    //背景色
    [SerializeField] Tilemap tilemap;
    public GameObject CountDownGO;
    //UI切り替え用
    int UIMode;
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public static Vector2[] respownPos = new Vector2[GameStart.MaxPlayer];
    GameObject[] defaultPlayerPos = new GameObject[GameStart.MaxPlayer];




    void Start()
    {
        canvas.gameObject.SetActive(true);
        frontCanvas.gameObject.SetActive(true);
        Debug.Log("PlayerNumber: " + GameStart.PlayerNumber + " Stage: " + GameStart.Stage);
        countDown = GameObject.Find("CountDown").GetComponent<Text>();
        playTimeTx = GameObject.Find("TimeText").GetComponent<Text>();
        for (int i = 0; i < GameStart.MaxPlayer; i++) //初期化処理
        {
            nameTags[i] = GameObject.Find("P" + (i + 1).ToString() + "NameTag");
            players[i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[i] = GameObject.Find("Stick" + (i + 1).ToString());
            deadTimer[i] = GameObject.Find("P" + (i + 1).ToString() + "CountDown");
            deadTimer[i].SetActive(false);
        }
        //ステージ切り替え
        for (int i = 0; i < stageObjectSingle.Length; i++)
        {
            stageObjectSingle[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectMulti.Length; i++)
        {
            stageObjectMulti[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectSingleArcade.Length; i++)
        {
            stageObjectSingleArcade[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < stageObjectMultiArcade.Length; i++)
        {
            stageObjectMultiArcade[i].gameObject.SetActive(false);
        }
        if (GameStart.gameMode1 == "Single")
        {
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectSingle[GameStart.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTime = 0;
                    elapsedTime = 0;
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    break;
                case "Arcade":
                    stageObjectSingleArcade[GameStart.Stage - 1].gameObject.SetActive(true);
                    tilemap.color = new Color32(50, 50, 50, 255);
                    playTime = 0;
                    elapsedTime = 0;
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    break;
            }
        }
        else
        {
            tilemap.color = new Color32(50, 50, 50, 255);
            switch (GameStart.gameMode2)
            {
                case "Nomal":
                    stageObjectMulti[GameStart.Stage - 1].gameObject.SetActive(true);
                    playTime = 0;
                    elapsedTime = 0;
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(false);
                    }
                    break;

                case "Arcade":
                    stageObjectMultiArcade[GameStart.Stage - 1].gameObject.SetActive(true);
                    playTimeTx.text = timeLimit.ToString();
                    playTime = GameStart.flagTimeLimit;
                    elapsedTime = GameStart.flagTimeLimit;
                    playTimeTx.text = GameStart.flagTimeLimit.ToString();
                    for (int i = 0; i < battleModeUI.Length; i++)
                    {
                        battleModeUI[i].gameObject.SetActive(true);
                    }
                    break;
            }

        }

        SoundTime = 1f;
        startTime = 6;
        Playable = false;
        StartFlag = true;
        countDown.text = ("3");

        // リスポーン位置
        if (GameStart.gameMode1 == "Multi")
        {
            for (int i = 0; i < GameStart.MaxPlayer; i++)
            {
                defaultPlayerPos[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
                respownPos[i] = defaultPlayerPos[i].gameObject.transform.position;
                defaultPlayerPos[i].gameObject.SetActive(false);
            }
        }
        else 
        {
            defaultPlayerPos[0] = GameObject.Find("DefaultPlayerPos1");
            respownPos[0] = defaultPlayerPos[0].gameObject.transform.position;
            defaultPlayerPos[0].gameObject.SetActive(false);
        }


        //プレイヤー人数の反映
        {
            int i = 0;
            for (; i < GameStart.PlayerNumber; i++)
            {
                nameTags[i].gameObject.SetActive(true);
                players[i].gameObject.SetActive(true);
                sticks[i].gameObject.SetActive(true);
                //初期位置
                players[i].gameObject.transform.position = respownPos[i];
                sticks[i].gameObject.transform.position = respownPos[i];
            }
            // プレイヤー人数の反映
            for (; i < GameStart.MaxPlayer; i++)
            {
                nameTags[i].gameObject.SetActive(false);
                players[i].gameObject.SetActive(false);
                sticks[i].gameObject.SetActive(false);
            }
        }

        GameStart.inDemoPlay = false;
        /*
        //アウトラインコンポーネント追加
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (obj.name.Contains("Wall") || obj.name.Contains("Flo") || obj.name.Contains("Thorn"))
            {
                obj.AddComponent<PutOutline>();
            }
        }
        */

    }
        void FixedUpdate()
        {
            NameTagPos();
        }

        void Update()
        {
            SwichUI();
            StartTimer();
        }


        void StartTimer()
        {
            //3・2・1カウントダウン　→　スタート
            if (StartFlag)
            {
                startTime -= Time.deltaTime;
                SoundTime -= Time.deltaTime;
                if (startTime > 3)
                {
                    countDown.text = null;
                    return;
                }
                else if (startTime > 2)
                {
                    CountDownGO.gameObject.SetActive(true);
                    countDown.text = ("3");
                }
                else if (startTime > 1)
                {
                    countDown.text = ("2");
                }
                else if (startTime > 0)
                {
                    countDown.text = ("1");
                }
                else if (startTime < 0 && startTime > -0.5f)
                {
                countDown.text = startText[Settings.languageNum];
                }
                else if (startTime < 0.9f)
                {
                    countDown.text = ("");
                    CountDownGO.gameObject.SetActive(false);
                    StartFlag = false;
                }
                if (SoundTime < 0)
                {
                    SoundEffect.soundTrigger[3] = 1;
                    SoundTime = 1;
                }
            }
            if (startTime < 0 && ButtonInGame.Paused != 1) //ゲーム開始
            {

                if (GameStart.gameMode2 == "Arcade" && playTime > 0)
                {
                    elapsedTime -= Time.deltaTime;
                }
                else if (GameStart.gameMode2 != "Arcade")
                {
                    elapsedTime += Time.deltaTime;
                }
                Playable = true;
                if(playTime > 99) 
                {
                    playTime = elapsedTime;
                    playTime = Mathf.Floor(playTime);
                }
                else 
                {
                    playTime = elapsedTime * 10;
                    playTime = Mathf.Floor(playTime) / 10;
                }
               
                playTimeTx.text = (playTime.ToString());
            }
        }

        void NameTagPos()
        {
            //ネームタグの位置
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                nameTagPos[i] = players[i].transform.position;
                nameTagPos[i].y += 0.5f;
                nameTags[i].transform.position = nameTagPos[i];
            }
        }

    void SwichUI()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え

        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            for (int i = 0; i < keyBoardMouseUI.Length; i++) { keyBoardMouseUI[i].gameObject.SetActive(true); }
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(false); }
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < keyBoardMouseUI.Length; i++) { keyBoardMouseUI[i].gameObject.SetActive(false); }
            for (int i = 0; i < controllerUI.Length; i++) { controllerUI[i].gameObject.SetActive(true); }      
        }
    }


}
