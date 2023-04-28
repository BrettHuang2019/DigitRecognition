using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PredictionPlot : MonoBehaviour
{
    public Image[] barPlots;

    private float[] targetFloats;
    private float lastSample;
    private ToggleGroup toggleGroup;

    private void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        ClearPlot();
    }

    public void ClearPlot()
    {
        UpdatePlot(new float[10]);
        
        for (int i=0; i < targetFloats.Length; i++)
        {
            barPlots[i].fillAmount = 0;
            barPlots[i].color = Color.yellow;
        }

        toggleGroup.SetAllTogglesOff();
    }

    public void UpdatePlot(float[] values)
    {
        targetFloats = values;
        lastSample = Time.time;
    }

    private void Update()
    {
        if ((Time.time - lastSample) < 1f)
            return;
        
        for (int i = 0; i < targetFloats.Length; i++)
        {
            barPlots[i].fillAmount = Mathf.Lerp(barPlots[i].fillAmount, targetFloats[i], Time.deltaTime * 15);
            barPlots[i].color = Color.Lerp(Color.yellow, Color.green, barPlots[i].fillAmount);
        }
    }
}
