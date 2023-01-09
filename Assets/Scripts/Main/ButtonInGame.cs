using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class ButtonInGame : MonoBehaviour
{
    public GameObject PausePanel, pauseButton, TLFrame;
    public static int Paused = 0;
    //animation
    [SerializeField]
    private Animator XButtonAnim;

    float holdTime = 0;
    float holdGoal = 0.9f;
    Controller controller;


    void Start()
    {
        Paused = 0;
    }
    void Update()
    {
        MainControll();
    }



    void MainControll()
    {
        for (int i = 0; i < GameStart.PlayerNumber; i++)
        {
            /*�R���g���[���[��X�{�^��*/
            if (controller.playerKey.back) //�����ꂽ�Ƃ�
            {

                if (Paused == 1)
                {
                    XButtonAnim.enabled = true;
                    XButtonAnim.SetTrigger("On");
                    controller.playerKey.back = false;
                }
            }


            if (controller.playerKey.backHold) //�����������Ƃ�
            {

                //�Q�[���X�^�[�g�̃{�^���̂ݒ������œ���
                holdTime += Time.deltaTime;
                if (holdTime > holdGoal)
                {
                    if (Paused == 1)
                    {
                        Paused = 0;
                        SoundEffect.PironTrigger = 1;
                        controller.playerKey.backHold = false;
                        BackToTitle();
                    }
                    else if (Goal.Goaled)
                    {
                        SetHighScore.ToSetHighscore();
                        controller.playerKey.backHold = false;
                        BackToTitle();
                    }
                }


            }
            if (controller.playerKey.nextHold == false) //�����ꂽ�Ƃ�
            {
                XButtonAnim.SetTrigger("Off");
                XButtonAnim.enabled = false;
                holdTime = 0;
            }
            /*/�R���g���[���[��X�{�^��*/

         
            /*���ڂ̃R���g���[���[��Start�{�^��*/
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || controller.playerKey.start)
            {
                if (GameSetting.StartTime < 0 && Paused == 0)
                {
                    Paused = 1;
                    GameSetting.Playable = false;
                    GameSetting.StartTime = 0;
                    pauseButton.gameObject.SetActive(false);
                    Settings.SettingPanelActive = true;
                    Settings.inSetting = true;
                }
                else if (Paused == 1)
                {
                    Paused = 0;
                    GameSetting.StartTime = -1;
                    GameSetting.Playable = true;
                    pauseButton.gameObject.SetActive(true);
                    Settings.SettingPanelActive = false;
                    Settings.inSetting = false;
                }
                controller.playerKey.start = false;
            }
            /*/���ڂ̃R���g���[���[��Start�{�^��*/

        }

    }

    public void BackToTitle()
    {
        SoundEffect.PironTrigger = 1;
        GameStart.phase = 0;
        GameStart.InSelectPN = false;
        GameSetting.Playable = false;
        GameStart.PlayerNumber = 1;
        Settings.SettingPanelActive = false;
        SceneManager.LoadScene("Title");
    }

}


    


