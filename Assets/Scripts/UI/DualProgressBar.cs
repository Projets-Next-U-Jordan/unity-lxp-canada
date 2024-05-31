using UnityEngine;
using UnityEngine.UIElements;

public class DualProgressBar : MonoBehaviour
{
    private VisualElement plantsFill;
    private VisualElement trashFill;
    private Label progressLabel;

    private float maxProgress;
    
    private float plantsProgress;
    private float trashProgress;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find the elements in the UXML
        plantsFill = root.Q<VisualElement>("plantsFill");
        trashFill = root.Q<VisualElement>("trashFill");
        progressLabel = root.Q<Label>("progress-label");

        // Initialize the progress
        SetProgress(0, 0);
    }

    public void SetMaxProgress(float progress)
    {
        maxProgress = progress;
    }
    
    public void SetProgress(float plants, float trash)
    {
        plantsProgress = Mathf.Clamp(plants, 0, 100);
        trashProgress = Mathf.Clamp(trash, 0, 100);

        float totalProgress = plantsProgress + trashProgress;
        if (totalProgress > 100)
        {
            float excess = totalProgress - 100;
            plantsProgress -= excess * (plantsProgress / totalProgress);
            trashProgress -= excess * (trashProgress / totalProgress);
        }

        plantsFill.style.width = new Length(plantsProgress, LengthUnit.Percent);
        trashFill.style.width = new Length(trashProgress, LengthUnit.Percent);

        progressLabel.text = $"{plantsProgress + trashProgress}%";
    }
}