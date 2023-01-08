using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPoint : MonoBehaviour
{
    float[] pointTimer;
    void Start()
    {
        pointTimer = new float[4];
        for (int i = 0; i < 4; i++)
        {
            pointTimer[i] = 0;
        }       
    }
    void OnTriggerStay2D(Collider2D other)
    {   
        if(GameSetting.PlayTime > 0 && ButtonInGame.Paused != 1)
        {
            for (int i = 1; i < GameStart.PlayerNumber; i++)
            {
                if (other.gameObject.name == "Player" + (i + 1).ToString() || other.gameObject.name == "Stick" + (i + 1).ToString() )
                {
                    pointTimer[i] += Time.deltaTime;
                    if(pointTimer[i] > 2)
                    {
                        SoundEffect.KinTrigger = 1;
                        BattleMode.points[i] += 1;
                        BattleMode.playParticle[i] = 1;
                        pointTimer[i] = 0;
                    }
                }
            }
                          
        }         
    }
}
