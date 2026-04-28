using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField]
    private SessionManager _sessionManager;

    [SerializeField] int _stage;
    [SerializeField] int _playerCounnt;
    [SerializeField] int _timeLimit;
    [SerializeField] int[] _teamSize;
    [SerializeField] string _playingType;
    [SerializeField] string _gameMode;
    [SerializeField] string _teamMode;

    void Awake()
    {
        _sessionManager.PlayingType = _playingType;
        _sessionManager.GameMode = _gameMode;
        _sessionManager.Stage = _stage;
        _sessionManager.TimeLimit = _timeLimit;
        GameStartHandler.PlayerCount = _playerCounnt;
        _sessionManager.TeamMode = _teamMode;
        GameStartHandler.TeamSize = _teamSize;
    }
}
