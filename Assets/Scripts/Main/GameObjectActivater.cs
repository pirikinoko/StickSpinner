using UnityEngine;

public class GameObjectActivater : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("GameObjectActivater Awake" + gameObject.name);
        this.gameObject.SetActive(true);
    }
}
