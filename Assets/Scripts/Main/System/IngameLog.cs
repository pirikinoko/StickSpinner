using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameLog : MonoBehaviour
{
    [SerializeField]
    GameObject logs;

    public static void GenerateIngameLog(string text)
    {
        Transform logsTransform = GameObject.Find("LogHolder").transform;
        GameObject logObj = Instantiate((GameObject)Resources.Load("IngameLogFrame"), new Vector3(-6, -4, 0.0f), Quaternion.identity, logsTransform);
        GameObject textObj = logObj.transform.GetChild(0).gameObject;
        textObj.GetComponent<Text>().text = text;

        Debug.Log("Log:" + text);
    }
    public void GenerateIngameBanner(string text)
    {
        Transform logsTransform = GameObject.Find("LogHolder").transform;
        GameObject logObj = Instantiate((GameObject)Resources.Load("IngameLogBanner"), new Vector3(0, 5, 0.0f), Quaternion.identity, logsTransform);
        GameObject textObj = logObj.transform.GetChild(0).gameObject;
        textObj.GetComponent<Text>().text = text;

        Debug.Log("Log:" + text);
    }
}
