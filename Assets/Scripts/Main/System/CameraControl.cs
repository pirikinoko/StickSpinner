using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraControl : MonoBehaviour
{
    GameSetting gameSetting;
    GameObject goalFlag;
    Vector2[] playerPos = new Vector2[4];
    public float minCameraSize = 3.5f;
    public float maxCameraSize = 5.5f;
    float cameraSpeed = 0.02f, cameraSpeedStart, cameraSize, cameraSizeStart;
    private Camera mainCamera;
    Vector3 cameraPos, vectorZero = new Vector3(0, 0, 0);
    int goals = 0, playerAlive, spectateTarget;
    bool[] isGoaled = new bool[4];
    bool chasePlayers = false;
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
        cameraPos = vectorZero;
        cameraPos.z = -10;


    }

    private void Update()
    {
        if (!GameSetting.allJoin)
        {
            return;
        }
        if (GameStart.gameMode1 == "Multi" || (GameStart.gameMode1 == "Single" && GameStart.gameMode2 == "Nomal"))
        {
            if (goalFlag == null)
            {
                goalFlag = GameObject.Find("GoalFlag");
                cameraPos = goalFlag.transform.position;


                cameraPos.z = -10;
                transform.position = cameraPos;
                cameraSizeStart = 1;
                cameraSpeedStart = 0.005f;
                chasePlayers = false;
                mainCamera.orthographicSize = cameraSizeStart;
            }
        }

        Vector3 centerPoint = Vector3.zero;
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

        //一人プレイの時
        if (playerAlive == 1 || NetWorkMain.isOnline)
        {
            if (GameSetting.startTime < 3)
            {
                mainCamera.orthographicSize = 3.0f;
            }

            if (NetWorkMain.isOnline)
            {
                if (!GameMode.Goaled) 
                {
                    if (gameSetting.players[NetWorkMain.netWorkId - 1] != null && gameSetting.players[NetWorkMain.netWorkId - 1].activeSelf)
                    {
                        centerPoint = gameSetting.players[NetWorkMain.netWorkId - 1].transform.position;
                    }
                }
                else 
                {
                    OnlineSpectate();
                    centerPoint = gameSetting.players[spectateTarget - 1].transform.position;
                }
               
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (gameSetting.players[i].activeSelf)
                    {
                        centerPoint = gameSetting.players[i].transform.position;
                    }
                }

            }
        }

        // 複数人プレイの時は最も離れている二点(X,Y)の中点にカメラ
        float maxDistance = 0f;
        float maxDistanceX = 0f;
        float maxDistanceY = 0f;
        bool yIsZero = true;
        if (!NetWorkMain.isOnline) 
        {
            for (int i = 0; i < GameStart.PlayerNumber - 1; i++)
            {
                playerPos[i] = gameSetting.players[i].transform.position;
                if (isGoaled[i]) { continue; }
                for (int j = i + 1; j < GameStart.PlayerNumber; j++)
                {
                    playerPos[j] = gameSetting.players[j].transform.position;

                    if (isGoaled[j]) { continue; }
                    float distance = Vector3.Distance(gameSetting.players[i].transform.position, gameSetting.players[j].transform.position);
                    float distanceX = (float)Math.Sqrt(Math.Pow(playerPos[i].x - playerPos[j].x, 2));
                    float distanceY = (float)Math.Sqrt(Math.Pow(playerPos[i].y - playerPos[j].y, 2));
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
                    else if (distanceY == 0 && yIsZero) { centerPoint.y = (playerPos[i].y + playerPos[j].y) / 2; } //スタート時はdistanceYが0のための処理

                }
            }
        }
         

        // カメラの拡大率をプレイヤー同士の距離に応じて変更
        cameraSize = maxDistance / 1.3f;
        cameraSize = System.Math.Min(cameraSize, maxCameraSize);
        cameraSize = System.Math.Max(cameraSize, minCameraSize);

        if (chasePlayers == false)
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
                    chasePlayers = true;
                }
            }
            return;
        }


        // カメラ移動 
        mainCamera.orthographicSize = cameraSize;
        cameraPos = Vector3.Lerp(transform.position, centerPoint, cameraSpeed);
        cameraPos.z = -10;
        transform.position = cameraPos;

        GoaledPlayersPos();
    }

    //ゴールしたプレイヤーをカメラの対象から外す
    void GoaledPlayersPos()
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