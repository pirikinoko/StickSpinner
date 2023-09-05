using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class GameStart : MonoBehaviour
{
    public const int MaxStage  = 4;     // 総ステージ数
    public const int MaxPlayer = 4;     // 総プレイヤー数
    const int KeyboardMode = 5;
    const int ControllerMode = 6;

    public GameObject mainTitle, startPanel, singleSelect, multiSelect, changePlayerNumber, stageSelect, stageInfo, selectGameMode, setArcadeGame;//, keyboardMouseUI1, keyboardMouseUI2;
    public GameObject[] controllerUI, playerIcon, playerSlot;
    //チーム選択
    public Vector2[] playerIconPos { get; set; } = new Vector2[4];
    public Vector2[] slot1Pos  = new Vector2[4];
    public int[] teamASlot { get; set; } = { 1, 0, 0};
    public int[] teamBSlot { get; set; } = { 1, 0, 0};
    public int[] teamCSlot { get; set; } = { 1, 0, 0};
    public int[] teamDSlot { get; set; } = { 1, 0, 0};
    public int[] playerTeam { get; set; } = { 0, 1, 2, 3}; // {p1, p2, p3, p4}が TeamA, TeamB, TeamC, TeamDにいることを示す。ex..a = 1, c =3
    public bool stageInfoActive { get; set; } = false;
    int lastPlayerNum;
    Button StartButton;
    public static bool inDemoPlay = false;
    public Text playerNumberText, stageNumberText, flagTimeLimitTx;
    public static int flagTimeLimit = 90;
    //動画
    public VideoPlayer stageVideo;
    public VideoClip[] singleStageVideo, singleArcadeVideo, multiStageVideo, multiArcadeVideo;
    //画像
    public Image stageImage;
    private Sprite imageSprite;
    //テキスト
    public Text difficultyText;
    public static string gameMode1 = "Single";
    public static string gameMode2 = "Nomal";
    public static int phase = 0;
    public static int PlayerNumber{get; set;} = 1;     // 参加プレイヤー数
    public static int Stage = 1;


    //ステージロール *使ってない
    public GameObject[] singleButtons, normalButtons, arcadeButtons;
    Vector2 singleButtonPos, normalButtonPos, arcadeButtonPos, lastSingleButtonPos, lastNormalButtonPos, lastArcadeButtonPos;
    float SingleButtonGap, NormalButtonGap, ArcadeButtonGap;
    public string rollDirection { get; set; } = "None";
    public bool inProgress { get; set; } = false;
    string targetString;
    private GameObject[] stageButtons;
    public int trigger { get; set; } = 0;
    public int clicks { get; set; } = 0;
    void Start()
    {
        inDemoPlay = false;
        rollDirection = "None";
        Stage = 1;
        PlayerNumber = 1;
        phase = 0;
        trigger = 0;
        clicks = 0;
        startPanel.gameObject.SetActive(false);
        for (int i = 0; i < playerIconPos.Length; i++)
        {
            playerIconPos[i] = slot1Pos[i];
        }
        //ボタン初期位置など設定
        singleButtonPos = singleButtons[0].transform.position;
        normalButtonPos = normalButtons[0].transform.position; 
        arcadeButtonPos = arcadeButtons[0].transform.position;
        SingleButtonGap = singleButtons[1].transform.position.x - singleButtonPos.x;
        NormalButtonGap = normalButtons[1].transform.position.x - normalButtonPos.x;
        ArcadeButtonGap = arcadeButtons[1].transform.position.x - arcadeButtonPos.x;
        lastSingleButtonPos = singleButtons[singleButtons.Length - 1].transform.position;
        lastNormalButtonPos = normalButtons[normalButtons.Length - 1].transform.position;
        lastArcadeButtonPos = arcadeButtons[arcadeButtons.Length - 1].transform.position;

    }
    void Update()
    {
        Debug.Log("Phase:" + phase + " gamemode1:" + gameMode1 + " gamemode2:" + gameMode2);
        //SwichUI();
        SwichStageMaterial();
        playerNumberText.text = PlayerNumber.ToString();
        if (rollDirection != "None") 
        {
            StartCoroutine("RollStage");
        }
        if(trigger == 1) 
        {
            ResetButtonPos();
            trigger = 0;
        }
        if (Settings.SettingPanelActive) 
        {
            DisablePanel();
            return; 
        }
        PhaseControll();
        //phase 0～3
        phase = System.Math.Min(phase, 4);
        phase = System.Math.Max(phase, 0);
    }

    
    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        
        switch (gameMode1)
        {
            case "Single":
                if(gameMode2 == "Nomal")
                {
                    stageNumberText.text = "Stage" + Stage.ToString();
                    imageSprite = Resources.Load<Sprite>("SingleNomal" + Stage + "Img");
                    difficultyText.text = "Easy";
                    //stageVideo.clip = singleStageVideo[Stage - 1];

                }
                else
                {
                    imageSprite = Resources.Load<Sprite>("SingleArcade" + Stage + "Img");
                    //stageVideo.clip = singleArcadeVideo[Stage - 1];
                }
                break;
            case "Multi":
                if(gameMode2 == "Nomal")
                {
                    stageNumberText.text = "Stage" + Stage.ToString();
                    imageSprite = Resources.Load<Sprite>("MultiNomal" + Stage + "Img");
                    //stageVideo.clip = multiStageVideo[Stage - 1];
                }
                else
                {
                    if(Stage < 3) { stageNumberText.text = "FlagMode" + Stage.ToString(); }
                    imageSprite = Resources.Load<Sprite>("MultiArcade" + Stage + "Img");
                    //stageVideo.clip = multiArcadeVideo[Stage - 1];
                    switch (Stage)
                    {
                        case 1:
                            difficultyText.text = "Easy";
                        break;

                        case 2:
                            difficultyText.text = "Nomal";
                        break;
                    }
                    

                }
                break;
               
        }
        stageImage.sprite = imageSprite;
    }

    void PhaseControll()　　　//タイトル画面のフェーズごとの処理
    {
        switch (gameMode1) 
        {
            case "Single":
                switch (phase)
                {
                    case 0:
                        DisablePanel();
                        GameStart.PlayerNumber = 1;
                        mainTitle.gameObject.SetActive(true);                  
                        break;
                    case 1:
                        DisablePanel();
                        selectGameMode.gameObject.SetActive(true);
                        break;
                    case 2:
                    DisablePanel();
                        stageSelect.gameObject.SetActive(true);
                        break;
                
                }
                break;
            case "Multi":
                switch (phase)
                {
                    case 0:
                        DisablePanel();
                        GameStart.PlayerNumber = 1;
                        mainTitle.gameObject.SetActive(true);
                        break;
                    case 1:
                        DisablePanel();
                        changePlayerNumber.gameObject.SetActive(true);
                        break;
                    case 2:
                        DisablePanel();
                        selectGameMode.gameObject.SetActive(true);
                        lastPlayerNum = PlayerNumber;
                        break;
                    case 3:
                        DisablePanel();
                        stageSelect.gameObject.SetActive(true);
                        break;
                        if(!(gameMode2 == "Arcade") ||  !(Stage < 3))
                        {
                            return;
                        }
                    case 4:
                        DisablePanel();
                        setArcadeGame.gameObject.SetActive(true);
                        flagTimeLimitTx.text = flagTimeLimit.ToString();
                        for (int i = 0; i < 4; i++)
                        {
                            playerIcon[i].gameObject.SetActive(false);
                            playerSlot[i].gameObject.SetActive(false);
                        }
                        for (int i = 0; i < PlayerNumber; i++)
                        {
                            playerIcon[i].gameObject.SetActive(true);
                            playerSlot[i].gameObject.SetActive(true);
                        }
                        Debug.Log("slot1Pos[1]: "+ slot1Pos[1] + "   player2IconPos:" + playerIconPos[1]); 
                        TeamSelect();

                        for (int i = 0; i < PlayerNumber; i++)
                        {
                            playerIcon[i].transform.position = playerIconPos[i];
                        }
                        break;
                }
                break;
        }
    
    }

    void TeamSelect()
    {
        for (int i = 0; i < PlayerNumber; i++)
        {
            for (int j = 0; j < PlayerNumber; j++)
            {
                if (playerTeam[i] == j)
                {
                    playerIconPos[i] = slot1Pos[j];
                    int count = 0;
                    for (int k = 0; k < PlayerNumber; k++)
                    {
                        if(k == i) { return; }
                        if (playerTeam[k] == playerTeam[i])
                        {
                            count++;
                            playerIconPos[i].x  = slot1Pos[playerTeam[i]].x + (1.5f * count);
                        }
                    }
                }
            }
           
        }
    }
    void DisablePanel()
    {
        stageSelect.gameObject.SetActive(false);
        stageInfo.gameObject.SetActive(false);
        startPanel.gameObject.SetActive(false);
        selectGameMode.gameObject.SetActive(false);
        mainTitle.gameObject.SetActive(false);
        changePlayerNumber.gameObject.SetActive(false);
        singleSelect.gameObject.SetActive(false);
        multiSelect.gameObject.SetActive(false);
        setArcadeGame.gameObject.SetActive(false);
        inDemoPlay = false;
    }
    /*
    void SwichUI()
    {
        //キーボードマウス用UIとコントローラー用UIの切り替え

        //キーボード,マウスのとき
        if (!(ControllerInput.usingController))
        {
            if (phase == 1)
            {
                keyboardMouseUI1.gameObject.SetActive(true);
            }
            else if (phase == 3)
            {
                keyboardMouseUI2.gameObject.SetActive(true);
            }
            for(int i = 0; i < 5; i++) { controllerUI[i].gameObject.SetActive(false); }
            
        }
        //コントローラーのとき
        else if (ControllerInput.usingController)
        {

            for (int i = 0; i < 5; i++) { controllerUI[i].gameObject.SetActive(false); }
            controllerUI[phase].gameObject.SetActive(true);
            controllerUI[4].gameObject.SetActive(true);
            keyboardMouseUI1.gameObject.SetActive(false);
            keyboardMouseUI2.gameObject.SetActive(false);
        }
    }
    */
    private IEnumerator RollStage()
    {
        if (inProgress) { yield break; }
        inProgress = true;
        float movingDistance = 4.0f, speed = 10.0f, sign = -1;
        if (rollDirection.Contains("Nomal")) { targetString = "TargetButtonNomal"; movingDistance = NormalButtonGap; }
        else if (rollDirection.Contains("Arcade")) { targetString = "TargetButtonArcade"; movingDistance = ArcadeButtonGap; }
        else if (rollDirection.Contains("Single")) { targetString = "TargetButtonSingle"; movingDistance = SingleButtonGap; }
        if (rollDirection.Contains("Right")) { speed *= sign; }
        stageButtons = FindObjectsWithName(rollDirection);
        Vector2 startPos = stageButtons[0].transform.position;
        Vector2 currentPos = stageButtons[0].transform.position;


        while ((currentPos.x > (startPos.x - movingDistance)) && (currentPos.x < (startPos.x + movingDistance)))
        {
                for (int i = 0; i < stageButtons.Length; i++)
                {
                    //移動
                    Vector2 targetPos = stageButtons[i].transform.position;
                    targetPos.x += speed * Time.deltaTime;
                    stageButtons[i].transform.position = targetPos;
                    //角度変更
                    /*
                    RectTransform rectTransform = stageButtons[i].GetComponent<RectTransform>();
                    float rotationY = rectTransform.localEulerAngles.y;
                    float rotationX = rectTransform.localEulerAngles.x;
                    rotationX += 0.05f;
                    rotationY += 0.2f;
                    rectTransform.localRotation = Quaternion.Euler(0f, rotationY, 0);
                    */

                }
            currentPos = stageButtons[0].transform.position;
            yield return new WaitForSecondsRealtime(0.0001f);

        }
        rollDirection = "None";
        inProgress = false;
        yield return null;
    }

    private GameObject[] FindObjectsWithName(string name)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> matchingObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(targetString))
            {
                matchingObjects.Add(obj);
            }
        }
        return matchingObjects.ToArray();
    }
    
    void ResetButtonPos() 
    {
        Vector2 tmp1 = singleButtonPos;
        Vector2 tmp2 = normalButtonPos;
        Vector2 tmp3 = arcadeButtonPos;
        for (int i = 0; i < singleButtons.Length; i++)
        {
            singleButtons[i].transform.position = singleButtonPos;
            singleButtonPos.x += SingleButtonGap;
        }
        for (int i = 0; i < normalButtons.Length; i++)
        {
            normalButtons[i].transform.position = normalButtonPos;
            normalButtonPos.x += NormalButtonGap;
        }
        for (int i = 0; i < arcadeButtons.Length; i++)
        {
            arcadeButtons[i].transform.position = arcadeButtonPos;
            arcadeButtonPos.x += ArcadeButtonGap;
        }
        singleButtonPos = tmp1;
        normalButtonPos = tmp2;
        arcadeButtonPos = tmp3;
    }
}
