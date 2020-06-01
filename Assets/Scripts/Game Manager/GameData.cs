using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Observable<PlayerController>
{
    public static readonly GameData Instance = new GameData();

    public static readonly string ON_ACTION_1 = "OnAction1";
    public static readonly string ON_START = "OnStart";
    public static readonly string ON_WOLF_BODY_EXIT = "OnWolfBodyExit";
}
