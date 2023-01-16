using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Goal : MonoBehaviour
{
    public static string[] goaledPlayer{ get; set;} = new string[GameStart.MaxPlayer];
    GameObject[] players  = new GameObject[GameStart.MaxPlayer];
    GameObject[] sticks   = new GameObject[GameStart.MaxPlayer];
    GameObject[] nameTags = new GameObject[GameStart.MaxPlayer];
    public Text[] resultText = new Text[GameStart.MaxPlayer];
    public static byte Goals = 0;
    public static float[] clearTime = new float[GameStart.MaxPlayer];
    public static bool Goaled;
    [SerializeField]
    GameObject ResultPanel;

    GameObject ResultPanelFront, InputField;

    void Start()
    {
        for (int i = 0; i < GameStart.MaxPlayer; i++) //����������
        {        
            nameTags[  i] = GameObject.Find("P" +      (i + 1).ToString() + "Text");
            players[   i] = GameObject.Find("Player" + (i + 1).ToString());
            sticks[    i] = GameObject.Find("Stick"  + (i + 1).ToString());
            clearTime[ i] = 0;
            resultText[i].text = null;
            goaledPlayer[i] = null;
        }
    
        ResultPanel = GameObject.Find("ResultPanel");
        ResultPanelFront = GameObject.Find("ResultPanelFront");
        InputField = GameObject.Find("InputField");
        ResultPanel.gameObject.SetActive(false);
        ResultPanelFront.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);
        Goaled = false;
        Goals = 0;
    }
  
    void Update()
    {
        if (Goals == GameStart.PlayerNumber)
        {
            Goaled = true;
            GameSetting.Playable = false;
            ResultPanel.gameObject.SetActive(true);
            ResultPanelFront.gameObject.SetActive(true);
            InputField.gameObject.SetActive(true);
            // �Q���v���C���[�����^�C���\��
            for (int i = 0; i < GameStart.PlayerNumber; i++)
            {
                resultText[i].text = (i + 1).ToString() + "��: " + goaledPlayer[i] + " �^�C��:" + clearTime[i] + "�b";
            }    
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //�ڐG�����I�u�W�F�N�g�̃^�O��"Player"�̂Ƃ�
        if (other.CompareTag("Player"))
        {
            // �S�[�������v���C���[��\������
            clearTime[Goals] = GameSetting.PlayTime;
            goaledPlayer[Goals] = other.gameObject.name;
            SoundEffect.PironTrigger = 1;
            Goals++;
            int num = int.Parse(other.gameObject.name.Substring(6)) - 1;
            players[num].gameObject.SetActive(false);
            sticks[num].gameObject.SetActive(false);
            nameTags[num].gameObject.SetActive(false);
        }
    }
}


