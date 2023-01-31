using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    public GameObject SettingPanel;
    AudioSource BGMSource;
    public static float BGMStage = 10;
    public Text BGMText;
    // Start is called before the first frame update
    void Start()
    {
        SettingPanel.gameObject.SetActive(false);
        BGMSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //上限下限の設定
        if (BGMStage > 20)
        {
            BGMStage = 20;
        }
        if (BGMStage < 0)
        {
            BGMStage = 0;
        }
        SetBGMVol();
    }
    void SetBGMVol()
    {
        BGMSource.volume = BGMStage / 80;
        BGMText.text = BGMStage.ToString();
    }
}
