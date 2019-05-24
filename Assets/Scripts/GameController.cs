using System;
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

    public float[] LayersLevels;
    public int LayersNumber;
    public int wolfScoreGoal;
    public Text wolfGoal;
    public Text gameoverText;
    public Text restartText;

    public Image NightImage;
    public Image FaceImage;

    public Sprite DogFace;
    public Sprite WolfFace;

    public int MinLayer = 8;

    private float cameraWidth;
    private float cameraHeight;

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

    private bool isGameOver = false;
    private bool isNightDropping = false;

    private float currentAlphaNightImage = 0;
    private float alphaNightImageCurrentTime = 0;

    private PlayerType SpecialAttackTurn = PlayerType.Dog;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Wolves = new List<GameObject>();
        Dogs = new List<GameObject>();

        wolfGoal.text = wolfScoreGoal.ToString();

        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        SplitLevelInLayers();

        foreach(Player player in PlayerDevicesData.Players)
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
            if (isGameOver)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        //nuit
        if (isNightDropping)
        {
            var tempColor = NightImage.color;
            tempColor.a = alphaNightImageCurrentTime/toNightTime - 0.01f;
            NightImage.color = tempColor;

            alphaNightImageCurrentTime += Time.deltaTime;

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

    private void SplitLevelInLayers()
    {
        LayersLevels = new float[LayersNumber];

        float step = (cameraHeight / 2) / (LayersNumber / 2);

        float currentStep = cameraHeight / 2;

        for (var i = 0; i < LayersLevels.Length; i++)
        {
            if (i != 0)
            {
                currentStep -= step;
            }

            LayersLevels[i] = currentStep;
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
                    sheep.GetComponent<Sheep>().FleeFrom(murderPosition, UnityEngine.Random.Range(time, time + 1f), bleed);
                }
            }
        }
    }

    public bool KillFromWolf(Vector2 wolfMouth)
    {
        RaycastHit2D hit = Physics2D.Raycast(wolfMouth, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Sheep" && ProgressBar.HungryValue >= 100)
            {
                if (hit.collider.GetComponent<Sheep>().dead) return false;
                hit.collider.GetComponent<Sheep>().Kill();
                FleeSheepsFrom(wolfMouth, 1f);

                wolfScoreGoal--;
                wolfGoal.text = wolfScoreGoal.ToString();

                ProgressBar.HungryValue = 0;
                AudioManager.Instance.Kill();
                return true;
            }
            else if(hit.collider.tag == "Dog" && ProgressBar.HungryValue >= 100 && wolfScoreGoal == 0)
            {
                if (hit.collider.GetComponent<Dog>().dead) return false;
                hit.collider.GetComponent<Dog>().Die();
                FleeSheepsFrom(wolfMouth, 1.5f);
                DogsNumber--;

                ProgressBar.HungryValue = 0;

                if (DogsNumber == 0)
                {
                    GameOver("Wolf won.");
                }
                AudioManager.Instance.Kill();
                return true;
            }
        }

        return false;
    }

    public void KillSheepFromDog(Vector2 dogMouth)
    {
        RaycastHit2D hit = Physics2D.Raycast(dogMouth, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Sheep")
            {
                if (hit.collider.GetComponent<Sheep>().dead) return;
                hit.collider.GetComponent<Sheep>().Kill();
                FleeSheepsFrom(dogMouth, 1f);

                wolfScoreGoal--;
                wolfGoal.text = wolfScoreGoal.ToString();
                AudioManager.Instance.Kill();
            }
            else if(hit.collider.tag == "Wolf")
            {
                KillWolf(hit.collider.gameObject);
                if (WolvesNumber == 0)
                {
                    GameOver("Doggo won.");
                }
            }
        }
    }

    private void KillWolf(GameObject wolf)
    {
        wolf.GetComponent<Wolf>().Die();
        WolvesNumber--;
        AudioManager.Instance.Kill();
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

    public void WolfGetOutFromBody(Vector2 wolfPosition, bool[] blood)
    {
        AudioManager.Instance.SwitchToStressMusic();
        //Spawner.Instance.SpawnBoule(wolfPosition);
    }

    public bool AskWoofWoof(Vector2 doggoMouth)
    {
        if (SpecialAttackTurn == PlayerType.Dog && ProgressBar.SpecialValue > 100)
        {
            FleeSheepsFromDog(doggoMouth, 2f, false);
            //SheepsGoToFromDog(doggoMouth, 1f);
            SpecialAttackTurn = PlayerType.Wolf;
            ProgressBar.SpecialValue = 0;
            FaceImage.sprite = WolfFace;
            AudioManager.Instance.Bark();
            return true;
        }
        return false;
    }

    private void FleeSheepsFromDog(Vector2 doggoMouth, float time, bool bleed)
    {
        foreach (GameObject sheep in Sheeps)
        {
            if (sheep.gameObject != null)
            {
                if (murderReactRange > Vector2.Distance(doggoMouth, sheep.transform.position))
                {
                    sheep.GetComponent<Sheep>().FleeFrom(doggoMouth, UnityEngine.Random.Range(time, time + 1f), bleed);
                }
            }
        }
    }

    private void SheepsGoToFromDog(Vector2 doggoMouth, float time)
    {
        foreach (GameObject sheep in Sheeps)
        {
            if (sheep.gameObject != null)
            {
                if (murderReactRange > Vector2.Distance(doggoMouth, sheep.transform.position))
                {
                    sheep.GetComponent<Sheep>().GoTo(doggoMouth, UnityEngine.Random.Range(time, time + 1f));
                }
            }
        }
    }

    public bool AskAhou()
    {
        if (SpecialAttackTurn == PlayerType.Wolf && ProgressBar.SpecialValue > 100)
        {
            isNightDropping = true;
            HideSheeps();
            SpecialAttackTurn = PlayerType.Dog;
            FaceImage.sprite = DogFace;
            ProgressBar.SpecialValue = 0;
            AudioManager.Instance.Howl();
            return true;
        }
        return false;
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
}
