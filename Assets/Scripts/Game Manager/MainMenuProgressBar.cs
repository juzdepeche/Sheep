using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuProgressBar : MonoBehaviour
{
    public Image ProgressBar;
    public static float ProgressBarValue = 0;

    void Update()
    {
        ChangeProgress();
    }

    private void ChangeProgress()
    {
        float amount = (ProgressBarValue / 100.0f);
        ProgressBar.fillAmount = amount;
    }
}
