using UnityEngine;

public record Constants
{
    public static readonly string[] PlayerNameByLanguage = { "プレイヤー", "Player" };
    public static readonly string[] TextOnStart = { "スタート", "Start" };
    public static readonly string[] TextOnWaiting = { "プレイヤーを待っています", "Waiting For Players" };
    public static readonly string[] TextOnGameStart = { "ゲームスタート", "Game Start" };
    public static readonly int MinFlagTime = 60;
    public static readonly int MaxFlagTime = 120;
    public static readonly int MaxStageNomal = 7;
    public static readonly float ReInputCooldown = 0.3f;
    public static readonly string[] SingleArcadeText = { "無限の塔", "InfinityTower" };
    public static readonly string[] MultiArcadeText = { "旗取りバトル", "FlagBattle", "サッカー", "FootBall" };
    public static readonly Vector3[] FirstSlotPosition = new Vector3[4];

    public static readonly float PointInterval = 1.0f;
    public static readonly float KillPoint = 5.0f;
    public static readonly float KillLogDuration = 5.0f;
    public static readonly float IconOffsetX = 0.2f;
    public static readonly float IconOffsetY = 0.35f;
    public static readonly float CheckPosOffsetX = 0.5f;
    public static readonly float KillTimerDuration = 3.0f;
}
