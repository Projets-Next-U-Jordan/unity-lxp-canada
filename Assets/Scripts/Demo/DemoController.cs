using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DemoScenario {
    public string name;
    public string description;
    public GameObject demoScene;
    public Transform playerPosition;
    public DemoGoal goal;
    public int trashCount;
    public int plantCount;
    public DemoRequirement requirement;

    public DemoScenario(
        string name,
        string description,
        int trashCount,
        int plantCount,
        Transform playerPosition,
        DemoRequirement requirement,
        DemoGoal goal
    ) {
        this.name = name;
        this.description = description;
        this.trashCount = trashCount;
        this.plantCount = plantCount;
        this.playerPosition = playerPosition;
        this.requirement = requirement;
        this.goal = goal;
    }
}

public class DemoController : MonoBehaviour
{
    
    [SerializeField] private PlayerController playerController;
    
    public int currentScenario = 0;
    public DemoScenario[] scenarios;

    void Start()
    {
        foreach (DemoScenario scenario in scenarios) {
            Debug.Log(scenario.name);
            if (scenario.demoScene)
                scenario.demoScene.gameObject.SetActive(false);
        }
        LoadScenario(currentScenario, currentScenario);
    }

    void Update()
    {
        if (scenarios[currentScenario].requirement.shouldReset()) {
            ResetScenario();
        }
        if (scenarios[currentScenario].goal)
            scenarios[currentScenario].goal.gameObject.SetActive(scenarios[currentScenario].requirement.isMet());
    }

    public void LoadScenario(int previousIndex, int index) {
        DemoScenario previousScenario = scenarios[previousIndex];
        if (previousScenario != null && previousIndex != index) {
            if (previousScenario.demoScene)
                previousScenario.demoScene.SetActive(false);
            if (previousScenario.requirement.spawnItems) {
                previousScenario.requirement.itemPopulator.PopulateItems();
            }
        }

        if (index < scenarios.Length) {
            DemoScenario scenario = scenarios[index];
            if (scenario.demoScene)
                scenario.demoScene.SetActive(true);
            // player.transform.position = scenario.playerPosition.position;
            if (scenario.playerPosition && playerController)
                playerController.TeleportPlayer(scenario.playerPosition.position);
            if (scenario.requirement.itemPopulator)
            {
                scenario.requirement.itemPopulator.trashNumber = scenario.trashCount;
                scenario.requirement.itemPopulator.plantNumber = scenario.plantCount;
                scenario.requirement.itemPopulator.PopulateItems();
            }
            CleaningManager.Instance.Reset();
        }
    }

    public void NextScenario() {
        if (currentScenario+1 >= scenarios.Length) return;
        int _prev = currentScenario;
        currentScenario++;
        LoadScenario(_prev, currentScenario);
    }

    public void ResetScenario() {
        LoadScenario(currentScenario, currentScenario);
    }
}
