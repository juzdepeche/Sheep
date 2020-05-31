using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuProgressBar : MonoBehaviour
{
    public static MainMenuProgressBar Instance;
    public GameObject ProgressBarFront;
    public GameObject ProgressBarBack;
    public float ProgressBarValue = 0;

    public bool Shown = false;

    private void Awake() {
        Instance = this;
    }

    void Update()
    {
        ChangeProgress();
    }

    private void ChangeProgress()
    {
        float amount = (ProgressBarValue / 100.0f);
        
        Image progressBarImage = ProgressBarFront.GetComponent<Image>();
        progressBarImage.fillAmount = amount;
    }

    public void Show()
    {
        Shown = true;
        ProgressBarFront.GetComponent<Image>().enabled = true;
        ProgressBarBack.GetComponent<Image>().enabled = true;
    }

    public void Hide()
    {
        Shown = false;
        ProgressBarFront.GetComponent<Image>().enabled = false;
        ProgressBarBack.GetComponent<Image>().enabled = false;
    }
}
