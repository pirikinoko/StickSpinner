using UnityEngine;

public class SingletonBGM : MonoBehaviour
{
    // 静的インスタンスを保持する変数
    private static SingletonBGM instance;

    // シングルトンのインスタンスへのアクセスプロパティ
    public static SingletonBGM Instance
    {
        get
        {
            if (instance == null)
            {
                // シーン内に存在するインスタンスを検索
                instance = FindObjectOfType<SingletonBGM>();

                if (instance == null)
                {
                    // インスタンスがない場合は新しく作成
                    GameObject singletonObject = new GameObject(typeof(SingletonBGM).Name);
                    instance = singletonObject.AddComponent<SingletonBGM>();
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
