using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [SerializeField] Camera camera_;

    // �v���C���[
    GameObject[] players{ get; set;} = new GameObject[GameStart.MaxPlayer];

    // �z��̓��������m�ۂ̂���
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
        // �Q���v���C���[
        int i = 0;
        for(; i < GameStart.PlayerNumber; i++)
        {
            players[  i] = GameObject.Find("Player" + (i + 1).ToString());
            distance[ i] = 0;
            Xdistance[i] = 0;
            Ydistance[i] = 0;
            playerActive[i] = true;
        }
        // �s�Q���v���C���[
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
        // 1�l�v���C�̎��͑傫�߂̉�ʂ�
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
        // �ő�l�𒴂�����ő�l��n��
        CameraSize = System.Math.Min(CameraSize, 5.5f);
        // �ŏ��l�����������ŏ��l��n��
        CameraSize = System.Math.Max(CameraSize, 3.5f);
        //X,Y���̋����̍ő�l�𒲂ׂ�
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
       
        //�J�����ʒu�ړ�
        if(cameraPos.x < cameraPosGoal.x)//�J�������������ړI�n�ɋ߂Â���
        {
            cameraPos.x += 0.015f;
            if(cameraPosGoal.x - cameraPos.x > 1)
            {
                cameraPos.x += 0.1f; //�ړI�n����傫������Ă���ꍇ�̓J�����𑬂�������
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

    //�S�[�������v���C���[���J�����̑Ώۂ���O��
    void GoaledPlayersPos()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            if (GameMode.goaledPlayer[i] != null)
            {
                int num = int.Parse(GameMode.goaledPlayer[i].Substring(6)) - 1; // Player�̔ԍ����擾
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
