using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public static Vector2[] respownPos = new Vector2[4];
    GameObject[] defaultPlayerPos = new GameObject[4];
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            defaultPlayerPos[i] = GameObject.Find("DefaultPlayerPos" + (i + 1).ToString());
            respownPos[i] = defaultPlayerPos[i].gameObject.transform.position;
            defaultPlayerPos[i].gameObject.SetActive(false);
        }
    }
    
}
