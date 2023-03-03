using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl2 : MonoBehaviour
{
    public Transform[] players;
    public GameObject[] playersGO;

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

    private void LateUpdate()
    {
        Vector3 centerPoint = Vector3.zero;
        playerAlive = 0;
        for (int i = 0; i < 4; i++)
        {
            if (playersGO[i].activeSelf)
            {
                playerAlive++;
            }
        }
        if (playerAlive == 1)
        {
            mainCamera.orthographicSize = 3.0f;
            for (int i = 0; i < 4; i++)
            {
                if (playersGO[i].activeSelf)
                {
                    centerPoint = players[i].transform.position;
                }
            }
        }

        // �ł�����Ă���2�l�̋������v�Z
        float maxDistance = 0f;
       
        for (int i = 0; i < GameStart.PlayerNumber - 1; i++)
        {
            if (isGoaled[i]) { continue; }
            for (int j = i + 1; j < GameStart.PlayerNumber; j++)
            {
                if (isGoaled[j]) { continue; }
                float distance = Vector3.Distance(players[i].position, players[j].position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    centerPoint = (players[i].position + players[j].position) / 2f;
                }
            }
        }

        // �J�����̊g�嗦�������ɉ����ĕύX
        float cameraSize = maxDistance / 1.3f;
        cameraSize = System.Math.Min(cameraSize, 5.5f);
        cameraSize = System.Math.Max(cameraSize, 3.5f);
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