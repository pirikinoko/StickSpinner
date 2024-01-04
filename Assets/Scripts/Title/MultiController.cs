using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiController : MonoBehaviour
{
    public static int[] lobbyPlayerNumber;
    public Text[] lobbyPlayerNumberText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lobbyPlayerNumberText.Length; i++)
        {
            lobbyPlayerNumberText[i].text = lobbyPlayerNumber[i] + " /  4" ;
        }
    }
}
