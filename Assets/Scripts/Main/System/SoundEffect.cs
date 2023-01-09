using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffect : MonoBehaviour
{
    public static byte BonTrigger, DyukushiTrigger, PironTrigger, BunTrigger, ChaTrigger, PowanTrigger, KinTrigger, KirarinTrigger;
    public AudioClip Bon, Dyukushi, Piron, Bun, Cha, Powan, Kin, Kirarin;
    public static float SEStage =  10;
    public Text SEText;
    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {   //音量ステージ上限下限の設定
        if (SEStage > 20)
        {
            SEStage = 20;
        }
        if (SEStage < 0)
        {
            SEStage = 0;
        }
        SetSEVol();

        if (BonTrigger == 1)
        {
            audioSource.PlayOneShot(Bon);
            BonTrigger = 0;
        }
        if (DyukushiTrigger == 1)
        {
            audioSource.PlayOneShot(Dyukushi);
            DyukushiTrigger = 0;
        }
        if (PironTrigger == 1)
        {
            audioSource.PlayOneShot(Piron);
            PironTrigger = 0;
        }
        if (BunTrigger == 1)
        {
            audioSource.PlayOneShot(Bun);
            BunTrigger = 0;
        }
        if (ChaTrigger == 1)
        {
            audioSource.PlayOneShot(Cha);
            ChaTrigger = 0;
        }
        if (PowanTrigger == 1)
        {
            audioSource.PlayOneShot(Powan);
            PowanTrigger = 0;
        }
        if (KinTrigger == 1)
        {
            audioSource.PlayOneShot(Kin);
            KinTrigger = 0;
        }
        if (KirarinTrigger == 1)
        {
            audioSource.PlayOneShot(Kirarin);
            KirarinTrigger = 0;
        }
    }

    void SetSEVol()
    {
        audioSource.volume = SEStage / 50;
        SEText.text = SEStage.ToString();
    }


}
