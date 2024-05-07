using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance { get; private set; }

    public TextMeshProUGUI garbagePointsText; // The TextMeshProUGUI component that will display the GarbagePoints
    public TextMeshProUGUI plantsPointsText; // The TextMeshProUGUI component that will display the GarbagePoints


    [SerializeField]
    public int difficulty = 1; // The difficulty level

    public int totalTrash; // The total number of trash
    public int cleanedTrash; // The number of cleaned trash
    public int plantSucked; // The number of plants sucked

    void Awake()
    {
        // Ensure there's only one instance of CleaningManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Define the total number of trash based on the difficulty level
        totalTrash = difficulty * 10; // Modify this as needed
    }

    // Call this method whenever a piece of trash is cleaned
    public void IncreaseCleanedTrash()
    {
        cleanedTrash++;
        if (garbagePointsText != null)
        {
            garbagePointsText.text = $"{CleaningManager.Instance.GetCleaningPercentage():0.00}%";
        }
    }

    // Returns the cleaning percentage
    public float GetCleaningPercentage()
    {
        return ((float)cleanedTrash / totalTrash) * 100;
    }

    public void IncreasePlantSucked()
    {
        plantSucked++;
        if (plantsPointsText != null)
        {
            plantsPointsText.text = $"Plants: {CleaningManager.Instance.GetPlantSucked()}";
        }
    }

    public int GetPlantSucked()
    {
        return plantSucked;
    }
}