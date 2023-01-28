using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trigger : MonoBehaviour
{

    float   pointTimer;
    //public static float[,] killTimer = new float[4, 4];
    int playerId;                   // �v���C���[�ԍ�(1�`4)
    //int otherId = 0;                // ������������̃v���C���[(1�`4)

    void Start()
    {
        // �{�f�B���X�e�B�b�N��� ID �𓾂�
        Controller cnt = GetComponent<Controller>();
        if (cnt)
		{   // Controller.cs
            playerId = cnt.id;
		}
        else
        {   // Body.cs
            Body bdy = GetComponent<Body>();
            playerId = bdy.id;
		}
        pointTimer = 0;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //
        if (gameObject.name.Contains("CheckPos"))
        {
            GameSetting.respownPos[playerId] = other.gameObject.transform.position;
            Debug.Log("P" + playerId.ToString() + "��CheckPoint��ݒ�");
        }

        if (this.gameObject.name.Contains("Player"))
        {
            if (other.gameObject.name.Contains("Point"))
            {

                {
                    if (GameSetting.PlayTime > 0 && ButtonInGame.Paused != 1)
                    {
                        pointTimer += Time.deltaTime;
                        if (pointTimer > 2)
                        {
                            SoundEffect.KinTrigger = 1;
                            GameMode.points[playerId] += 1;
                            GameMode.playParticle[playerId] = 1;
                            pointTimer = 0;
                        }
                    }


                }

            }
        }
    }

    /*
    private void OnCollisionStay2D(Collision2D other)
    {
        if(GameStart.Stage == 4)
        {
            //�G�ɐG��Ă���ܕb�ԃL������
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (this.gameObject.name == "Player" + (i + 1).ToString() || this.gameObject.name == "Stick" + (i + 1).ToString())
                {
                    for (int j = 0; i < GameStart.PlayerNumber; i++)
                    {
                        if (other.gameObject.name == "Player" + (j + 1).ToString() || other.gameObject.name == "Stick" + (j + 1).ToString())
                        {
                            killTimer[i, j] = 5.0f;
                        }
                    }
                }
            }
        }      
    }

    void KillTimer()
    {
        //�G�ɐG��Ă���ܕb�ԃL������
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            for (int j = 0; i < GameStart.PlayerNumber; i++)
            {
                if (killTimer[i, j] > 0)
                {
                    killTimer[i, j] -= Time.deltaTime;
                }
            }

        }
    }*/
}
