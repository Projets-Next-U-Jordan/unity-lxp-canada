using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Attributes;

[Serializable]
public class DemoRequirement
{
    [SerializeField]
    public bool spawnItems;

    [SerializeField, ConditionalHide(true, "spawnItems=true")]
    public SuckableItemPopulator itemPopulator;
    
    [SerializeField, ConditionalHide(true, "spawnItems=true")]
    public bool allowTrash;

    [SerializeField, ConditionalHide(true, "spawnItems=true", "allowTrash=true")]
    public int neededTrash;

    [SerializeField, ConditionalHide(true, "spawnItems=true")]
    public bool allowPlant;

    [SerializeField, ConditionalHide(true, "spawnItems=true", "allowPlant=true")]
    public int neededPlant;

    public bool isMet() {
        if (spawnItems) {
            CleaningManager cleaningManager = CleaningManager.Instance;
            if (cleaningManager == null) return false;
            if (itemPopulator == null) return false;
            if (allowTrash && cleaningManager.cleanedTrash < neededTrash) return false;
            if (allowPlant && cleaningManager.plantSucked < neededPlant) return false;
        }
        return true;
    }

    public bool shouldReset() {
        if (spawnItems) {
            CleaningManager cleaningManager = CleaningManager.Instance;
            // Debug.Log("=====================================");
            // Debug.Log(cleaningManager == null);
            if (cleaningManager == null) return false;
            // Debug.Log(itemPopulator == null);
            if (itemPopulator == null) return false;
            // Debug.Log(!allowTrash && cleaningManager.cleanedTrash > 0);
            if (!allowTrash && cleaningManager.cleanedTrash > 0) return true;
            // Debug.Log(cleaningManager.plantSucked);
            if (!allowPlant && cleaningManager.plantSucked > 0) return true;
        }
        return false;
    }
}