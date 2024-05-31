using UnityEngine;
using Utils;

public enum ModelRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY,
}

[CreateAssetMenu(fileName = "PlayerModelScriptableObject", menuName = "ScriptableObjects/PlayerModel")]
public class PlayerModelSO : ScriptableObject
{
    public string modelName;
    public string description;

    public float speed;
    public float rotationSpeed;
    public float storage;
    public float vision;
    public float reach;
    public float strength;
    public bool filterPlants;
    public ModelRarity rarity;
    public GameObject modelPrefab;

    public float getSpeed()
    {
        return Constants.Speed * speed;
    }

    public float getStorage()
    {
        return Constants.Storage * storage;
    }

    public float getReach()
    {
        return Constants.Reach * reach;
    }

    public float getVision()
    {
        return Constants.Vision * vision;
    }

    public float getStrength()
    {
        return Constants.Strength * strength;
    }

    public float getRotationSpeed()
    {
        return Constants.RotationSpeed * rotationSpeed;
    }
}
