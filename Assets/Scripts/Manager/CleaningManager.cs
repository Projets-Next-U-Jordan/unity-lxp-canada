using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance { get; private set; }

    private UIDocument uiDocument;

    [SerializeField]
    public int difficulty = 1;

    public int totalTrash;
    public int cleanedTrash;
    public int plantSucked;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        uiDocument = GetComponent<UIDocument>();
    }

    public void Start()
    {
        // Define the total number of trash based on the difficulty level
        totalTrash = difficulty * 10; // Modify this as needed
    }

    public void Update()
    {
        TextField plantSuckedTextField = uiDocument.rootVisualElement.Q<TextField>("plants_value");
        if (plantSuckedTextField != null)
        {
            plantSuckedTextField.value = ""+plantSucked;
        }

        ProgressBar cleaningProgressBar = uiDocument.rootVisualElement.Q<ProgressBar>("cleaning_progress");
        if (cleaningProgressBar != null)
        {
            cleaningProgressBar.value = GetCleaningPercentage();
        }
    }

    // Call this method whenever a piece of trash is cleaned
    public void IncreaseCleanedTrash()
    {
        cleanedTrash++;
    }

    // Returns the cleaning percentage
    public float GetCleaningPercentage()
    {
        return ((float)cleanedTrash / totalTrash) * 100;
    }

    public void IncreasePlantSucked()
    {
        plantSucked++;
    }

    public int GetPlantSucked()
    {
        return plantSucked;
    }

    public void Reset()
    {
        cleanedTrash = 0;
        plantSucked = 0;
    }
}