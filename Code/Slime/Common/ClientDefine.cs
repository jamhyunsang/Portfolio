using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class ClientDefine
{
    public static readonly byte[] EncryptKey = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("YouHyunsang"));
    public static readonly byte[] EncryptIV = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes("JamHyunsang"));

    public static readonly string LANGUAGE_KEY = "Language";

    public static readonly float MinDistance = (10f / 25.4f) * Screen.dpi;

    // 발판 최대 개수
    public static readonly int FloorCount = 10;
}

public enum eUI
{
    Main,
    Popup,
    Effect,
    Loading,

    Max
}

public enum eScene
{
    Title,
    Lobby,
    Game
}

public enum eBattleModule
{
    Main
}


public enum eDirection
{
    None,
    Up,
    Right,
    Left
}

public enum  ePosition
{
    LeftDrop,
    Left,
    Mid,
    Right,
    RightDrop
}