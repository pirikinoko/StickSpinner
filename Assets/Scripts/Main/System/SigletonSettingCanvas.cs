using UnityEngine;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;

public class SingletonSettingCanvas : MonoBehaviour
{
    // �ÓI�C���X�^���X��ێ�����ϐ�
    private static SingletonSettingCanvas? instance;

    // �V���O���g���̃C���X�^���X�ւ̃A�N�Z�X�v���p�e�B
    public static SingletonSettingCanvas? Instance
    {
        get
        {
            if (instance == null)
            {
                // �V�[�����ɑ��݂���C���X�^���X������
                instance = FindObjectOfType<SingletonSettingCanvas>();

                if (instance == null)
                {
                    // �C���X�^���X���Ȃ��ꍇ�͐V�����쐬             
                    GameObject singletonObject = (GameObject)Instantiate(Resources.Load("SettingCanvas"));
                    instance = singletonObject.GetComponent<SingletonSettingCanvas>();
                }
            }
            return instance;
        }
    }

    // Awake���\�b�h�ŃV���O���g����ݒ�
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
