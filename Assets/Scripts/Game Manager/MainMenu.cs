﻿using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameController;
using static PlayerInput;

public class MainMenu : MonoBehaviour
{
    public GameObject[] Roles;
    public GameObject[] Controllers;

    public Sprite DogFaceSprite;
    public Sprite WolfFaceSprite;

    public Sprite Keyboard1Sprite;
    public Sprite Keyboard2Sprite;
    public Sprite ControllerSprite;

    public List<PlayerInput> players;
    public Dictionary<string, PlayerController> playablePlayers;
    private List<Guid> deviceGUID;

    private bool InMainMenu = true;

    private int currentPlayerIndex = 0;

    private float currentTimeChangeRole = 0f;
    private float intervalChangeRole = 0.2f;

    public float ProgressBarSpeed = 30f;

    [SerializeField]
    private Transform[] PlayerSpawnPosition;

    void Start()
    {
        players = new List<PlayerInput>();
        playablePlayers = new Dictionary<string, PlayerController>();
        deviceGUID = new List<Guid>();
        MainMenuProgressBar.Instance.Hide();
    }

    void Update()
    {
        HandlePlayerCommands();
        CreatePlayers();
        
        LoadGame();
    }

    private void HandlePlayerCommands()
    {
        foreach (PlayerInput player in players)
        {
            if(player.Action1WasPressed())
            {
                if(player.RoleLocked) return;

                LockRoleForPlayer(player.GetGUID());
                AddPlayer(player);
            }

            if(player.Action2WasPressed())
            {
                UnlockRoleForPlayer(player.GetGUID());
                RemovePlayer(player);
            }

            if (player.LeftWasPressed() || player.RightWasPressed())
            {
                if(player.RoleLocked) return;

                ChangeRole(player.GetGUID());
            }
        }
    }

