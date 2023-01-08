using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static Vector2[] checkPos; 
    void Start()
    {
        if(GameStart.Stage == 4)
        {
            checkPos[0] = new Vector2(-8, -2f);
            checkPos[1] = new Vector2(7.84f, -2f);
            checkPos[3] = new Vector2(-4.01f, -2f);
            checkPos[3] = new Vector2(3.84f, -2f);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        for(int i = 1; i < GameStart.PlayerNumber; i++)
        {
            if (other.gameObject.name == "Player" + (i + 1).ToString())
            {
                checkPos[i] = other.gameObject.transform.position;
            }
        }
    }
}
