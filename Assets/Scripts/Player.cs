using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class Player
{
    public InputDevice Device;
    public int PlayerIndex;
    public PlayerType Role;

    public Player(InputDevice device, int index, PlayerType role)
    {
        Device = device;
        PlayerIndex = index;
        Role = role;
    }
}
