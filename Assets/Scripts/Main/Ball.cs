using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Ball : MonoBehaviour
{
    GameMode gameMode;
    GameSetting gameSetting;
    int lastColId;
    // Start is called before the first frame update
    void Start()
    {
        
        if(GameStart.gameMode1 != "Online") 
        {
            this.GetComponent<PhotonRigidbody2DView>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMode == null) 
        {
            gameMode = GameObject.Find("Scripts").GetComponent<GameMode>();
        }
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
            if(GameStart.teamMode == "FFA") 
            {
                GameMode.points[1]++;
            }
            else 
            {
                GameMode.teamPoints[1]++;
            }
            SoundEffect.soundTrigger[2] = 1;
            StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
            PlayPaperCanon(1);
        }
        if (col.gameObject.name == "GoalZoneRight")
        {
            if (GameStart.teamMode == "FFA")
            {
                GameMode.points[0]++;
            }
            else
            {
                GameMode.teamPoints[0]++;
            }
            SoundEffect.soundTrigger[2] = 1;
            StartCoroutine(GameObject.Find("Scripts").GetComponent<GameMode>().BallReset(this.gameObject));
            PlayPaperCanon(0);
        }
    }

    void PlayPaperCanon(int goalTeam) 
    {
        gameSetting = GameObject.Find("Scripts").GetComponent<GameSetting>();
        //�p�[�e�B�N���Đ�
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            Vector2[] particlePos = new Vector2[4];
             particlePos[i] = gameSetting.players[i].gameObject.transform.position;
            if(GameStart.playerTeam[i] == goalTeam) 
            {
                GameObject particleObj = (GameObject)Resources.Load("PaperCanon2");
                Instantiate(particleObj, particlePos[i], Quaternion.identity); //�p�[�e�B�N���p�Q�[���I�u�W�F�N�g����
            }
        }
    }
}
