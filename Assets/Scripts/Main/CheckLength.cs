using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int objNumber = 0;
        if (other.gameObject.name.Contains("Wall"))
        {
            objNumber += 4;
        }
        for (int i = 0; i < 4; i++)
        {
            if (other.gameObject.name.Contains((i + 1).ToString()))
            {
                objNumber += i;
            }
        }
        GenerateStage.collisionPos[objNumber] = this.gameObject.transform.position.x;
        Debug.Log(this.gameObject.transform.position.x);
    }
}
