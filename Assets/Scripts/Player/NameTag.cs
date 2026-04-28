using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    private int _id;
    private TextMeshProUGUI _text;
    private SessionManager _sessionManager;

    void Start()
    {
        _id = transform.parent.transform.parent.GetComponent<Body>().Id;
        _sessionManager = SessionManager.Instance;
        _text = GetComponent<TextMeshProUGUI>();
        SetName();
    }

    void SetName()
    {
        _text.text = _sessionManager.PlayingType == "Online"
            ? _sessionManager.PlayerNames[_id - 1]
            : Constants.PlayerNameByLanguage[Settings.languageNum] + _id.ToString();
    }
}
