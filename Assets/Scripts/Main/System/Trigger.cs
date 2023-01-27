using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    float[] pointTimer = new float[4];
    public static float[,] killTimer = new float[4, 4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            pointTimer[i] = 0;
        }
        if(GameStart.Stage == 4) { KillTimer(); }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (this.gameObject.name.Contains("CheckPos"))
        {
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if (other.gameObject.name == "Player" + (i + 1).ToString())
                {
                    CheckPoint.respownPos[i] = other.gameObject.transform.position;
                }
            }
        }

        if (this.gameObject.name.Contains("Point"))
        {
            if (GameSetting.PlayTime > 0 && ButtonInGame.Paused != 1)
            {
                for (int i = 1; i < GameStart.PlayerNumber; i++)
                {
                    if (other.gameObject.name == "Player" + (i + 1).ToString() || other.gameObject.name == "Stick" + (i + 1).ToString())
                    {
                        pointTimer[i] += Time.deltaTime;
                        if (pointTimer[i] > 2)
                        {
                            SoundEffect.KinTrigger = 1;
                            GameMode.points[i] += 1;
                            GameMode.playParticle[i] = 1;
                            pointTimer[i] = 0;
                        }
                    }
                }

            }
        }

    }


    private void OnCollisionStay2D(Collision2D other)
    {
        if(GameStart.Stage == 4)
        {
            //ìGÇ…êGÇÍÇƒÇ©ÇÁå‹ïbä‘ÉLÉãîªíË
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
        //ìGÇ…êGÇÍÇƒÇ©ÇÁå‹ïbä‘ÉLÉãîªíË
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
    }
}
