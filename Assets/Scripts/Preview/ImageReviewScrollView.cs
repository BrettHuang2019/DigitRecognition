using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageReviewScrollView : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private ImageReview imagePrefab;
    public UnityAction<string, string> OnDeleteImage;
    
    private ToggleGroup group;
    private ToggleGroup Group
    {
        get
        {
            if (!group) group = GetComponent<ToggleGroup>();
            return group;
        }
    }

    public void RefreshImages(string folderName, Dictionary<string, Texture2D> dictionary)
    {
        if (content.childCount>0)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (KeyValuePair<string,Texture2D> imageInfo in dictionary)
        {
            ImageReview newImage = Instantiate(imagePrefab, content);
            newImage.InitImage(folderName, imageInfo.Key, imageInfo.Value);
            newImage.SetToggleGroup(Group);
            newImage.OnDeleteBtnClick += DeleteImage;
        }
    }

    public void DeleteImage(string folderName, string fileName)
    {
        OnDeleteImage(folderName, fileName);
    }
}
