using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    GameMode gameMode;
    int lastColId;
    // Start is called before the first frame update
    void Start()
    {
        gameMode = GameObject.Find("Scripts").GetComponent<GameMode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D col) 
    {
        if (col.gameObject.CompareTag("Player")) 
        {
            lastColId = col.gameObject.GetComponent<Body>().id;
        }
        if (col.gameObject.CompareTag("Stick"))
        {
            lastColId = col.gameObject.GetComponent<Controller>().id;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
         if(col.gameObject.name == "GoalZoneLeft")
        {
            GameMode.teamPoints[0]++;
            SoundEffect.soundTrigger[2] = 1;
            StartCoroutine(gameMode.BallReset());
        }
        if (col.gameObject.name == "GoalZoneRight")
        {
            GameMode.teamPoints[1]++;
            SoundEffect.soundTrigger[2] = 1;
            StartCoroutine(gameMode.BallReset());
        }
    }
}
