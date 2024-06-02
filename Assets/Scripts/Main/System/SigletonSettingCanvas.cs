using UnityEngine;

public class SingletonSettingCanvas : MonoBehaviour
{
    // 静的インスタンスを保持する変数
    private static SingletonSettingCanvas instance;

    // シングルトンのインスタンスへのアクセスプロパティ
    public static SingletonSettingCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                // シーン内に存在するインスタンスを検索
                instance = FindObjectOfType<SingletonSettingCanvas>();

                if (instance == null)
                {
                    // インスタンスがない場合は新しく作成
                    GameObject singletonObject = new GameObject(typeof(SingletonSettingCanvas).Name);
                    instance = singletonObject.AddComponent<SingletonSettingCanvas>();
                }
            }
            return instance;
        }
    }

    // Awakeメソッドでシングルトンを設定
    private void Awake()
    {
        if (instance == null)
        {
            // インスタンスがない場合はこのオブジェクトをインスタンスとして設定
            instance = this;
            // オブジェクトがシーンロードで破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでにインスタンスが存在する場合は、このオブジェクトを破棄
            Destroy(gameObject);
        }
    }
}
