using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TextActive : NetworkBehaviour
{
    private SessionManager _sessionManager;

    Text text;
    Color textColor;

    // Start is called before the first frame update
    void Start()
    {
        _sessionManager = SessionManager.Instance;
        text = this.gameObject.GetComponent<Text>();
        textColor = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            float newAlpha = 1f;
            if (Runner.LocalPlayer.PlayerId != _sessionManager.LeaderId)
            {
                newAlpha = 0.3f;
            }
            textColor.a = newAlpha;
            text.color = textColor;
        }
    }
}
