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
    private GameObject Wolf;
    private GameObject Dog;

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

        wolfGoal.text = wolfScoreGoal.ToString();

        cameraHeight = 2f * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;

        SplitLevelInLayers();

        foreach(Player player in PlayerDevicesData.Players)
        {
            switch (player.Role)
            {
                case PlayerType.Dog:
                    Dog = Spawner.Instance.SpawnDog(player);
                    break;
                case PlayerType.Wolf:
                    Wolf = Spawner.Instance.SpawnWolf(player);
                    break;
            }
        }

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
            tempColor.a = alphaNightImageCurrentTime/toNightTime - 0.02f;
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

    public bool KillSheepFromWolf(Vector2 wolfMouth)
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

                if (wolfScoreGoal == 0)
                {
                    GameOver("Wolf won.");
                }

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

                if (wolfScoreGoal == 0)
                {
                    GameOver("Wolf won.");
                }
            }
            else if(hit.collider.tag == "Wolf")
            {
                GameOver("Doggo won.");
            }
        }
    }

    private void GameOver(string text)
    {
        isGameOver = true;
        gameoverText.text = text;
        gameoverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);

        Wolf.GetComponent<Wolf>().ExitBody();
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
                }
            }
        }

        return false;
    }

    public bool AskWoofWoof(Vector2 doggoMouth)
    {
        if (SpecialAttackTurn == PlayerType.Dog && ProgressBar.SpecialValue > 100)
        {
            //plus gros range (3 au lieu de 2)
            FleeSheepsFrom(doggoMouth, 3f, false);
            SpecialAttackTurn = PlayerType.Wolf;
            ProgressBar.SpecialValue = 0;
            FaceImage.sprite = WolfFace;
            return true;
        }
        return false;
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
            return true;
        }
        return false;
    }

    private void HideSheeps()
    {
        foreach(GameObject sheep in Sheeps)
        {
            if(sheep.GetComponent<Sheep>() != null)
            {
                sheep.GetComponent<Sheep>().Hide();
                sheep.GetComponent<Sheep>().bloodSplatterManager.HideAll();
            }
        }
        if (Wolf.GetComponent<Wolf>() != null)
        {
            Wolf.GetComponent<Wolf>().Hide();
            Wolf.GetComponent<Wolf>().bloodSpatterManager.HideAll();
        }
        StartCoroutine(ShowSheeps());
    }

    private IEnumerator ShowSheeps()
    {
        yield return new WaitForSeconds(NightTime);
        foreach (GameObject sheep in Sheeps)
        {
            if (sheep.GetComponent<Sheep>() != null)
            {
                sheep.GetComponent<Sheep>().Show();
                sheep.GetComponent<Sheep>().bloodSplatterManager.ShowAll();
            }
        }
        if (Wolf.GetComponent<Wolf>() != null)
        {
            Wolf.GetComponent<Wolf>().Show();
            Wolf.GetComponent<Wolf>().bloodSpatterManager.ShowAll();
        }

        var tempColor = NightImage.color;
        tempColor.a = 0f;
        NightImage.color = tempColor;
    }
}
