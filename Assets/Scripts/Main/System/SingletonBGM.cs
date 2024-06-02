using UnityEngine;

public class SingletonBGM : MonoBehaviour
{
    // �ÓI�C���X�^���X��ێ�����ϐ�
    private static SingletonBGM instance;

    // �V���O���g���̃C���X�^���X�ւ̃A�N�Z�X�v���p�e�B
    public static SingletonBGM Instance
    {
        get
        {
            if (instance == null)
            {
                // �V�[�����ɑ��݂���C���X�^���X������
                instance = FindObjectOfType<SingletonBGM>();

                if (instance == null)
                {
                    // �C���X�^���X���Ȃ��ꍇ�͐V�����쐬
                    GameObject singletonObject = new GameObject(typeof(SingletonBGM).Name);
                    instance = singletonObject.AddComponent<SingletonBGM>();
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
