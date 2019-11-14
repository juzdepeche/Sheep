using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDevicesData
{
    private static List<Player> players;

    public static List<Player> Players
    {
        get
        {
            return players;
        }
        set
        {
            players = value;
        }
    }
}
