using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageReviewToggleGroup : MonoBehaviour
{
    [SerializeField] private ImageReviewTab tabPrefab;
    private ImageReviewTab[] tabChildren = new ImageReviewTab[] { };
    public UnityAction<string> OnToggleOn;

    private ToggleGroup group;
    private ToggleGroup Group
    {
        get
        {
            if (!group) group = GetComponent<ToggleGroup>();
            return group;
        }
    }

    public string ActiveLabelName
    {
        get
        {
            Toggle activeToggle = Group.GetFirstActiveToggle();
            ImageReviewTab activeTab = activeToggle.GetComponent<ImageReviewTab>();
            return activeTab.LabelName;
        }
    }

    public void RefreshTabs(string[] labels)
    {
        foreach (Transform tab in transform)
        {
            Destroy(tab.gameObject);
        }

        foreach (string label in labels)
        {
            ImageReviewTab newTab = Instantiate(tabPrefab, transform);
            newTab.SetLabel(label);
            newTab.SetToggleGroup(Group);
            newTab.OnToggleTurnOn += OnToggleTurnOn;
        }
    }

    private void OnToggleTurnOn(string labelName)
    {
        OnToggleOn?.Invoke(labelName);
    }
}
