using UnityEngine;
using UnityEngine.UI;

public class ShowHighScore : MonoBehaviour
{
    private SessionManager _sessionManager;

    [SerializeField]
    Text highScoreText;
    SaveData data;
    public static int[] singleHighScore = new int[10], multiHighScore = new int[10], singleArcadeHighScore = new int[10], multiArcadeHighScore = new int[10];
    string[] text = { "ハイスコア", "HighScore" };
    string[] unit = { "秒", "Sec" };


    private void Start()
    {
        _sessionManager = SessionManager.Instance;

        data = DataManager.Instance.data;
        for (int i = 0; i < 10; i++)
        {
            if (singleHighScore[i] == 0)
            {
                singleHighScore[i] = data.singleHighScore[i];
            }
            if (multiHighScore[i] == 0)
            {
                multiHighScore[i] = data.multiHighScore[i];
            }
            if (singleArcadeHighScore[i] == 0)
            {
                singleArcadeHighScore[i] = data.singleArcadeHighScore[i];
            }
            if (multiArcadeHighScore[i] == 0)
            {
                multiArcadeHighScore[i] = data.multiArcadeHighScore[i];
            }
        }
    }
    private void Update()
    {
        if (_sessionManager.Stage < 1) { return; }
        highScoreText.text = _sessionManager.PlayingType == "Single"
            ? _sessionManager.GameMode == "Nomal"
                ? text[Settings.languageNum] + ":" + singleHighScore[_sessionManager.Stage - 1] + unit[Settings.languageNum]
                : text[Settings.languageNum] + ":" + singleArcadeHighScore[_sessionManager.Stage - 1] + "m"
            : _sessionManager.GameMode == "Nomal"
                ? text[Settings.languageNum] + ":" + multiHighScore[_sessionManager.Stage - 1] + unit[Settings.languageNum]
                : text[Settings.languageNum] + ":" + multiArcadeHighScore[_sessionManager.Stage - 1] + "P";
        for (int i = 0; i < 10; i++)
        {
            data.singleHighScore[i] = singleHighScore[i];
            data.singleArcadeHighScore[i] = singleArcadeHighScore[i];
            data.multiHighScore[i] = multiHighScore[i];
            data.multiArcadeHighScore[i] = multiArcadeHighScore[i];
        }
    }
}

