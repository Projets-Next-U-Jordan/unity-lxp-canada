using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public enum VacuumUpgrades {
    Speed,
    Storage,
    Size,
    Reach,
    Vision,
    Strength
}

[Serializable]
public class PlayerSaveData
{
    public int money;
    public List<int> unlockedModels;
    public int lastInGameDay;
    public long lastSave;
    public int lastModelUsed;

    public PlayerSaveData() {
        money = 0;
        unlockedModels = new List<int>(){0};
        lastInGameDay = 0;
        lastModelUsed = -1;
    }
}

public class PlayerStateManager : SingletonPersistent<PlayerStateManager>
{
    public PlayerModelPoolSO modelPool;
    
    public PlayerModelSO currentModel;
    
    public PlayerSaveData playerData;
    public int upgradePoints = 0;
    
    private Dictionary<VacuumUpgrades, float> _upgradesMap = new Dictionary<VacuumUpgrades, float>();
    
    // Events
    public UnityEvent<PlayerModelSO> modelChangeEvent = new UnityEvent<PlayerModelSO>();
    
    protected override void Awake()
    {
        base.Awake();
        
        LoadData();
        
        if (playerData.lastModelUsed == -1) playerData.lastModelUsed = 0;
        SetModel(playerData.lastModelUsed);
        
        ResetUpgrades();
    }

    public void SetModel(int index)
    {
        if (modelPool.playerModels.Count < index)
        {
            Debug.LogWarning($"Model {index} does not exist !!");
        }

        PlayerModelSO wantedModel = modelPool.playerModels[index];
        
        if (playerData.unlockedModels.Contains(index))
        {
            currentModel = wantedModel;
            modelChangeEvent.Invoke(wantedModel);
            Debug.Log($"Player changed to {wantedModel.modelName}");
        }
        else
        {
            Debug.Log($"Player does not own model {wantedModel.modelName}");
        }
    }

    public float IncreaseUpgrade(VacuumUpgrades upgrade) {
        _upgradesMap[upgrade]++;
        upgradePoints--;
        return _upgradesMap[upgrade];
    }
    public float GetUpgradeLevel(VacuumUpgrades upgrade) {
        return _upgradesMap[upgrade];
    }
    public float DecreaseUpgrade(VacuumUpgrades upgrade) {
        _upgradesMap[upgrade]--;
        upgradePoints++;
        return _upgradesMap[upgrade];
    }

    public void ResetUpgrades() {
        _upgradesMap.Clear();
        foreach (VacuumUpgrades upgrade in Enum.GetValues(typeof(VacuumUpgrades))) {
            _upgradesMap.Add(upgrade, 0);
        }
    }

    private void SaveData()
    {
        playerData.lastSave = DateTime.Now.Ticks;
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerData", "");
        if (!string.IsNullOrEmpty(json)) {
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
            if (playerData == null)
                playerData = new PlayerSaveData();
            else if (playerData.unlockedModels == null || playerData.unlockedModels.Count == 0)
                playerData.unlockedModels = new List<int>() { 0 };
        } else
            playerData = new PlayerSaveData();
    }
    
}


