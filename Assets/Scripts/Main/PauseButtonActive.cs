using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Settings.SettingPanelActive) 
        {
            GetComponent<ButtonClick>().enabled = false;
        }
        else 
        {
            GetComponent<ButtonClick>().enabled = true;
        }
    }
}
