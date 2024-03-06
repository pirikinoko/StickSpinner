using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Utility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameObject FindObjectWithContainingName(string name) 
    {
        GameObject[] target = GameObject.FindObjectsOfType<GameObject>()
       .Where(go => go.name.Contains(name))
       .ToArray();
        return target[0];
    }
}
