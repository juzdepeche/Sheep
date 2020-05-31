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
    public int Index;
    public PlayerType Role;
    public bool RoleLocked = false;
    public EControllerType ControllerType;
    public string KeyboardType;

    public string InputAxeX;
    public string InputAxeY;

    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Action1;
    public KeyCode Action2;
    public KeyCode Action3;
    public KeyCode Action4;

    public Player(InputDevice device, int index, PlayerType role, EControllerType controllerType)
    {
        Device = device;
        Index = index;
        Role = role;
        ControllerType = controllerType;

        //to refactor
        if (controllerType == EControllerType.Keyboard1)
        {
            KeyboardType = "Space";
            InputAxeX = "Horizontal3";
            InputAxeY = "Vertical3";
            Left = KeyCode.A;
            Right = KeyCode.D;
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
            Left = KeyCode.LeftArrow;
            Right = KeyCode.RightArrow;
            Action1 = KeyCode.KeypadEnter;
            Action2 = KeyCode.Keypad0;
            Action3 = KeyCode.KeypadPlus;
            Action4 = KeyCode.KeypadMinus;
        }
        else KeyboardType = null;
    }

    public bool Action1WasPressed()
    {
        bool isController = ControllerType == EControllerType.Controller;
		bool wasPressed = isController ? Device.GetControl(InputControlType.Action1).WasPressed : Input.GetKeyDown(Action1);

        return wasPressed;
    }

    public bool Action2WasPressed()
    {
		bool isController = ControllerType == EControllerType.Controller;
		return isController ? Device.GetControl(InputControlType.Action2).WasPressed : Input.GetKeyDown(Action2);
    }

    public bool Action3WasPressed()
    {
        bool isController = ControllerType == EControllerType.Controller;
		return isController ? Device.GetControl(InputControlType.Action3).WasPressed : Input.GetKeyDown(Action3);
    }

    public bool LeftWasPressed()
    {
        bool isController = ControllerType == EControllerType.Controller;
		return isController ? Device.GetControl(InputControlType.DPadLeft).WasPressed : Input.GetKeyDown(Left);
    }

    public bool RightWasPressed()
    {
        bool isController = ControllerType == EControllerType.Controller;
		return isController ? Device.GetControl(InputControlType.DPadRight).WasPressed : Input.GetKeyDown(Right);
    }

    public string GetGUID()
    {
        bool isController = ControllerType == EControllerType.Controller;
        return isController ? Device.GUID.ToString() : KeyboardType;
    } 
}
