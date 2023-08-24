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

    public GameObject mainTitle, startPanel, singleSelect, multiSelect, changePlayerNumber, stageSelect, stageInfo;//, stage1Scores, stage2Scores, stage3Scores, stage4Scores, keyboardMouseUI1, keyboardMouseUI2;
    public GameObject[] controllerUI;
    public bool stageInfoActive { get; set; } = false;
    Button StartButton;
    public static bool inDemoPlay = false;
    public Text playerNumberText;
    /*
    public SpriteRenderer StageTitle, StageDifficulity, StageDescription;
    public Sprite Stage1Title, Stage1Difficulity, Stage1Description;
    public Sprite Stage2Title, Stage2Difficulity, Stage2Description;
    public Sprite Stage3Title, Stage3Difficulity, Stage3Description;
    public Sprite Stage4Title, Stage4Difficulity, Stage4Description;
    [SerializeField] Image StageImage;
    [SerializeField] Sprite[] stageImages = new Sprite[4];

    //動画
    public VideoPlayer StageVideo;
    public VideoClip Stage1Video, Stage2Video, Stage3Video, Stage4Video;
    */
    public static string gameMode1 = "Single";
    public static string gameMode2 = "Nomal";
    public static int phase = 0;
    public static int PlayerNumber{get; set;} = 1;     // 参加プレイヤー数
    public static int Stage = 1;
    //ステージロール
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
        //SwichUI();
        //SwichStageMaterial();
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
        phase = System.Math.Min(phase, 2);
        phase = System.Math.Max(phase, 0);
    }

    /*
    void SwichStageMaterial() //選択ステージ毎に情報切り替え
    {
        switch (Stage)
        {
            case 1:
                StageTitle.sprite = Stage1Title;
                StageDifficulity.sprite = Stage1Difficulity;
                StageDescription.sprite = Stage1Description;
                DisableScores();
                stage1Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage1Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 2:
                StageTitle.sprite = Stage2Title;
                StageDifficulity.sprite = Stage2Difficulity;
                StageDescription.sprite = Stage2Description;
                DisableScores();
                stage2Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage2Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 3:
                StageTitle.sprite = Stage3Title;
                StageDifficulity.sprite = Stage3Difficulity;
                StageDescription.sprite = Stage3Description;
                DisableScores();
                stage3Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage3Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;

            case 4:
                StageTitle.sprite = Stage4Title;
                StageDifficulity.sprite = Stage4Difficulity;
                StageDescription.sprite = Stage4Description;
                DisableScores();
                stage4Scores.gameObject.SetActive(true);
                StageVideo.clip = Stage4Video;
                StageImage.sprite = stageImages[Stage - 1];
                break;
        }
    }
    
    void DisableScores()
    {
        stage1Scores.gameObject.SetActive(false);
        stage2Scores.gameObject.SetActive(false);
        stage3Scores.gameObject.SetActive(false);
        stage4Scores.gameObject.SetActive(false);
    }
    */
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
                        singleSelect.gameObject.SetActive(true);
                        if (stageInfoActive)
                        {
                            stageInfo.gameObject.SetActive(true);
                        }
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
                        multiSelect.gameObject.SetActive(true);
                        if (stageInfoActive)
                        {
                            stageInfo.gameObject.SetActive(true);
                        }
                        break;
                }
                break;
        }
    
    }

    void DisablePanel()
    {
        stageSelect.gameObject.SetActive(false);
        stageInfo.gameObject.SetActive(false);
        startPanel.gameObject.SetActive(false);
        mainTitle.gameObject.SetActive(false);
        changePlayerNumber.gameObject.SetActive(false);
        singleSelect.gameObject.SetActive(false);
        multiSelect.gameObject.SetActive(false);
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
