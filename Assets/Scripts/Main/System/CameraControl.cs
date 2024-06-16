using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraControl : MonoBehaviour
{
    GameSetting gameSetting;
    [SerializeField]
    GameObject spectateUI;
    GameObject goalFlag;
    Vector2[] playerPos = new Vector2[4];
    public float minCameraSize = 3.5f;
    public float maxCameraSize = 5.5f;
    float cameraSpeed = 0.02f, cameraSpeedStart, cameraSize, cameraSizeStart;
    private Camera mainCamera;
    Vector3 cameraPos, centerPoint;
    int goals = 0, playerAlive, spectateTarget;
    bool[] isGoaled = new bool[4];
    bool isFirstAnimationEnded = false;
    private void Start()
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        goalFlag = null;

        for (int i = 0; i < 4; i++)
        {
            isGoaled[i] = false;
        }
        spectateTarget = 1;
        goals = 0;
        mainCamera = GetComponent<Camera>();
        cameraPos = Vector2.zero;
        cameraPos.z = -10;
        cameraSpeedStart = 0.005f;

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("spectateTarget   " + spectateTarget + "PlayerNumber   " + GameStart.PlayerNumber);
        }

        if (!GameSetting.setupEnded)
        {
            return;
        }
        //セットアップ終了後旗にズームインする
        ZoomInToFlag();

        // Vector3 centerPoint = Vector3.zero;
        CountPlayersInTheScene();
        OnePlayerCamera();
        CalucuratePosAndSize();  
        MoveCamera();
        RejectGoaldPlayerFromTarget();
    }
    void CountPlayersInTheScene() 
    {
        playerAlive = 0;
        if (!NetWorkMain.isOnline)
        {
            for (int i = 0; i < 4; i++)
            {
                if (gameSetting.players[i].activeSelf)
                {
                    playerAlive++;
                }
            }
        }
    }
    void CalucuratePosAndSize() 
    {
        // 複数人プレイの時は最も離れている二点(X,Y)の中点にカメラ
        float maxDistance = 0f;
        float maxDistanceX = 0f;
        float maxDistanceY = 0f;
        bool yIsZero = true;
        if (!NetWorkMain.isOnline || (GameStart.gameMode1 == "Online" && GameStart.gameMode2 == "Arcade" && GameStart.stage == 2))
        {
            // i(プレイヤー１～３とj(プレイヤー2~4)のxとyの距離を見て一番遠いxとyを採用してカメラの位置とする)
            for (int i = 0; i < GameStart.PlayerNumber - 1; i++)
            {
                //プレイヤーが死亡中やゴール済みの場合は計算に入れない
                if (gameSetting.players[i] != null)
                {
                    playerPos[i] = gameSetting.players[i].transform.position;
                    if (isGoaled[i]) { continue; }
                    for (int j = i + 1; j < GameStart.PlayerNumber; j++)
                    {
                        //プレイヤーが死亡中やゴール済みの場合は計算に入れない
                        if (gameSetting.players[j] == null)
                        {
                            return;
                        }
                        playerPos[j] = gameSetting.players[j].transform.position;

                        if (isGoaled[j]) { continue; }
                        //単純にどのプレイヤー間が一番離れているか，カメラのサイズの調整に使用する
                        float distance = Vector3.Distance(gameSetting.players[i].transform.position, gameSetting.players[j].transform.position);
                        //一番離れているxとyをそれぞれ求めるためのものカメラの位置の計算に使用する
                        float distanceX = (float)Math.Sqrt(Math.Pow(playerPos[i].x - playerPos[j].x, 2));
                        float distanceY = (float)Math.Sqrt(Math.Pow(playerPos[i].y - playerPos[j].y, 2));

                        //サッカーボールもカメラに含める
                        if (GameStart.gameMode1 != "Single" && GameStart.gameMode2 == "Arcade" && GameStart.stage == 2)
                        {
                            GameObject ballObj = Utility.FindObjectWithContainingName("SoccerBall");


                            Vector2 ballPos = ballObj.transform.position;
                            float ballDistance = Vector3.Distance(gameSetting.players[i].transform.position, ballPos);
                            float ballDistanceX = (float)Math.Sqrt(Math.Pow(playerPos[i].x - ballPos.x, 2));
                            float ballDistanceY = (float)Math.Sqrt(Math.Pow(playerPos[i].y - ballPos.y, 2));

                            if (ballDistance > maxDistance)
                            {
                                maxDistance = ballDistance;
                            }
                            if (ballDistanceX > maxDistanceX)
                            {
                                maxDistanceX = ballDistanceX;
                                centerPoint.x = (playerPos[i].x + ballPos.x) / 2;
                            }
                            if (ballDistanceY > maxDistanceY)
                            {
                                maxDistanceY = ballDistanceY;
                                centerPoint.y = (playerPos[i].y + ballPos.y) / 2;
                                yIsZero = false;
                            }
                        }
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                        }
                        if (distanceX > maxDistanceX)
                        {
                            maxDistanceX = distanceX;
                            centerPoint.x = (playerPos[i].x + playerPos[j].x) / 2;
                        }
                        if (distanceY > maxDistanceY)
                        {
                            maxDistanceY = distanceY;
                            centerPoint.y = (playerPos[i].y + playerPos[j].y) / 2;
                            yIsZero = false;
                        }
                        //スタート時はdistanceYが0のための処理
                        else if (distanceY == 0 && yIsZero) { centerPoint.y = (playerPos[i].y + playerPos[j].y) / 2; }
                    }
                }
            }
        }
        // カメラの拡大率をプレイヤー同士の距離に応じて変更
        cameraSize = maxDistance / 1.3f;
        cameraSize = System.Math.Min(cameraSize, maxCameraSize);
        cameraSize = System.Math.Max(cameraSize, minCameraSize);
    }
    void ZoomInToFlag() 
    {
        //最初のカメラズームアウト演出
        if ((GameStart.gameMode1 == "Multi" && GameStart.stage == 1) || (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Nomal"))
        {
            if (goalFlag == null)
            {
                //カメラをゴールフラッグの位置まで移動しズームインする
                goalFlag = GameObject.Find("GoalFlag");
                cameraPos = goalFlag.transform.position;
                cameraPos.z = -10;
                transform.position = cameraPos;
                cameraSizeStart = 1;
                isFirstAnimationEnded = false;
                mainCamera.orthographicSize = cameraSizeStart;
            }
        }

    }
    void OnePlayerCamera() 
    {
        //一人しか未ゴールプレイヤーがいない，またはオンラインの時カメラは自分のみを写す
        if (playerAlive == 1 || NetWorkMain.isOnline)
        {
            if (GameSetting.startTime < 3)
            {
                mainCamera.orthographicSize = 3.0f;
            }


            if (NetWorkMain.isOnline)
            {
                //ゴールしていなければ自身の位置にカメラを
                if (!GameMode.Goaled)
                {
                    if (gameSetting.players[NetWorkMain.netWorkId - 1] != null && gameSetting.players[NetWorkMain.netWorkId - 1].activeSelf)
                    {
                        centerPoint = gameSetting.players[NetWorkMain.netWorkId - 1].transform.position;
                    }
                }
                //ゴール済みであれば観戦できるようにする
                if (gameSetting.players[NetWorkMain.netWorkId - 1].activeSelf == false)
                {
                    OnlineSpectate();
                    if (gameSetting.players[spectateTarget - 1] == true)
                    {
                        centerPoint = gameSetting.players[spectateTarget - 1].transform.position;
                    }
                    else
                    {
                        spectateTarget++;
                    }
                }

            }
        }
    }
    void MoveCamera() 
    {
        if (isFirstAnimationEnded == false)
        {
            if (GameSetting.startTime < 4.3)
            {
                cameraSizeStart += 1.5f * Time.deltaTime;
                cameraSizeStart = System.Math.Min(cameraSize, cameraSizeStart);
                mainCamera.orthographicSize = cameraSizeStart;
                cameraPos = Vector3.Lerp(transform.position, centerPoint, cameraSpeedStart * Time.deltaTime);
                cameraSpeedStart += 0.01f;
                cameraPos.z = -10;
                transform.position = cameraPos;
                if (GameSetting.startTime < 0)
                {
                    isFirstAnimationEnded = true;
                }
            }
            return;
        }
        else
        {
            // カメラ計算されたcenterPointの位置に反映する処理
            mainCamera.orthographicSize = cameraSize;
            cameraPos = Vector3.Lerp(transform.position, centerPoint, cameraSpeed);
            cameraPos.z = -10;
            transform.position = cameraPos;

        }
    }
    //ゴールしたプレイヤーをカメラの対象から外す
    void RejectGoaldPlayerFromTarget()
    {
        if (GameStart.gameMode2 == "Nomal" && !(GameMode.Goaled))
        {
            if (GameMode.goaledPlayer[goals] != null)
            {
                int playerid = int.Parse(GameMode.goaledPlayer[goals].Substring(6)); // Playerの番号を取得
                isGoaled[playerid - 1] = true;
                goals++;
            }
        }
    }

    void OnlineSpectate() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            spectateTarget++;
        }
        if(spectateTarget > GameStart.PlayerNumber) 
        {
            spectateTarget = 1;
        }
        if (isGoaled[spectateTarget - 1])
        {
            spectateTarget++;
        }
    }

}