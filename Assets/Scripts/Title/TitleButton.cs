
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
    const int stage4 = 4, firstStage = 1, lastStage = 4;
    const float holdGoal = 0.85f;
    float holdTime = 0;
    int minPlayer = 1;
    int maxPlayer = 4;
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
            // �v���C���[������ 
            case 2:
                ChangePlayerNumber();
                break;
            // �{�^���������ςȂ��ŃQ�[���J�n
            case 3:
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
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.Stage--;
        }
        // �ő�l�𒴂�����ő�l��n��
        GameStart.Stage = System.Math.Min(GameStart.Stage, lastStage);
        // �ŏ��l�����������ŏ��l��n��
        if (GameStart.Stage == 4) { minPlayer = 2; }
        GameStart.Stage = System.Math.Max(GameStart.Stage, firstStage);
    }

    //
    // �v���C���[������ 
    //
    void ChangePlayerNumber()
    {
        // ����
        if (Input.GetButtonDown("Next_1"))
        {
            SoundEffect.PironTrigger = 1;
            GameStart.phase = 3;
        }
        // �߂�
        else if (Input.GetButtonDown("XBack_1"))
        {
            // �L�����Z������炷
            GameStart.phase = 0;
        }

        // LR �Ńv���C���[���I��
        if (Input.GetButtonDown("Plus_1"))
        {
            GameStart.PlayerNumber++;
        }
        else if (Input.GetButtonDown("Minus_1"))
        {
            GameStart.PlayerNumber--;
        }
        // �ő�l�𒴂�����ő�l��n��
        GameStart.PlayerNumber = System.Math.Min(GameStart.PlayerNumber, maxPlayer);
        // �ŏ��l�����������ŏ��l��n��
        if (GameStart.Stage == 4) { minPlayer = 2; } else { minPlayer = 1; }
        GameStart.PlayerNumber = System.Math.Max(GameStart.PlayerNumber, minPlayer);
    }
    //
    // �{�^���������ςȂ��ŃQ�[���J�n
    //
    void HoldButtonGoToGame()
    {
        // �{�^�����������u��
        if (Input.GetButtonDown("Next_1"))
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
                GameStart.inDemoPlay = false;
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
        else if (Input.GetButtonDown("XBack_1"))
        {
            // �L�����Z������炷
            GameStart.phase = 1;
        }

      
    }
}