    private void CreatePlayers()
    {
        var playerDevices = players.Where(p => p.Device != null).Select(p => p.Device).ToArray();
        var nonRegisteredDevices = InputManager.Devices.Where(d => !playerDevices.Any(pd => pd.GUID == d.GUID)).ToArray();
        foreach (var device in nonRegisteredDevices)
        {
            if (device.GetControl(InputControlType.Action1).WasPressed)
            {
                CreatePlayerForDevice(device);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreatePlayerForDevice(null, "Space");
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CreatePlayerForDevice(null, "Enter");
        }
    }

    private void LoadGame()
    {
        int wolvesCount = players.Where(p => p.Role == PlayerType.Wolf).Count();
        int dogsCount = players.Where(p => p.Role == PlayerType.Dog).Count();
        if (wolvesCount > 0 && dogsCount > 0 && AllPlayersAreLocked())
        {
            LoadProgressBar();
        }
        else
        {
            UnloadProgressBar();
        }

        //Start game
        if (MainMenuProgressBar.Instance.ProgressBarValue >= 100)
        {
            PlayerDevicesData.Players = players;
            SceneManager.LoadScene("Simple");
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

    private void AddPlayer(PlayerInput player)
    {
        if (player == null) return;

        switch (player.Role)
        {
            case PlayerType.Dog:
                var dog = Spawner.Instance.SpawnDog(player, GetMenuSpawnPositionFromPlayerIndex(player.Index));
                playablePlayers.Add(player.GetGUID(), dog.GetComponent<Dog>());
                break;
            case PlayerType.Wolf:
                var wolf = Spawner.Instance.SpawnWolf(player, GetMenuSpawnPositionFromPlayerIndex(player.Index));
                playablePlayers.Add(player.GetGUID(), wolf.GetComponent<Wolf>());
                break;
        }
    }

    private void RemovePlayer(PlayerInput player)
    {
        if (player == null) return;

        var playablePlayer = playablePlayers[player.GetGUID()];

        if (playablePlayer == null) return;
        playablePlayers.Remove(player.GetGUID());
        playablePlayer.Die();
    }

    private Vector2 GetMenuSpawnPositionFromPlayerIndex(int playerIndex)
    {
        return new Vector2(PlayerSpawnPosition[playerIndex].position.x, PlayerSpawnPosition[playerIndex].position.y);
    }

    private void LoadProgressBar()
    {
        if (MainMenuProgressBar.Instance.ProgressBarValue < 100)
        {
            MainMenuProgressBar.Instance.ProgressBarValue += ProgressBarSpeed * Time.deltaTime;
        }

        if (!MainMenuProgressBar.Instance.Shown && MainMenuProgressBar.Instance.ProgressBarValue > 0)
        {
            MainMenuProgressBar.Instance.Show();
        }
    }

    private void UnloadProgressBar()
    {
        if (MainMenuProgressBar.Instance.ProgressBarValue > 0)
        {
            MainMenuProgressBar.Instance.ProgressBarValue -= ProgressBarSpeed * Time.deltaTime;
        }

        if (MainMenuProgressBar.Instance.Shown && MainMenuProgressBar.Instance.ProgressBarValue <= 0)
        {
            MainMenuProgressBar.Instance.Hide();
        }
    }

    private void UnlockRoleForPlayer(string guid)
    {
        PlayerInput player = GetPlayerFromDeviceGUID(guid);
        if (player == null) return;

        player.RoleLocked = false;

        var color = ColorUtils.ToColor(Roles[player.Index].GetComponent<SpriteRenderer>().color);
        Roles[player.Index].GetComponent<SpriteRenderer>().color = color;
    }

    private PlayerInput LockRoleForPlayer(string guid)
    {
        PlayerInput player = GetPlayerFromDeviceGUID(guid);

        if (player == null || player.RoleLocked) return null;

        player.RoleLocked = true;

        var color = ColorUtils.ToBlack(Roles[player.Index].GetComponent<SpriteRenderer>().color);
        Roles[player.Index].GetComponent<SpriteRenderer>().color = color;

        return player;
    }

    private void ChangeRole(string guid, string keyboardType = null)
    {
        PlayerInput player = GetPlayerFromDeviceGUID(guid);

        if (player == null || player.RoleLocked) return;

        //set role and role image in menu
        if (player.Role == PlayerType.Dog)
        {
            player.Role = PlayerType.Wolf;
            Roles[player.Index].GetComponent<SpriteRenderer>().sprite = WolfFaceSprite;
        }
        else
        {
            player.Role = PlayerType.Dog;
            Roles[player.Index].GetComponent<SpriteRenderer>().sprite = DogFaceSprite;
        }
    }

    private bool CreatePlayerForDevice(InputDevice device, string keyboard = null)
    {
        PlayerInput newPlayer = null;
        if (device == null)
        {
            if(keyboard == "Space")
            {
                if (CheckPlayerKeyboard(EControllerType.Keyboard1)) return true;
                newPlayer = new PlayerInput(null, currentPlayerIndex, PlayerType.Dog, EControllerType.Keyboard1);
            }
            else if(keyboard == "Enter")
            {
                if (CheckPlayerKeyboard(EControllerType.Keyboard2)) return true;
                newPlayer = new PlayerInput(null, currentPlayerIndex, PlayerType.Dog, EControllerType.Keyboard2);
            }
        }
        else
        {

            bool alreadyCreated = CheckDeviceGUID(device.GUID);
            if (alreadyCreated) return true;

            //set autrement aleatoirement ou choisit le role
            newPlayer = new PlayerInput(device, currentPlayerIndex, PlayerType.Dog, EControllerType.Controller);
        }

        //set role image
        if (newPlayer.Role == PlayerType.Dog)
        {
            Roles[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = DogFaceSprite;
        }
        else
        {
            Roles[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = WolfFaceSprite;
        }

        Roles[currentPlayerIndex].SetActive(true);
        
        if (Controllers[currentPlayerIndex].GetComponent<SpriteRenderer>() != null)
        {
            switch (newPlayer.ControllerType)
            {
                case EControllerType.Controller:
                    Controllers[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = ControllerSprite;
                    break;
                case EControllerType.Keyboard1:
                    Controllers[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = Keyboard1Sprite;
                    break;
                case EControllerType.Keyboard2:
                    Controllers[currentPlayerIndex].GetComponent<SpriteRenderer>().sprite = Keyboard2Sprite;
                    break;
            }
        }
        

        Controllers[currentPlayerIndex].SetActive(true);
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

    private PlayerInput GetPlayerFromDeviceGUID(string guid)
    {
        return players.Where(p => p.GetGUID() == guid).First();
    }
}
