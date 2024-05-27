using System;
using System.Collections.Generic;
using UnityEngine;

public enum VacuumUpgrades {
    SPEED,
    STORAGE,
    SIZE,
    REACH,
    VISION,
    STRENGTH
}

[Serializable]
public class PlayerSaveData {
    public int money;
    public int[] unlockedModels;
    public int lastIGDay = 0;
    public long lastSave = 0;

    public PlayerSaveData() {
        money = 0;
        unlockedModels = new int[1]{0};
        lastIGDay = 0;
    }
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public PlayerSaveData playerData;
    public int upgradePoints = 0;

    public Dictionary<VacuumUpgrades, float> upgradesMap = new Dictionary<VacuumUpgrades, float>();

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
    }

    void Start()
    {
        LoadData();
        ResetUpgrades();
    }

    public float IncreaseUpgrade(VacuumUpgrades upgrade) {
        upgradesMap[upgrade]++;
        upgradePoints--;
        return upgradesMap[upgrade];
    }

    public float GetUpgradeLevel(VacuumUpgrades upgrade) {
        return upgradesMap[upgrade];
    }

    public float DecreaseUpgrade(VacuumUpgrades upgrade) {
        upgradesMap[upgrade]--;
        upgradePoints++;
        return upgradesMap[upgrade];
    }

    void ResetUpgrades() {
        upgradesMap.Clear();
        foreach (VacuumUpgrades upgrade in Enum.GetValues(typeof(VacuumUpgrades))) {
            upgradesMap.Add(upgrade, 0);
        }
    }

    public void SaveData()
    {
        playerData.lastSave = DateTime.Now.Ticks;
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerData", "");
        if (!string.IsNullOrEmpty(json)) {
            playerData = JsonUtility.FromJson<PlayerSaveData>(json);
            if (playerData == null)
                playerData = new PlayerSaveData();
            else if (playerData.unlockedModels == null || playerData.unlockedModels.Length == 0)
                playerData.unlockedModels = new int[1]{0};
        } else
            playerData = new PlayerSaveData();
    }
}