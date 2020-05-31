using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDevicesData
{
    private static List<PlayerInput> players;

    public static List<PlayerInput> Players
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
