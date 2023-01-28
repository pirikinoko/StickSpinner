using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageControl : MonoBehaviour
{
    GameObject[] stageObject = new GameObject[4];
    [SerializeField] GameObject[] battleModeUI = new GameObject[2];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++) //‰Šú‰»ˆ—
        {
            stageObject[i] = GameObject.Find("Stage" + (i + 1).ToString() + "Objects");
            if (GameStart.Stage == (i + 1)) { stageObject[i].gameObject.SetActive(true); }
            else { stageObject[i].gameObject.SetActive(false); }
        }

        if (GameStart.Stage == 4)
        {
            battleModeUI[0].gameObject.SetActive(true);
            battleModeUI[1].gameObject.SetActive(true);
        }
        else
        {
            battleModeUI[0].gameObject.SetActive(false);
            battleModeUI[1].gameObject.SetActive(false);
        }
    }
   

}
