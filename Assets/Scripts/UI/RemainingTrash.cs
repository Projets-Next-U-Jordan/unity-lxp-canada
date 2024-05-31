using UnityEngine;
using UnityEngine.UIElements;

public class RemainingTrash : MonoBehaviour
{
    private Label trashRemainingLabel;
    private float trashRemaining;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find the elements in the UXML
        trashRemainingLabel = root.Q<Label>("trashRemainingCount");
    }

    public void SetProgress(float trash)
    {
        trashRemaining = trash;
        trashRemainingLabel.text = $"{trashRemaining}";
    }
}