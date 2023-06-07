using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReviewPanel : MonoBehaviour
{

    [SerializeField] private ImageManager imageManager;
    [SerializeField] private ImageReviewToggleGroup tabsGroup;
    [SerializeField] private ImageReviewScrollView scrollView;

    private Dictionary<string, Dictionary<string, Texture2D>> imageDict = new Dictionary<string, Dictionary<string, Texture2D>>();

    public void OnEnable()
    {
        scrollView.OnDeleteImage += OnDeleteImage;
        if (LoadImages())
        {
            tabsGroup.RefreshTabs(imageDict.Keys.ToArray());
            tabsGroup.OnToggleOn += OnToggleChanged;
            ShowImages();
        }
    }

    private void OnDisable()
    {
        scrollView.OnDeleteImage -= OnDeleteImage;
    }

    private bool LoadImages()
    {
        imageDict = imageManager.LoadImages();
        return imageDict.Count > 0;
    }

    private void ShowImages(string tabName = null)
    {
        string tabToShow = tabName;
        if (string.IsNullOrEmpty(tabName))
        {
            tabToShow = tabsGroup.ActiveLabelName;
        }

        scrollView.RefreshImages(tabToShow, imageDict[tabToShow]);
    }

    private void OnToggleChanged(string labelName)
    {
        ShowImages(labelName);
    }

    private void OnDeleteImage(string folderName, string fileName)
    {
        imageManager.DeleteImage(folderName, fileName);
    }
}
