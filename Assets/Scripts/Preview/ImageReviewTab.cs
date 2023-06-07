using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageReviewTab : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private string labelName;
    public string LabelName => labelName;
    public UnityAction<string> OnToggleTurnOn;

    private Toggle toggle;
    private Toggle Toggle
    {
        get
        {
            if (!toggle)
                toggle = GetComponent<Toggle>();
            return toggle;
        }
    }

    public void SetLabel(string labelName)
    {
        this.labelName = labelName;
        if (!tmp)
            tmp = GetComponentInChildren<TextMeshProUGUI>();
        
        tmp.text = labelName;

        Toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void SetToggleGroup(ToggleGroup group)
    {
        Toggle.group = group;
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
            OnToggleTurnOn?.Invoke(labelName);
    }
}
