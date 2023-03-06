using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraControl : MonoBehaviour
{
    public GameObject[] players;
    Vector2[] playerPos = new Vector2[4];
    public float minCameraSize = 3.5f;
    public float maxCameraSize = 5.5f;
    public float cameraMoveSpeed = 0.015f;

    private Camera mainCamera;
    Vector3 cameraPos;
    int goals = 0;
    bool[] isGoaled = new bool[4];
    int playerAlive;
    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            isGoaled[i] = false;
        }
        goals = 0;
        mainCamera = GetComponent<Camera>();
        cameraPos = this.transform.position;
    }

    private void Update()
    {
        Vector3 centerPoint = Vector3.zero;
        playerAlive = 0;
        for (int i = 0; i < 4; i++)
        {
            if (players[i].activeSelf)
            {
                playerAlive++;
            }
        }
        //��l�v���C�̎�
        if (playerAlive == 1)
        {
            mainCamera.orthographicSize = 3.0f;
            for (int i = 0; i < 4; i++)
            {
                if (players[i].activeSelf)
                {
                    centerPoint = players[i].transform.position;
                }
            }
        }

        // �����l�v���C�̎��͍ł�����Ă����_(X,Y)�̒��_�ɃJ����
        float maxDistance = 0f;
        float maxDistanceX = 0f;
        float maxDistanceY = 0f;
        for (int i = 0; i < GameStart.PlayerNumber - 1; i++)
        {
            playerPos[i] = players[i].transform.position;
            if (isGoaled[i]) { continue; }
            for (int j = i + 1; j < GameStart.PlayerNumber; j++)
            {
                playerPos[j] = players[j].transform.position;

                if (isGoaled[j]) { continue; }
                float distance = Vector3.Distance(players[i].transform.position, players[j].transform.position);
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
                }
                else if (distanceY == 0) { centerPoint.y = (playerPos[i].y + playerPos[j].y) / 2; }
            }
        }

        // �J�����̊g�嗦�������ɉ����ĕύX
        float cameraSize = maxDistance / 1.3f;
        cameraSize = System.Math.Min(cameraSize, maxCameraSize);
        cameraSize = System.Math.Max(cameraSize, minCameraSize);
        mainCamera.orthographicSize = cameraSize;

        // �J�����𒆓_�Ɉړ�
        cameraPos = Vector3.Lerp(transform.position, centerPoint, cameraMoveSpeed);
        cameraPos.z = -10;
        transform.position = cameraPos;

        GoaledPlayersPos();
    }

    //�S�[�������v���C���[���J�����̑Ώۂ���O��
    void GoaledPlayersPos()
    {
        if (!(GameMode.Goaled))
        {
            if (GameMode.goaledPlayer[goals] != null)
            {
                int playerid = int.Parse(GameMode.goaledPlayer[goals].Substring(6)); // Player�̔ԍ����擾
                isGoaled[playerid - 1] =  true;
                goals++;
            }
        }

    }

}