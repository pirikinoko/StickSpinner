using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    [SerializeField]
    Button button;
    [HideInInspector] public SaveData data;     // json変換するデータのクラス
    string filepath;                            // jsonファイルのパス
    string fileName = "Data.json";              // jsonファイル名

    //-------------------------------------------------------------------
    // 開始時にファイルチェック、読み込み
    void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
        button.OnClickAsObservable()
        .Subscribe(_ => Save(data));
        // パス名取得
        filepath = Application.dataPath + "/" + fileName;

        data = new SaveData();

        // ファイルがないとき、ファイル作成
        if (!File.Exists(filepath))
        {
            Save(data);
        }

        // ファイルを読み込んでdataに格納
        data = Load(filepath);
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);                 // jsonとして変換
        Debug.Log(json);
        StreamWriter wr = new StreamWriter(filepath, false);    // ファイル書き込み指定
        wr.WriteLine(json);                                     // json変換した情報を書き込み
        wr.Close();                                             // ファイル閉じる
    }

    // jsonファイル読み込み
    SaveData Load(string path)
    {
        StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
        string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
        rd.Close();                                             // ファイル閉じる

        return JsonUtility.FromJson<SaveData>(json);            // jsonファイルを型に戻して返す
    }

    //-------------------------------------------------------------------
    // ゲーム終了時に保存
    void OnDestroy()
    {
        if (Instance == this)
        {
            Save(data);
        }
    }
}