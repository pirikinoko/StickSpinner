using UnityEngine;

public class SingletonSettingCanvas : MonoBehaviour
{
    // �ÓI�C���X�^���X��ێ�����ϐ�
    private static SingletonSettingCanvas instance;

    // �V���O���g���̃C���X�^���X�ւ̃A�N�Z�X�v���p�e�B
    public static SingletonSettingCanvas Instance
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
                    GameObject singletonObject = new GameObject(typeof(SingletonSettingCanvas).Name);
                    instance = singletonObject.AddComponent<SingletonSettingCanvas>();
                }
            }
            return instance;
        }
    }

    // Awake���\�b�h�ŃV���O���g����ݒ�
    private void Awake()
    {
        if (instance == null)
        {
            // �C���X�^���X���Ȃ��ꍇ�͂��̃I�u�W�F�N�g���C���X�^���X�Ƃ��Đݒ�
            instance = this;
            // �I�u�W�F�N�g���V�[�����[�h�Ŕj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���łɃC���X�^���X�����݂���ꍇ�́A���̃I�u�W�F�N�g��j��
            Destroy(gameObject);
        }
    }
}
