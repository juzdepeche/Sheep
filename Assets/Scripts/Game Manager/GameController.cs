﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum PlayerType
    {
        Wolf,
        Dog
    }
    
    public int LayersNumber;
    public int wolfScoreGoal;
    public Text WolfGoal;
    public Text gameoverText;
    public Text restartText;

    public Image NightImage;
    public Image FaceImage;

    public Sprite DogFace;
    public Sprite WolfFace;

    public int MinLayer = 8;

    public static GameController Instance;

    public int SheepNumber;
    public float murderReactRange = 8f;
    public float NightTime = 2.5f;
    public float toNightTime = 0.4f;

    public float HungryProgressBarSpeed = 0.5f;
    public float SpecialProgressBarSpeed = 0.5f;

    private GameObject[] Sheeps;
    private List<GameObject> Wolves;
    private List<GameObject> Dogs;

    private int WolvesNumber;
    private int DogsNumber;

    public bool isGameOver = false;
    private bool isNightDropping = false;

    private float currentAlphaNightImage = 0;
    private float alphaNightImageCurrentTime = 0;

    private PlayerType SpecialAttackTurn = PlayerType.Dog;

    private void Awake()
    {
        isGameOver = false;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Wolves = new List<GameObject>();
        Dogs = new List<GameObject>();
        
        WolfGoal.text = wolfScoreGoal.ToString();

        if (PlayerDevicesData.Players != null)
        {
            foreach (PlayerInput player in PlayerDevicesData.Players)
            {
                switch (player.Role)
                {
                    case PlayerType.Dog:
                        Dogs.Add(Spawner.Instance.SpawnDog(player));
                        break;
                    case PlayerType.Wolf:
                        Wolves.Add(Spawner.Instance.SpawnWolf(player));
                        break;
                }
            }
        }

        GameData.Instance.AddObserver(OnAction1, GameData.ON_ACTION_1, null);
        GameData.Instance.AddObserver(OnAction3, GameData.ON_ACTION_3, null);
        GameData.Instance.AddObserver(OnStart, GameData.ON_START, null);
        GameData.Instance.AddObserver(OnWolfBodyExit, GameData.ON_WOLF_BODY_EXIT, null);

        WolvesNumber = Wolves.Count;
        DogsNumber = Dogs.Count;

        Sheeps = Spawner.Instance.SpawnSheeps(SheepNumber);

        ProgressBar.SpecialValue = 0;
        ProgressBar.HungryValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }

        //nuit
        if (isNightDropping)
        {
            var tempColor = NightImage.color;
            tempColor.a = alphaNightImageCurrentTime/toNightTime - 0.01f;
            NightImage.color = tempColor;

            alphaNightImageCurrentTime += Time.deltaTime;

            Debug.Log(tempColor.a);
            
            if (alphaNightImageCurrentTime > toNightTime)
            {
                alphaNightImageCurrentTime = 0;
                isNightDropping = false;
            }
        }

        //load special power bar
        if(ProgressBar.SpecialValue < 100)
        {
            ProgressBar.SpecialValue += SpecialProgressBarSpeed * Time.deltaTime;
        }
        if (ProgressBar.HungryValue < 100)
        {
            ProgressBar.HungryValue += HungryProgressBarSpeed * Time.deltaTime;
        }
    }

    public void RestartGame()
    {
        if (isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void FleeSheepsFrom(Vector2 murderPosition, float time, bool bleed = true)
    {
        foreach (GameObject sheep in Sheeps)
        {
            if(sheep.gameObject != null)
            {
                if (murderReactRange > Vector2.Distance(murderPosition, sheep.transform.position))
                {
                    sheep.GetComponent<Sheep>().FleeFrom(murderPosition, UnityEngine.Random.Range(time, time + 1f));
                    sheep.GetComponent<IMortal>().Bleed(1);
                }
            }
        }
    }

    //to rework for the death of a  wolf
    private void GameOver(string text)
    {
        isGameOver = true;
        gameoverText.text = text;
        gameoverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        
        foreach(var wolf in Wolves)
        {
            wolf.GetComponent<Wolf>().ExitBody();
        }
    }

    public bool WolfGetInNewBody(Wolf wolf, Vector2 wolfMouth)
    {
        RaycastHit2D hit = Physics2D.Raycast(wolfMouth, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Sheep")
            {
                Sheep sheep = hit.collider.GetComponent<Sheep>();
                if (sheep.dead)
                {
                    Destroy(sheep.gameObject);
                    wolf.GetInNewBody(sheep.bloodSplatterManager.BloodSpattersBool);
                    AudioManager.Instance.SwitchToMainThemeMusic();
                }
            }
        }

        return false;
    }

    public void OnWolfBodyExit(PlayerController player, string key)
    {
        AudioManager.Instance.SwitchToStressMusic();
        //todo: particles
        //Spawner.Instance.SpawnBoule(wolfPosition);
    }

    // private void SheepsGoToFromDog(Vector2 doggoMouth, float time)
    // {
    //     foreach (GameObject sheep in Sheeps)
    //     {
    //         if (sheep.gameObject != null)
    //         {
    //             if (murderReactRange > Vector2.Distance(doggoMouth, sheep.transform.position))
    //             {
    //                 sheep.GetComponent<Sheep>().GoTo(doggoMouth, UnityEngine.Random.Range(time, time + 1f));
    //             }
    //         }
    //     }
    // }

    public void DoDogSuperAttack(Vector2 doggoMouth)
    {
        if (SpecialAttackTurn == PlayerType.Dog && ProgressBar.SpecialValue >= 100)
        {
            FleeSheepsFromDog(doggoMouth, 2f);
            SpecialAttackTurn = PlayerType.Wolf;
            ProgressBar.SpecialValue = 0;
            FaceImage.sprite = WolfFace;
            AudioManager.Instance.Bark();
        }
    }

    private void FleeSheepsFromDog(Vector2 doggoMouth, float time)
    {
        foreach (GameObject sheep in Sheeps)
        {
            if (sheep.gameObject != null)
            {
                if (murderReactRange > Vector2.Distance(doggoMouth, sheep.transform.position))
                {
                    sheep.GetComponent<Sheep>().FleeFrom(doggoMouth, UnityEngine.Random.Range(time, time + 1f));
                }
            }
        }
    }

    private void HideSheeps()
    {
        foreach(GameObject sheep in Sheeps)
        {
            if (sheep == null) continue;
            if(sheep.GetComponent<Sheep>() != null)
            {
                sheep.GetComponent<Sheep>().Hide();
                sheep.GetComponent<Sheep>().bloodSplatterManager.HideAll();
            }
        }
        foreach(var wolf in Wolves)
        {
            if (wolf.GetComponent<Wolf>() != null)
            {
                wolf.GetComponent<Wolf>().Hide();
                wolf.GetComponent<Wolf>().bloodSpatterManager.HideAll();
            }
        }
        StartCoroutine(ShowSheeps());
    }

    public void DoWolfSuperAttack()
    {
        if (SpecialAttackTurn == PlayerType.Wolf && ProgressBar.SpecialValue >= 100)
        {
            isNightDropping = true;
            HideSheeps();
            SpecialAttackTurn = PlayerType.Dog;
            FaceImage.sprite = DogFace;
            ProgressBar.SpecialValue = 0;
            AudioManager.Instance.Howl();
        }
    }

    private IEnumerator ShowSheeps()
    {
        yield return new WaitForSeconds(NightTime);
        foreach (GameObject sheep in Sheeps)
        {
            if (sheep == null) continue;
            if (sheep.GetComponent<Sheep>() != null)
            {
                sheep.GetComponent<Sheep>().Show();
                sheep.GetComponent<Sheep>().bloodSplatterManager.ShowAll();
            }
        }
        foreach (var wolf in Wolves)
        {
            if (wolf.GetComponent<Wolf>() != null)
            {
                wolf.GetComponent<Wolf>().Show();
                wolf.GetComponent<Wolf>().bloodSpatterManager.ShowAll();
            }
        }

        var tempColor = NightImage.color;
        tempColor.a = 0f;
        NightImage.color = tempColor;
    }

    private void OnAction1(PlayerController player, string key)
    {
        switch(player.PlayerInput.Role)
        {
            case PlayerType.Dog:
                DoDogKill(player.Mouth.transform.position);
                break;
            case PlayerType.Wolf:
                bool hasKilled = DoWolfKill(player.Mouth.transform.position);
                if (hasKilled)
                {
                    var wolf = (Wolf)player;
                    wolf.OnKillConfirmed();
                }    
                break;
        }
    }

    public void DoDogKill(Vector2 dogMouth)
    {
        RaycastHit2D hit = Physics2D.Raycast(dogMouth, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Sheep")
            {
                if (hit.collider.GetComponent<Sheep>().dead) return;
                KillSheep(hit);
            }
            else if(hit.collider.tag == "Wolf")
            {
                KillWolf(hit.collider.gameObject);
            }
        }
    }

    public bool DoWolfKill(Vector2 wolfMouth)
    {
        RaycastHit2D hit = Physics2D.Raycast(wolfMouth, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Sheep" && ProgressBar.HungryValue >= 100)
            {
                if (hit.collider.GetComponent<Sheep>().dead) return false;

                KillSheep(hit);
                ProgressBar.HungryValue = 0;

                return true;
            }
        }
        return false;
    }

    private void KillSheep(RaycastHit2D raycastHit)
    {
        raycastHit.collider.GetComponent<Sheep>().Die();
        FleeSheepsFrom(raycastHit.collider.transform.position, 1f);

        wolfScoreGoal--;
        UpdateWolfGoalText(wolfScoreGoal);

        if (wolfScoreGoal <= 0)
        {
            GameOver("Wolf won.");
        }

        AudioManager.Instance.Kill();
    }

    private void KillWolf(GameObject wolf)
    {
        wolf.GetComponent<Wolf>().Die();
        AudioManager.Instance.Kill();

        WolvesNumber--;
        if (WolvesNumber == 0)
        {
            GameOver("Doggo won.");
        }
    }

    private void UpdateWolfGoalText(int newScrore)
    {
        WolfGoal.text = newScrore.ToString();
    }

    private void OnAction3(PlayerController player, string key)
    {
        switch(player.PlayerInput.Role)
        {
            case PlayerType.Dog:
                DoDogSuperAttack(player.Mouth.transform.position);
                break;
            case PlayerType.Wolf:
                DoWolfSuperAttack();
                break;
        }
    }

    private void OnStart(PlayerController value, string key)
    {
        RestartGame();
    }
}
