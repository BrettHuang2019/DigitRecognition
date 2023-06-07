using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageReview : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Button deleteBtn;
    public UnityAction<string, string> OnDeleteBtnClick;

    private string folderName; 
    private string fileName;
    private ImageReviewScrollView scrollView;
    

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
    
    private void OnEnable()
    {
        deleteBtn.onClick.AddListener(DeleteImage);
    }

    public void InitImage(string folderName, string fileName, Texture2D tex)
    {
        this.folderName = folderName;
        this.fileName = fileName;
        image.texture = tex;
    }
    
    public void SetToggleGroup(ToggleGroup group)
    {
        Toggle.group = group;
    }

    private void DeleteImage()
    {
        OnDeleteBtnClick?.Invoke(folderName, fileName);
        Destroy(gameObject);
    }



}
