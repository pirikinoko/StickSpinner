using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BGM : MonoBehaviour //BGM音量調整スクリプト
{
    [SerializeField] int maxVolume, volumeReducer;
    public AudioClip titleBGM, gameBGM;
    AudioSource audioSource;
    public static float BGMStage = 10;
    string currentSceneName;
    // Start is called before the first frame update
    void Start()
    {
        //AudioSorceの取得
        audioSource = GetComponent<AudioSource>();
        //シーンによってのBGM切り替え
        currentSceneName = SceneManager.GetActiveScene().name;
        audioSource.clip = (currentSceneName == "Title") ? titleBGM : gameBGM;
        audioSource.Play();
    }


    // Update is called once per frame
    void Update()
    {
        //BGMの範囲設定
        BGMStage = Mathf.Clamp(BGMStage, 0, maxVolume);
        SetBGMVol();
        if (currentSceneName != SceneManager.GetActiveScene().name)
        {
            //シーンによってのBGM切り替え
            currentSceneName = SceneManager.GetActiveScene().name;
            audioSource.clip = (currentSceneName == "Title") ? titleBGM : gameBGM;
            audioSource.Play();
        }
    }
    void SetBGMVol()
    {
        audioSource.volume = BGMStage / volumeReducer;
    }
}
