using Fusion;
using R3;
using System.Linq;
using UnityEngine;

// セッションの情報を管理するクラス
[RequireComponent(typeof(NetworkRunner))]
public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance;

    public NetworkRunner NetworkRunner;

    // 共有用の変数(ホスト以外はreadOnly)

    [Networked]
    public TickTimer StartTimerNetworked { get; set; }
    private TickTimer _localStartTimer;

    [Networked]
    public string PlayingTypeNetworked { get; set; } = "Single";
    private string _localPlayingType = "Single";

    [Networked]
    public string GameModeNetworked { get; set; } = "Nomal";
    private string _localGameMode = "Nomal";

    [Networked]
    public int StageNetworked { get; set; } = 1;
    private int _localStage = 1;

    [Networked]
    public string TeamModeNetworked { get; set; } = "FreeForAll";
    private string _localTeamMode = "FreeForAll";

    [Networked]
    public int TimeLimitNetworked { get; set; } = 90;
    private int _localTimeLimit = 90;

    [Networked]
    public int LeaderIdNetworked { get; set; }
    private int _localLeaderId;

    [Networked, Capacity(4)]
    public NetworkArray<int> PlayerTeamNetworked { get; } = MakeInitializer(new int[] { 0, 1, 2, 3 });
    private int[] _localPlayerTeam = new int[4] { 0, 1, 2, 3 };

    [Networked, Capacity(4)]
    public NetworkArray<string> PlayerNamesNetworked { get; } = MakeInitializer(new string[] { "", "", "", "" });
    private string[] _localPlayerNames = new string[4] { "", "", "", "" };

    [Networked, Capacity(4)]
    public NetworkArray<int> WinningsNetworked { get; } = MakeInitializer(new int[] { 0, 0, 0, 0 });
    private int[] _localWinnings = new int[4] { 0, 0, 0, 0 };

    [Networked, Capacity(4)]
    public NetworkArray<bool> IsReadyToStartTheGameNetworked { get; } = MakeInitializer(new bool[] { false, false, false, false });
    private bool[] _localIsReadyToStartTheGame = new bool[4] { false, false, false, false };

    [Networked, Capacity(4)]
    public NetworkArray<bool> IsReadyToSwitchSceneNetworked { get; } = MakeInitializer(new bool[] { false, false, false, false });
    private bool[] _localIsReadyToSwitchScene = new bool[4] { false, false, false, false };

    [Networked, Capacity(4)]
    public NetworkArray<int> PointsNetworked { get; } = MakeInitializer(new int[] { 0, 0, 0, 0 });
    private int[] _localPoints = new int[4] { 0, 0, 0, 0 };

    // ラップするプロパティ
    public TickTimer StartTimer
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? StartTimerNetworked : _localStartTimer;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                StartTimerNetworked = value;
            }
            else
            {
                _localStartTimer = value;
            }
        }
    }

    public string PlayingType
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? PlayingTypeNetworked : _localPlayingType;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                PlayingTypeNetworked = value;
            }
            else
            {
                _localPlayingType = value;
            }
        }
    }

    public string GameMode
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? GameModeNetworked : _localGameMode;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                GameModeNetworked = value;
            }
            else
            {
                _localGameMode = value;
            }
        }
    }

    public int Stage
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? StageNetworked : _localStage;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                StageNetworked = value;
            }
            else
            {
                _localStage = value;
            }
        }
    }

    public string TeamMode
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? TeamModeNetworked : _localTeamMode;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                TeamModeNetworked = value;
            }
            else
            {
                _localTeamMode = value;
            }
        }
    }

    public int TimeLimit
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? TimeLimitNetworked : _localTimeLimit;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                TimeLimitNetworked = value;
            }
            else
            {
                _localTimeLimit = value;
            }
        }
    }

    public int LeaderId
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? LeaderIdNetworked : _localLeaderId;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                LeaderIdNetworked = value;
            }
            else
            {
                _localLeaderId = value;
            }
        }
    }

    public int[] PlayerTeam
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? PlayerTeamNetworked.ToArray() : _localPlayerTeam;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < PlayerTeamNetworked.Length; i++)
                {
                    PlayerTeamNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localPlayerTeam = value;
            }
        }
    }

    public string[] PlayerNames
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? PlayerNamesNetworked.ToArray() : _localPlayerNames;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < PlayerNamesNetworked.Length; i++)
                {
                    PlayerNamesNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localPlayerNames = value;
            }
        }
    }

    public int[] Winnings
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? WinningsNetworked.ToArray() : _localWinnings;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < WinningsNetworked.Length; i++)
                {
                    WinningsNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localWinnings = value;
            }
        }
    }

    public bool[] IsReadyToStartTheGame
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? IsReadyToStartTheGameNetworked.ToArray() : _localIsReadyToStartTheGame;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < IsReadyToStartTheGameNetworked.Length; i++)
                {
                    IsReadyToStartTheGameNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localIsReadyToStartTheGame = value;
            }
        }
    }

    public bool[] IsReadyToSwitchScene
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? IsReadyToSwitchSceneNetworked.ToArray() : _localIsReadyToSwitchScene;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < IsReadyToSwitchSceneNetworked.Length; i++)
                {
                    IsReadyToSwitchSceneNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localIsReadyToSwitchScene = value;
            }
        }
    }

    public int[] Points
    {
        get => NetworkRunner != null && NetworkRunner.IsRunning ? PointsNetworked.ToArray() : _localPoints;
        set
        {
            if (NetworkRunner != null && NetworkRunner.IsRunning)
            {
                for (int i = 0; i < PointsNetworked.Length; i++)
                {
                    PointsNetworked.Set(i, value[i]);
                }
            }
            else
            {
                _localPoints = value;
            }
        }
    }

    // ホストが書き換え，サーバーに送信される更新用の変数
    public TickTimer StartTimerToSend { get; set; }
    public string PlayingTypeToSend { get; set; } = "Single";
    public string GameModeToSend { get; set; } = "Nomal";
    public int StageToSend { get; set; } = 1;
    public string TeamModeToSend { get; set; } = "FreeForAll";
    public int TimeLimitToSend { get; set; } = 90;
    public int LeaderIdToSend { get; set; }

    public int[] PlayerTeamToSend { get; set; } = new int[4] { 0, 1, 2, 3 };
    public string[] PlayerNamesToSend { get; set; } = new string[4] { "", "", "", "" };
    public int[] WinningsToSend { get; set; } = new int[4] { 0, 0, 0, 0 };
    public bool[] IsReadyToStartTheGameToSend { get; set; } = new bool[4] { false, false, false, false };
    public bool[] IsReadyToSwitchSceneToSend { get; set; } = new bool[4] { false, false, false, false };
    public int[] PointsToSend { get; set; } = new int[4] { 0, 0, 0, 0 };


    // Events

    public Observable<bool> OnPlayerCountChange => _onPlayerCountChange;

    private Subject<bool> _onPlayerCountChange = new();

    private int _lastFramePlayerCount;

    public Observable<int> OnLeaderIdChanged => _onLeaderIdChanged;

    private Subject<int> _onLeaderIdChanged = new();

    private int _lastFrameLeaderId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        NetworkRunner = GetComponent<NetworkRunner>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Leaderのプレイヤーがホストになる
        OnLeaderIdChanged.Subscribe(LeaderId => NetworkRunner.SetPlayerObject(NetworkRunner.ActivePlayers.ElementAtOrDefault(LeaderId), Object));
    }

    private void Update()
    {
        if (PlayingType == "Online" && NetworkRunner.IsInSession)
        {
            ControlOnPlayerChangeEvent();
            ControlOnLeaderIdChangeEvent();
        }
    }

    private void ControlOnPlayerChangeEvent()
    {
        GameStartHandler.PlayerCount = NetworkRunner.ActivePlayers.Count();

        if (_lastFramePlayerCount < GameStartHandler.PlayerCount)
        {
            _onPlayerCountChange.OnNext(true);
        }
        else if (_lastFramePlayerCount > GameStartHandler.PlayerCount)
        {
            _onPlayerCountChange.OnNext(false);
        }

        _lastFramePlayerCount = GameStartHandler.PlayerCount;
    }

    private void ControlOnLeaderIdChangeEvent()
    {
        if (LeaderId != _lastFrameLeaderId)
        {
            _onLeaderIdChanged.OnNext(LeaderId);
        }
        _lastFrameLeaderId = LeaderId;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // 送信用の変数で[Networked]プロパティを書き換える
            StartTimerNetworked = StartTimerToSend;
            PlayingTypeNetworked = PlayingTypeToSend;
            GameModeNetworked = GameModeToSend;
            StageNetworked = StageToSend;
            TeamModeNetworked = TeamModeToSend;
            TimeLimitNetworked = TimeLimitToSend;
            LeaderIdNetworked = LeaderIdToSend;

            for (int i = 0; i < PlayerTeamNetworked.Length; i++)
            {
                PlayerTeamNetworked.Set(i, PlayerTeamToSend[i]);
            }

            for (int i = 0; i < PlayerNamesNetworked.Length; i++)
            {
                PlayerNamesNetworked.Set(i, PlayerNamesToSend[i]);
            }

            for (int i = 0; i < WinningsNetworked.Length; i++)
            {
                WinningsNetworked.Set(i, WinningsToSend[i]);
            }

            for (int i = 0; i < IsReadyToStartTheGameNetworked.Length; i++)
            {
                IsReadyToStartTheGameNetworked.Set(i, IsReadyToStartTheGameToSend[i]);
            }

            for (int i = 0; i < IsReadyToSwitchSceneNetworked.Length; i++)
            {
                IsReadyToSwitchSceneNetworked.Set(i, IsReadyToSwitchSceneToSend[i]);
            }

            for (int i = 0; i < PointsNetworked.Length; i++)
            {
                PointsNetworked.Set(i, PointsToSend[i]);
            }
        }
    }
}
