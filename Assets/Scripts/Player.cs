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

    public string InputAxeX;
    public string InputAxeY;

    public KeyCode Action1;
    public KeyCode Action2;
    public KeyCode Action3;
    public KeyCode Action4;

    public Player(InputDevice device, int index, PlayerType role, EControllerType controllerType)
    {
        Device = device;
        PlayerIndex = index;
        Role = role;
        ControllerType = controllerType;

        //to refactor
        if (controllerType == EControllerType.Keyboard1)
        {
            KeyboardType = "Space";
            InputAxeX = "Horizontal3";
            InputAxeY = "Vertical3";
            Action1 = KeyCode.Space;
            Action2 = KeyCode.LeftControl;
            Action3 = KeyCode.R;
            Action4 = KeyCode.E;
        }
        else if (controllerType == EControllerType.Keyboard2)
        {
            KeyboardType = "Enter";
            InputAxeX = "Horizontal2";
            InputAxeY = "Vertical2";
            Action1 = KeyCode.KeypadEnter;
            Action2 = KeyCode.Keypad0;
            Action3 = KeyCode.KeypadPlus;
            Action4 = KeyCode.KeypadMinus;
        }
        else KeyboardType = null;
    }
}
