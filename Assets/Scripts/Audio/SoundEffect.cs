using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffect : MonoBehaviour　//効果音呼び出しスクリプト
{
    public static byte[] soundTrigger = new byte[8];
    public AudioClip[] sounds;
    public static float SEStage =  10;
    public Text SEText;
    AudioSource audioSource;
    float[] coolTime = new float[8];
    bool[] isReady = new bool[8];
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < coolTime.Length; i++)
        {
            coolTime[i] = 0.1f;
            isReady[i] = true; 
        }
    }
    void Update()
    {   
        //音量上限下限の設定
        if (SEStage > 20)
        {
            SEStage = 20;
        }
        if (SEStage < 0)
        {
            SEStage = 0;
        }
        SetSEVol();

        for (int i = 0; i < sounds.Length; i++)
        {
            if(soundTrigger[i] == 1) 
            {
                if (isReady[i]) 
                {
                    audioSource.PlayOneShot(sounds[i]);
                    soundTrigger[i] = 0;
                }            
                isReady[i] = false;
            }
        }
     

        for (int i = 0; i < coolTime.Length; i++)
        {
            if(isReady[i] == false) 
            {
                coolTime[i] -= Time.deltaTime;
            }
            if(coolTime[i] < 0) 
            {
                coolTime[i] = 0.2f;
                isReady[i] = true;
            }
        }
    }

    void SetSEVol()
    {
        audioSource.volume = SEStage / 170;
        SEText.text = SEStage.ToString();
    }


}
