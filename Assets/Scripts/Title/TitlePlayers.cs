using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePlayers : MonoBehaviour
{
    public GameObject[] players = new GameObject[4];
    public GameObject[] sticks = new GameObject[4];
    public GameObject[] nameTags = new GameObject[4];
    public GameObject[] sensText = new GameObject[4];
    public GameObject[] UIkey;
    private Vector2[] nameTagPos;
    bool[] uIActive = new bool[4];
    public Text PNtext;
    private Vector2 P1textPos, P2textPos, P3textPos, P4textPos;
    private byte[] count = new byte[4];
    public Rigidbody2D[] stickrb;
    bool p2UIActive, p3UIActive, p4UIActive;


    // Start is called before the first frame update
    void Start()
    {
        count[0] = 0;
        count[1] = 0;
        count[2] = 0;
        count[3] = 0;

        for (int i = 0; i < 4; i++) //����������
        {
            players[i].gameObject.SetActive(true);
            sticks[i].gameObject.SetActive(true);
        }
        
    }

    void Update()
    {
        PNtext.text = (GameStart.PlayerNumber + "�l");
        PlayerActive();
        ShowUI();
    }
    void FixedUpdate()
    {
        NameTagPos();
    }
    void ShowUI()
    {
        //�v���C���[���ɂ����UI�̕\�����ύX


        if (GameStart.PlayerNumber >= 2)
        {
            uIActive[0]  = true;
        }
        if (GameStart.PlayerNumber >= 3)
        {
            uIActive[1] = true;
        }
        if (GameStart.PlayerNumber >= 4)
        {
            uIActive[2] = true;
        }

        for (int i = 1; i < GameStart.PlayerNumber; i++)
        {
            UIkey[i].gameObject.SetActive(uIActive[i - 1]);
            sensText[i].gameObject.SetActive(uIActive[i - 1]);
        }
    }
    void PlayerActive()
    {
        if (GameStart.InSelectPN)
        {
            //�v���C���[�l���̔��f
            for(int i = 0; i < GameStart.PlayerNumber; i++)
            {
                if(count[i] == 0)
                {
                    nameTags[i].gameObject.SetActive(true);
                    UIkey[i].gameObject.SetActive(true);
                    players[i].gameObject.transform.position = new Vector2(-2 + i * 2, 0);
                    sticks[i].gameObject.transform.position = new Vector2(-2 + i * 2, 0);
                    stickrb[i].velocity = new Vector2(0f, 0f);
                    count[i] = 1;
                }     
            }

            //�v���C���[�l���̔��f    @@ 4 ����n�߂�Ɓ@�z��O�ɂȂ�̂� 3����ɂ���(3�`0�̎l�񃋁[�v)
            for (int i = 3; i >= GameStart.PlayerNumber; i--)
            {
                if (count[i] == 1)
                {
                    nameTags[i].gameObject.SetActive(false);
                    UIkey[i].gameObject.SetActive(false);
                    players[i].gameObject.transform.position = new Vector2(-100 + i * 2, 0);
                    sticks[i].gameObject.transform.position = new Vector2(-100 + i * 2, 0);
                    count[i] = 0;
                }
            }
        }

        else
        {
            for (int i = 0; i < 4; i++)
            {
                stickrb[i].velocity = new Vector2(0f, 0f);
                nameTags[i].gameObject.SetActive(false);
                UIkey[i].gameObject.SetActive(false);
                players[i].gameObject.transform.position = new Vector2(-100 + i * 2, 0);
                sticks[i].gameObject.transform.position = new Vector2(-100 + i * 2, 0);
                count[i] = 0;
            }
        }
    }
    void NameTagPos()
    {
        //�l�[���^�O�̈ʒu

        // nameTagPos �� null �ɂȂ��Ă���

       /*for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            nameTagPos[i] = players[i].transform.position;
            nameTagPos[i].y += 0.5f;
            nameTags[i].transform.position = nameTagPos[i];
        }*/
    }
}
