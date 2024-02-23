    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ButtonInGame : MonoBehaviourPunCallbacks
{
    public static int Paused = 0;
    private GameSetting gameSetting = new GameSetting();
   
    void Start() 
    {
        Paused = 0;
    }
}

