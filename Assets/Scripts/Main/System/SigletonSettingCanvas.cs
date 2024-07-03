using UnityEngine;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;

public class SingletonSettingCanvas : MonoBehaviour
{
    // 静的インスタンスを保持する変数
    private static SingletonSettingCanvas? instance;

    // シングルトンのインスタンスへのアクセスプロパティ
    public static SingletonSettingCanvas? Instance
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
                    GameObject singletonObject = (GameObject)Instantiate(Resources.Load("SettingCanvas"));
                    instance = singletonObject.GetComponent<SingletonSettingCanvas>();
                }
            }
            return instance;
        }
    }

    // Awakeメソッドでシングルトンを設定
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
