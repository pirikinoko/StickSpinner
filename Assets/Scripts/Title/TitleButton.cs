
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleButton : MonoBehaviour
{

    //const int title = 0;
    //const int selectStage = 1;
    //const int selectPlayerNumber = 2;

    //const int stage1 = 1;
    //const int stage2 = 2;
    //const int stage3 = 3;
    const int stage4 = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;

    // �{�^���̒���������
    [SerializeField]
    private Animator YButtonAnim;


    void Update()
    {
        // �^�C�g����ʂ͎O�ɕ����ď�������
        switch (GameStart.phase)
        {
            // �^�C�g��
            case 0:
                Title();
                break;
            // �X�e�[�W�I��
            case 1:
                SelectStage();
                break;
            // �v���C���[������ & �{�^���������ςȂ��ŃQ�[���J�n
            case 2:
                HoldButtonGoToGame();
                break;
        }
    }

    //
    // �^�C�g��
    //
    void Title()
    {
        if(Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 1;
            if (GameStart.Stage == stage4){ GameStart.PlayerNumber = 2;}    // ����Ȃ񂾂낤? stage4 �͋����I�ɂQ�l�v���C�Ƃ����Ӗ�?
        }
	}

    //
    // �X�e�[�W�I��
    //
    void SelectStage()
    {
        // ����
        if(Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 2;
            if (GameStart.Stage == stage4){ GameStart.PlayerNumber = 2;}
        }
        // �߂�
        else if(Input.GetButtonDown("XBack_1"))
        {
            // �L�����Z������炷
            GameStart.phase = 0;
		}

        // LR �ŃX�e�[�W�I��
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.Stage++;
            if(GameStart.Stage > GameStart.MaxStage){ GameStart.Stage = GameStart.MaxStage - 1;}
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.Stage--;
            if(GameStart.Stage < 0){ GameStart.Stage = 0;}
        }
	}

    //
    // �v���C���[������ & �{�^���������ςȂ��ŃQ�[���J�n
    //
    void HoldButtonGoToGame()
    {
        // �{�^�����������u��
        if(Input.GetButtonDown("Next_1"))
        {
            YButtonAnim.enabled = true;
            YButtonAnim.SetTrigger("On");
        }
        // �{�^���������������Ƃ� -> ���[�^�[���オ�葱���ăX�e�[�W�J�n
        else if (Input.GetButton("Next_1"))
        {
            holdTime += Time.deltaTime;
            if (holdTime > holdGoal)
            {
                GameStart.InSelectPN = false;
                SoundEffect.PironTrigger = 1;
                SceneManager.LoadScene("Stage" + GameStart.Stage.ToString());
            }
        }
        // �{�^�����������
        else if (Input.GetButtonUp("Next_1"))
        {
            YButtonAnim.SetTrigger("Off");
            YButtonAnim.enabled = false;
            holdTime = 0;
		}
        // �߂�
        else if(Input.GetButtonDown("XBack_1"))
        {
            // �L�����Z������炷
            GameStart.phase = 1;
		}

        // LR �Ńv���C���[���I��
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.PlayerNumber++;
            if(GameStart.PlayerNumber > GameStart.MaxPlayer){ GameStart.PlayerNumber = GameStart.MaxPlayer - 1;}
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.PlayerNumber--;
            if(GameStart.PlayerNumber < 1){ GameStart.PlayerNumber = 1;}
        }
    }
}

