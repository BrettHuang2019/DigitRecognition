using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PredictionPlot : MonoBehaviour
{
    public Image[] barPlots;

    private void Start()
    {
        barPlots = GetComponentsInChildren<Image>();
        ClearPlot();
    }

    public void ClearPlot()
    {
        UpdatePlot(new float[10]);
    }

    public void UpdatePlot(float[] values)
    {
        for (int i=0; i < values.Length; i++)
        {
            barPlots[i].fillAmount = values[i];
            barPlots[i].color = Color.Lerp(Color.blue, Color.green, values[i]);
        }
    }
}
