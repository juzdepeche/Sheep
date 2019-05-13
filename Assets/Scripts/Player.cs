using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class Player
{
    public enum EControllerType
    {
        Controller,
        Keyboard1,
        Keyboard2
    }

    public InputDevice Device;
    public int PlayerIndex;
    public PlayerType Role;
    public bool RoleLocked = false;
    public EControllerType ControllerType;
    public string KeyboardType;
    public Player(InputDevice device, int index, PlayerType role, EControllerType controllerType)
    {
        Device = device;
        PlayerIndex = index;
        Role = role;
        ControllerType = controllerType;

        //to refactor
        if (controllerType == EControllerType.Keyboard1) KeyboardType = "Space";
        else if (controllerType == EControllerType.Keyboard2) KeyboardType = "Enter";
        else KeyboardType = null;
    }
}
