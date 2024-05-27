using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VacuumModel
{
    public int id;
    public string name;
    public string description;
    public float speed;
    public float rotationSpeed;
    public float storage;
    public float size;
    public float range;
    public float vision;
    public float suckingPower;
    public bool filterPlants;
    public string modelPath;

    public override string ToString()
    {
        return $"VacuumModel: {{ Name: {name}, Description: {description}, Speed: {speed}, Storage: {storage}, Size: {size}, Range: {range}, Vision: {vision}, SuckingPower: {suckingPower}, ModelPath: {modelPath} }}";
    }
}

[System.Serializable]
public class VacuumModels
{
    public VacuumModel[] models;
}

public class ModelManager : MonoBehaviour
{
    public static ModelManager Instance { get; private set; }

    public List<VacuumModel> Models { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadModels();
        }
    }

    void LoadModels()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("Data/vacuums");
        if (jsonText == null)
        {
            Debug.LogError("Failed to load Data/vacuums");
            return;
        }
        VacuumModels vacuumModels = JsonUtility.FromJson<VacuumModels>(jsonText.text);
        Models = new List<VacuumModel>(vacuumModels.models);
    }
}