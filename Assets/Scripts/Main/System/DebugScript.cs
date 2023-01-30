using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField] int targetStage;
    [SerializeField] int playerNumber;
    // Start is called before the first frame update
    void Start()
    {
        GameStart.Stage = targetStage;
        GameStart.PlayerNumber = playerNumber;
    }

}
