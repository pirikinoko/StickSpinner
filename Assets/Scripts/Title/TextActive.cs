using UnityEngine;
using UnityEngine.UI;

public class TextActive : MonoBehaviour
{
    Text text;
    Color textColor;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
        textColor = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            float newAlpha = 1f;
            if (NetWorkMain.NetWorkId != NetWorkMain.leaderId)
            {
                newAlpha = 0.3f;
            }
            textColor.a = newAlpha;
            text.color = textColor;
        }
    }
}
