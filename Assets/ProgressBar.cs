using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image SpecialBar;
    public Image HungryBar;
    public static float SpecialValue = 0;
    public static float HungryValue = 0;
    // Update is called once per frame
    void Update()
    {
        ChangeProgress();
    }

    private void ChangeProgress()
    {
        float amount = (SpecialValue / 100.0f) * 180.0f / 360.0f;
        SpecialBar.fillAmount = amount;

        amount = (HungryValue / 100.0f) * 180.0f / 360.0f;
        HungryBar.fillAmount = amount;
    }
}
