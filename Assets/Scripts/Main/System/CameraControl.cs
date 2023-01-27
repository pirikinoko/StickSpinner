using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [SerializeField] Camera camera_;

    // プレイヤー
    GameObject[] players{ get; set;} = new GameObject[GameStart.MaxPlayer];

    // 配列はメモリを確保のこと
    bool[]    playerActive    = new bool[GameStart.MaxPlayer];
    Vector2[] playerTransform = new Vector2[GameStart.MaxPlayer];
    float[] distance  = new float[6];
    float[] Xdistance = new float[6];
    float[] Ydistance = new float[6];
    int[] memo1 = new int[6];
    int[] memo2 = new int[6];
    float maxDistansX, maxDistansY, maxDistans;
    float CameraSize;
    int   StartTrigger = 0;//, CameraTarget;
    //Vector2 DefaultCamaeraPos;

    void Start()
    {
        // 参加プレイヤー
        int i = 0;
        for(; i < GameStart.PlayerNumber; i++)
        {
            players[  i] = GameObject.Find("Player" + (i + 1).ToString());
            distance[ i] = 0;
            Xdistance[i] = 0;
            Ydistance[i] = 0;
            playerActive[i] = true;
        }
        // 不参加プレイヤー
        for(; i < GameStart.MaxPlayer; i++)
        {
            playerActive[i] = false;
		}

        StartTrigger = 0;   
        //CameraTarget = GameStart.PlayerNumber;
    }

    void Update()
    {
        GoaledPlayersPos();
        PlayerPos();
        GoaledPlayersPos();
        // 1人プレイの時は大きめの画面で
        if (GameStart.PlayerNumber != 1)
        {
            camera_.orthographicSize = CameraSize;
        }
        else
        {
            transform.position = new Vector3(playerTransform[0].x, playerTransform[0].y, -10);
            camera_.orthographicSize = 3.0f;
        }
    }

    void FixedUpdate()
    {
        if (GameStart.PlayerNumber != 1)
        {
            CameraPosition();
        }
    }


    public void CameraPosition()
    {
        Transform CameraPosGoalTransform = transform;
        Vector3   cameraPosGoal          = CameraPosGoalTransform.position;
        Transform CameraTransform        = transform;
        Vector3   cameraPos              = CameraTransform.position;
        int num = 0;
        for (int i = 0; (i + 1) < GameStart.PlayerNumber; i++)
        {
            for (int j = 0; (j + 1 + i) < GameStart.PlayerNumber; j++)
            {
                distance[num]  = (float)Math.Sqrt(Math.Pow(playerTransform[i].x - playerTransform[j + 1].x, 2) + Math.Pow(playerTransform[i].y - playerTransform[j + 1].y, 2));
                Xdistance[num] = (float)Math.Sqrt(Math.Pow(playerTransform[i].x - playerTransform[j + 1].x, 2));
                Ydistance[num] = (float)Math.Sqrt(Math.Pow(playerTransform[i].y - playerTransform[j + 1].y, 2));             
                memo1[num] = i;
                memo2[num] = j + 1;
                num++;
            }    
        }
        maxDistans = Mathf.Max(distance);
        maxDistansX = Mathf.Max(Xdistance);
        maxDistansY = Mathf.Max(Ydistance);
        CameraSize = maxDistans / 1.3f;
        // 最大値を超えたら最大値を渡す
        CameraSize = System.Math.Min(CameraSize, 5.5f);
        // 最小値を下回ったら最小値を渡す
        CameraSize = System.Math.Max(CameraSize, 3.5f);
        //X,Y軸の距離の最大値を調べる
        for (int i = 0; i < distance.Length; i++)
        {
            int num1 = memo1[i];
            int num2 = memo2[i];
            if (maxDistansX == Xdistance[i])
            {
                cameraPosGoal.x = (playerTransform[num1].x + playerTransform[num2].x) / 2;
            }
            if (maxDistansY == Ydistance[i])
            {
                cameraPosGoal.y = (playerTransform[num1].y + playerTransform[num2].y) / 2;
            }
        }
       
        //カメラ位置移動
        if(cameraPos.x < cameraPosGoal.x)//カメラをゆっくり目的地に近づける
        {
            cameraPos.x += 0.015f;
            if(cameraPosGoal.x - cameraPos.x > 1)
            {
                cameraPos.x += 0.1f; //目的地から大きく離れている場合はカメラを速く動かす
            }
        }
        if (cameraPos.x > cameraPosGoal.x)
        {
            cameraPos.x -= 0.015f;
            if (cameraPos.x - cameraPosGoal.x > 1)
            {
                cameraPos.x -= 0.1f;
            }
        }
        if (cameraPos.y < cameraPosGoal.y)
        {
            cameraPos.y += 0.015f;
            if (cameraPosGoal.y - cameraPos.y > 1)
            {
                cameraPos.y += 0.1f; 
            }
        }
        if (cameraPos.y > cameraPosGoal.y)
        {
            cameraPos.y -= 0.015f;
            if (cameraPos.y - cameraPosGoal.y > 1)
            {
                cameraPos.y -= 0.1f; 
            }
        }

        if (StartTrigger == 0)
        {
            cameraPos = cameraPosGoal;
            StartTrigger = 1;
        }
        transform.position = cameraPos;
    }

    //ゴールしたプレイヤーをカメラの対象から外す
    void GoaledPlayersPos()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (GameMode.goaledPlayer[i] != null)
            {
                int num = int.Parse(GameMode.goaledPlayer[i].Substring(6)) - 1; // Playerの番号を取得
                playerActive[num] = false;
                if(playerActive[i] == false)
                {
                    for(int j = 0; j < GameStart.PlayerNumber; j++)
                    {
                        if (playerActive[j])
                        {
                            players[i].transform.position = players[j].transform.position;
                        }                     
                    }
                    
                }
            }
        }
    }

    void PlayerPos()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            playerTransform[i] = players[i].transform.position;
        }
    }
}
