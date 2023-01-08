using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSystem : MonoBehaviour
{
    public static float[,] killTimer = new float[4,4];

    void Update()
    {
        KillTimer();
    }
    void KillTimer()
    {
        //“G‚ÉG‚ê‚Ä‚©‚çŒÜ•bŠÔƒLƒ‹”»’è
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            for (int j = 0; i < GameStart.PlayerNumber; i++)
            {
                if(killTimer[i, j] > 0)
                {
                    killTimer[i, j] -= Time.deltaTime;
                }
            }

        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {

        //“G‚ÉG‚ê‚Ä‚©‚çŒÜ•bŠÔƒLƒ‹”»’è
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




