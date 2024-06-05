using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utils;
using Random = UnityEngine.Random;

public class CleaningManager : SingletonPersistent<CleaningManager>
{
    [Header("Timer")]
    public DemoTimer.CountdownFormatting countdownFormatting = DemoTimer.CountdownFormatting.DaysHoursMinutesSeconds;
    public bool showMilliseconds = true;
    public double countdownTime = 600;
    double countdownInternal;
    bool countdownOver = false;
    public SceneField sceneToLoad;
    private Label timerLabel;
    

    private DualProgressBar _dualProgressBar;

    private DemoTimer _demoTimer;
    
    private PlayerStateManager _playerStateManager;
    [Header("Other")]
    public TrashPoolSO trashPoolSo;
    public PlantPoolSO plantPoolSo;

    public int trashAmount = 0;
    public List<GameObject> trash = new List<GameObject>();

    public int trashSucked = 0;
    public int plantSucked = 0;
    
    public float heldTrash = 0f;
    public float heldPlants = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        _dualProgressBar = GetComponent<DualProgressBar>();
        _demoTimer = GetComponent<DemoTimer>();
    }
    
    private void Start()
    {
        countdownInternal = countdownTime;
        _playerStateManager = PlayerStateManager.Instance;    
    }

    private void Update()
    {
        CountDown();
    }

    private float maxStorage()
    {
        return _playerStateManager.currentModel.getStorage();
    }

    public void SpawnPlant(Vector3 pos)
    {
        List<SuckableItemSO> items = new List<SuckableItemSO>(plantPoolSo.plants);
        SuckableItemSO item = GetRandomItemByWeight(items);
        GameObject itemObject = Instantiate(item.prefab, pos, Quaternion.identity);
        itemObject.tag = "Plant";
        itemObject.layer = LayerMask.NameToLayer("Suckable");
        SuckableItem suckableItem = itemObject.AddComponent<SuckableItem>();
        suckableItem.data = item;
    }

    public void SpawnTrash(Vector3 pos)
    {
        List<SuckableItemSO> items = new List<SuckableItemSO>(trashPoolSo.trash);
        SuckableItemSO item = GetRandomItemByWeight(items);
        GameObject itemObject = Instantiate(item.prefab, pos, Quaternion.identity);
        itemObject.tag = "Trash";
        itemObject.layer = LayerMask.NameToLayer("Suckable");
        SuckableItem suckableItem = itemObject.AddComponent<SuckableItem>();
        suckableItem.data = item;
    }
    
    public bool canSuckItem(SuckableItem suckableItem)
    {
        if (suckableItem.data == null) return false;
        
        return (heldPlants + heldTrash) + suckableItem.data.weight <= maxStorage();
    }
    

    public void suckItem(SuckableItem suckableItem)
    {
        if (!canSuckItem(suckableItem)) return;
        bool isTrash = suckableItem.gameObject.CompareTag("Trash");
        if (isTrash)
        {
            heldTrash += suckableItem.data.weight;
            trash.Remove(suckableItem.gameObject);
        }
        else
        {
            heldPlants += suckableItem.data.weight;
        }
        
        UpdateProgressBar();
        Destroy(suckableItem.gameObject);
    }
    

    
    public SuckableItemSO GetRandomItemByWeight(List<SuckableItemSO> items)
    {
        int totalWeight = items.Sum(item => item.spawnWeight);
        int randomValue = Random.Range(0, totalWeight);
        int runningTotal = 0;

        foreach (var item in items)
        {
            runningTotal += item.spawnWeight;
            if (randomValue < runningTotal)
            {
                return item;
            }
        }
        return items[0];
    }

    void UpdateProgressBar()
    {
        _dualProgressBar.SetProgress(heldPlants, heldTrash);
    }

    public void Reset()
    {
        foreach (GameObject trash in trash)
        {
            Destroy(trash);
        }
        trash.Clear();

        trashAmount = 0;
        trashSucked = 0;
        heldTrash = 0;

        plantSucked = 0;
        heldPlants = 0;
        UpdateProgressBar();


    }


    private void CountDown()
    {
        if(countdownInternal > 0) {
            countdownInternal -= Time.deltaTime;
            if(countdownInternal < 0) {
                countdownInternal = 0;
            }
            _demoTimer.SetTime(countdownInternal, countdownFormatting, showMilliseconds);

            if (countdownInternal <= 15)
            {
                Color toColor = Color.Lerp(Color.white, Color.red, Mathf.Sin(Time.time * 5));
                _demoTimer.SetColor(toColor);
            }
        }
        else {
            if(!countdownOver) {
                countdownOver = true;
                Debug.Log("Countdown Over!");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
