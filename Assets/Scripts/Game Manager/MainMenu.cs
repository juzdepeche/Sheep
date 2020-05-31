using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameController;
using static Player;

public class MainMenu : MonoBehaviour
{
    public GameObject[] Roles;
    public GameObject[] Controllers;

    public Sprite DogFaceSprite;
    public Sprite WolfFaceSprite;

    public Sprite Keyboard1Sprite;
    public Sprite Keyboard2Sprite;
    public Sprite ControllerSprite;

    public List<Player> players;
    private List<Guid> deviceGUID;

    private bool InMainMenu = true;

    private int currentPlayerIndex = 0;

    private float currentTimeChangeRole = 0f;
    private float intervalChangeRole = 0.2f;

    public float ProgressBarSpeed = 30f;

    [SerializeField]
    private Transform[] PlayerSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<Player>();
        deviceGUID = new List<Guid>();
        MainMenuProgressBar.Instance.Hide();
    }

    // Update is called once per frame
    void Update()
    {
        ControllerDeviceInput();
        KeyboardInput();

        //load game
        int wolvesCount = players.Where(p => p.Role == PlayerType.Wolf).Count();
        int dogsCount = players.Where(p => p.Role == PlayerType.Wolf).Count();
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

    private void ControllerDeviceInput()
    {
        foreach (var device in InputManager.Devices)
        {
            //join/lock
            if (device.GetControl(InputControlType.Action1).WasPressed)
            {
                bool playerAlreadyCreated = CreatePlayerForDevice(device);
                if (playerAlreadyCreated)
                {
                    Player player = LockRoleForPlayer(device.GUID);
                    AddPlayer(player);
                }
            }

            //unlock
            if (device.GetControl(InputControlType.Action2).WasPressed)
            {
                bool playerAlreadyCreated = CreatePlayerForDevice(device);
                if (playerAlreadyCreated)
                {
                    UnlockRoleForPlayer(device.GUID);
                    //RemovePlayer()
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
    }

    private void KeyboardInput()
    {
        //Keyboard inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool playerAlreadyCreated = CreatePlayerForDevice(null, "Space");
            if (playerAlreadyCreated)
            {
                Player player = LockRoleForPlayer(Guid.Empty, "Space");
                AddPlayer(player);
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            bool playerAlreadyCreated = CreatePlayerForDevice(null, "Enter");
            if (playerAlreadyCreated)
            {
                Player player = LockRoleForPlayer(Guid.Empty, "Enter");
                AddPlayer(player);
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

    private void AddPlayer(Player player)
    {
        if (player == null) return;

        switch (player.Role)
        {
            case PlayerType.Dog:
                Spawner.Instance.SpawnDog(player, GetMenuSpawnPositionFromPlayerIndex(player.Index));
                break;
            case PlayerType.Wolf:
                Spawner.Instance.SpawnWolf(player, GetMenuSpawnPositionFromPlayerIndex(player.Index));
                break;
        }
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

    private void UnlockRoleForPlayer(Guid guid)
    {
        Player player = GetPlayerFromDeviceGUID(guid);
        if (player == null) return;

        player.RoleLocked = false;

        var color = ColorUtils.ToColor(Roles[player.Index].GetComponent<SpriteRenderer>().color);
        Roles[player.Index].GetComponent<SpriteRenderer>().color = color;
    }

    private Player LockRoleForPlayer(Guid guid, string keyboardType = null)
    {
        Player player = keyboardType != null ? GetPlayerFromKeyboardType(keyboardType) : GetPlayerFromDeviceGUID(guid);

        if (player == null || player.RoleLocked) return null;

        player.RoleLocked = true;

        var color = ColorUtils.ToBlack(Roles[player.Index].GetComponent<SpriteRenderer>().color);
        Roles[player.Index].GetComponent<SpriteRenderer>().color = color;

        return player;
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
