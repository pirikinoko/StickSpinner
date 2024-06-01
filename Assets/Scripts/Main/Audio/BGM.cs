using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BGM : MonoBehaviour //BGM音量調整スクリプト
{
    [SerializeField] int maxVolume, volumeReducer;
    public GameObject SettingPanel;
    public AudioClip titleBGM, gameBGM;
    AudioSource audioSource;
    public static float BGMStage = 10;
    public Text BGMText;
    // Start is called before the first frame update
    void Start()
    {
        //シーン遷移時に破棄しない
        DontDestroyOnLoad(this.gameObject);
        SettingPanel.gameObject.SetActive(false);
        //AudioSorceの取得
        audioSource = GetComponent<AudioSource>();
        //シーンによってのBGM切り替え
        string currentSceneName = SceneManager.GetActiveScene().name;
        audioSource.clip = (currentSceneName == "Title") ? titleBGM : gameBGM;
    }

    // Update is called once per frame
    void Update()
    {
        //BGMの範囲設定
        BGMStage = Mathf.Clamp(BGMStage, 0, maxVolume);
        SetBGMVol();
    }
    void SetBGMVol()
    {
        audioSource.volume = BGMStage / volumeReducer;
    }
}
