using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class CleaningManager : SingletonPersistent<CleaningManager>
{
    
    private DualProgressBar _dualProgressBar;

    private RemainingTrash _remainingTrash;
    
    private PlayerStateManager _playerStateManager;

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
        _remainingTrash = GetComponent<RemainingTrash>();
    }
    
    private void Start()
    {
        _playerStateManager = PlayerStateManager.Instance;    
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
        trash.Add(itemObject);
        _remainingTrash.SetProgress(trash.Count);
    }
    
    public bool canSuckItem(SuckableItem suckableItem)
    {
        if (suckableItem.data == null) return false;
        
        Debug.Log($"{(heldPlants + heldTrash) + suckableItem.data.weight} <= {maxStorage()}");
        
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
        _remainingTrash.SetProgress(trash.Count);
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
        _dualProgressBar.SetMaxProgress(maxStorage());
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
    
}
