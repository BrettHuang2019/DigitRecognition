using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveImage : MonoBehaviour
{
    [SerializeField] private ImageManager imageManager;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private LineDrawer lineDrawer;
    [SerializeField] private PreviewManager previewManager;
    
    
    private Button btn;
    private void Start()
    {
        btn = GetComponent<Button>();
        
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        btn.interactable = toggleGroup.AnyTogglesOn();
    }

    public void Save()
    {
        if (btn.interactable)
        {
            imageManager.SaveImage(previewManager.ScaledTexture, int.Parse(toggleGroup.GetFirstActiveToggle().name));
            btn.interactable = false;
        }
    }
}
