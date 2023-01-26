using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
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
        
    }
}
