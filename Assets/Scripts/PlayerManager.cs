using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameController;
using static Player;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] Roles;

    public Sprite DogFaceSprite;
    public Sprite WolfFaceSprite;

    public List<Player> players;
    private List<Guid> deviceGUID;

    private bool InMainMenu = true;

    private int currentPlayerIndex = 0;

    private float currentTimeChangeRole = 0f;
    private float intervalChangeRole = 0.2f;

    public float ProgressBarSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<Player>();
        deviceGUID = new List<Guid>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var device in InputManager.Devices)
        {
            //join/lock
            if (device.GetControl(InputControlType.Action1).WasPressed)
            {
                bool playerAlreadyCreated = CreatePlayerForDevice(device);
                if (playerAlreadyCreated)
                {
                    LockRoleForPlayer(device.GUID);
                }
            }

            //unlock
            if (device.GetControl(InputControlType.Action2).WasPressed)
            {
                bool playerAlreadyCreated = CreatePlayerForDevice(device);
                if (playerAlreadyCreated)
                {
                    UnlockRoleForPlayer(device.GUID);
                }
            }

            //joystick (changerole)
            if (device.LeftStickX > 0.7 || device.LeftStickX < -0.9)
            {
                currentTimeChangeRole += Time.deltaTime;
                if (intervalChangeRole < currentTimeChangeRole)
                {
                    ChangeRole(device.GUID);
                    currentTimeChangeRole = 0;
                }
            }
            else
            {
                currentTimeChangeRole = 0;
            }

            //dpad c(change role)
            if (device.DPadLeft.WasPressed || device.DPadRight.WasPressed)
            {
                ChangeRole(device.GUID);
            }
        }

        //Keyboard inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool playerAlreadyCreated = CreatePlayerForDevice(null, "Space");
            if (playerAlreadyCreated)
            {
                LockRoleForPlayer(Guid.Empty, "Space");
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            bool playerAlreadyCreated = CreatePlayerForDevice(null, "Enter");
            if (playerAlreadyCreated)
            {
                LockRoleForPlayer(Guid.Empty, "Enter");
            }
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            ChangeRole(Guid.Empty, "Space");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeRole(Guid.Empty, "Enter");
        }

        //load game
        if (players.Count >= 2 && AllPlayersAreLocked())
        {
            LoadProgressBar();
        }
        else
        {
            UnloadProgressBar();
        }

        //Start game
        if (MainMenuProgressBar.ProgressBarValue >= 100)
        {
            PlayerDevicesData.Players = players;
            SceneManager.LoadScene("Main");
        }
    }

    private bool AllPlayersAreLocked()
    {
        foreach (var player in players)
        {
            if (!player.RoleLocked)
            {
                return false;
            }
        }
        return true;
    }

    private void LoadProgressBar()
    {
        if (MainMenuProgressBar.ProgressBarValue < 100)
        {
            MainMenuProgressBar.ProgressBarValue += ProgressBarSpeed * Time.deltaTime;
        }
    }

    private void UnloadProgressBar()
    {
        if (MainMenuProgressBar.ProgressBarValue > 0)
        {
            MainMenuProgressBar.ProgressBarValue -= ProgressBarSpeed * Time.deltaTime;
        }
    }

    private void UnlockRoleForPlayer(Guid guid)
    {
        Player player = GetPlayerFromDeviceGUID(guid);
        if (player == null) return;

        player.RoleLocked = false;

        var tempColor = Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().color;
        tempColor.r = 255f;
        tempColor.g = 255f;
        tempColor.b = 255f;

        Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().color = tempColor;
    }

    private void LockRoleForPlayer(Guid guid, string keyboardType = null)
    {
        Player player = keyboardType != null ? GetPlayerFromKeyboardType(keyboardType) : GetPlayerFromDeviceGUID(guid);

        if (player == null || player.RoleLocked) return;

        player.RoleLocked = true;

        var tempColor = Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().color;
        tempColor.r = 0f;
        tempColor.g = 0f;
        tempColor.b = 0f;

        Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().color = tempColor;
    }

    private void ChangeRole(Guid guid, string keyboardType = null)
    {
        Player player = null;

        if(keyboardType != null)
        {
            player = GetPlayerFromKeyboardType(keyboardType);
        }
        else
        {
            player = GetPlayerFromDeviceGUID(guid);
        }

        if (player == null || player.RoleLocked) return;

        //set la face du joueur
        if (player.Role == PlayerType.Dog)
        {
            player.Role = PlayerType.Wolf;
            Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().sprite = WolfFaceSprite;
        }
        else
        {
            player.Role = PlayerType.Dog;
            Roles[player.PlayerIndex].GetComponent<SpriteRenderer>().sprite = DogFaceSprite;
        }
    }

    private bool CreatePlayerForDevice(InputDevice device, string keyboard = null)
    {
        Player newPlayer = null;
        if (device == null)
        {
            if(keyboard == "Space")
            {
                if (CheckPlayerKeyboard(EControllerType.Keyboard1)) return true;
                newPlayer = new Player(null, currentPlayerIndex, PlayerType.Dog, EControllerType.Keyboard1);
            }
            else if(keyboard == "Enter")
            {
                if (CheckPlayerKeyboard(EControllerType.Keyboard2)) return true;
                newPlayer = new Player(null, currentPlayerIndex, PlayerType.Dog, EControllerType.Keyboard2);
            }
        }
        else
        {

            bool alreadyCreated = CheckDeviceGUID(device.GUID);
            if (alreadyCreated) return true;

            //set autrement aleatoirement ou choisit le role
            newPlayer = new Player(device, currentPlayerIndex, PlayerType.Dog, EControllerType.Controller);
        }

        //set la face du joueur
        if (newPlayer.Role == PlayerType.Dog)
        {
            Roles[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = DogFaceSprite;
        }
        else
        {
            Roles[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = WolfFaceSprite;
        }

        Roles[currentPlayerIndex].SetActive(true);

        players.Add(newPlayer);
        currentPlayerIndex++;
        return false;
    }

    private bool CheckPlayerKeyboard(EControllerType keyboardType)
    {
        foreach (var player in players)
        {
            if (player.ControllerType == keyboardType)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckDeviceGUID(Guid gUID)
    {
        if (deviceGUID.Contains(gUID)) return true;

        deviceGUID.Add(gUID);

        return false;
    }

    private Player GetPlayerFromDeviceGUID(Guid guid)
    {
        foreach (var player in players)
        {
            if (player.Device == null) continue;
            if (player.Device.GUID == guid)
            {
                return player;
            }
        }
        return null;
    }

    private Player GetPlayerFromKeyboardType(string keyboardType)
    {
        foreach (var player in players)
        {
            if (player.KeyboardType == keyboardType)
            {
                return player;
            }
        }
        return null;
    }
}
