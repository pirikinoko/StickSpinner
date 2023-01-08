
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    const int title = 0;
    const int selectStage = 1;
    const int selectPlayerNumber = 2;
    const int stage1 = 1;
    const int stage2 = 2;
    const int stage3 = 3;
    const int stage4 = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;
    // �{�^���̒���������                           
    [SerializeField]
    private Animator YButtonAnim;
    Controller controller;
    void Update()
    {
        TitleControll();  
    }
    void TitleControll()
    {
        /*���ڂ̃R���g���[���[��Y�{�^��*/
        if (controller.playerKey[0].next) //�����ꂽ�Ƃ�
        {
            switch (GameStart.phase)
            {
                case title:
                    GameStart.phase++;
                    if (GameStart.Stage == stage4)
                    {
                        GameStart.PlayerNumber = 2;
                    }
                    break;
                case selectStage:
                    GameStart.phase++;
                    if (GameStart.Stage == stage4)
                    {
                        GameStart.PlayerNumber = 2;
                    }
                    break;
                case selectPlayerNumber:
                    YButtonAnim.enabled = true;
                    YButtonAnim.SetTrigger("On");
                    break;
            }
            controller.playerKey[0].next = false;
        }
        if (controller.playerKey[0].nextHold) //�����������Ƃ�
        {
            if (GameStart.phase == selectPlayerNumber)
            {
                //�Q�[���X�^�[�g�̃{�^���̂ݒ������œ���
                holdTime += Time.deltaTime;
                if (holdTime > holdGoal)
                {
                    // Stage1 �` 3 �� Int�^�Ȃ̂ŁA���ł����Ǝv��
                    GameStart.InSelectPN = false;
                    SoundEffect.PironTrigger = 1;
                    SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
                }
            }
        }
        if (controller.playerKey[0].nextHold == false) //�����ꂽ�Ƃ�
        {
            YButtonAnim.SetTrigger("Off");
            YButtonAnim.enabled = false;
            holdTime = 0;
        }
        /*/���ڂ̃R���g���[���[��Y�{�^��*/

        /*���ڂ̃R���g���[���[��X�{�^��*/
        if (controller.playerKey[0].back) //�����ꂽ�Ƃ�
        {
            GameStart.phase--;
            controller.playerKey[0].back = false;
        }
        /*/���ڂ̃R���g���[���[��X�{�^��*/

        /*���ڂ̃R���g���[���[��R�{�^��*/
        if (controller.playerKey[0].plus)
        {
            switch (GameStart.phase)
            {
                case selectStage:
                    GameStart.Stage++;
                    break;
                case selectPlayerNumber:
                    GameStart.PlayerNumber++;
                    break;
            }
            controller.playerKey[0].plus = false;
        }

        /*/���ڂ̃R���g���[���[��R�{�^��*/

        /*���ڂ̃R���g���[���[��L�{�^��*/
        if (controller.playerKey[0].minus)
        {
            if (GameStart.Stage != stage4)
            {
                GameStart.PlayerNumber = 1;
            }
            switch (GameStart.phase)
            {
                case selectStage:
                    GameStart.Stage--;
                    break;
                case selectPlayerNumber:
                    bool bProcessed = false;
                    if (GameStart.Stage == stage4 && GameStart.PlayerNumber > 2)
                    {
                        bProcessed = true;
                    }
                    //�X�e�[�W4�ȊO
                    else if (GameStart.Stage != stage4 && GameStart.PlayerNumber > 1)
                    {
                        bProcessed = true;
                    }
                    if (bProcessed)
                    {
                        GameStart.PlayerNumber--;
                    }
                    break;
            }
            controller.playerKey[0].minus = false;
        }
        /*/���ڂ̃R���g���[���[��L�{�^��*/

        /*���ڂ̃R���g���[���[��Start�{�^��*/
        if (controller.playerKey[0].start)
        {
            //�ݒ��ʂ̕\��
            Settings.SettingPanelActive = !(Settings.SettingPanelActive);
            Settings.inSetting = !(Settings.inSetting);
            controller.playerKey[0].start = false;
        }
        /*/���ڂ̃R���g���[���[��Start�{�^��*/

    }
}

