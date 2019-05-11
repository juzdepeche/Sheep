using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class PlayerManager : MonoBehaviour
{
    public GameObject[] Roles;

    public Sprite DogFaceSprite;
    public Sprite WolfFaceSprite;

    public List<Player> players;
    private List<Guid> deviceGUID;

    private bool InMainMenu = true;

    private int currentPlayerIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<Player>();
        deviceGUID = new List<Guid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InMainMenu)
        {
            InputDevice device = InputManager.ActiveDevice;

            if (device.GetControl(InputControlType.Action1))
            {
                Debug.Log(1);
                CreatePlayerForDevice(device);
            }
        }
    }

    private void CreatePlayerForDevice(InputDevice device)
    {
        bool alreadyCreated = CheckDeviceGUID(device.GUID);
        if (alreadyCreated) return;
        
        //set autrement aleatoirement ou choisit le role
        Player newPlayer = new Player(device, currentPlayerIndex, PlayerType.Dog);

        //set la face du joueur
        if(newPlayer.Role == PlayerType.Dog)
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
    }

    private bool CheckDeviceGUID(Guid gUID)
    {
        if (deviceGUID.Contains(gUID)) return true;

        deviceGUID.Add(gUID);

        return false;
    }
}
