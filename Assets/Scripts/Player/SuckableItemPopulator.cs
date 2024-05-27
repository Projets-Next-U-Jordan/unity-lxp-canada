using UnityEngine;
using System.Collections.Generic;

public class SuckableItemPopulator : MonoBehaviour
{
    public GameObject trashPrefab;
    public GameObject plantPrefab;
    public Vector3 spawnArea = new Vector3(10, 0, 10); // The area in which to spawn the items

    private List<GameObject> Items = new List<GameObject>();
    
    public float minDistanceBetweenItems = 5f;

    public void Populate(int trash, int plant, bool reset = true) {
        if (reset) {
            foreach (GameObject item in Items) { Destroy(item); }
            Items = new List<GameObject>();
        }

        Vector3 position = transform.position;
        List<Vector3> itemPositions = new List<Vector3>();

        SpawnPrefab(trashPrefab, trash, position, itemPositions, "TrashItems");
        SpawnPrefab(plantPrefab, plant, position, itemPositions, "PlantItems");
    }

    void Populate()
    {
        Vector3 position = transform.position;
        int itemCount = CleaningManager.Instance.totalTrash;
        List<Vector3> itemPositions = new List<Vector3>();

        SpawnPrefab(trashPrefab, itemCount, position, itemPositions, "TrashItems");

        int plantCount = Random.Range(itemCount/3, itemCount/2);
        SpawnPrefab(plantPrefab, plantCount, position, itemPositions, "PlantItems");
    }

    void OnDrawGizmosSelected()
    {
        // Draw a cube gizmo to visualize the spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }

    public void Reset()
    {
        foreach (GameObject item in Items)
        {
            Destroy(item);
        }
        Items = new List<GameObject>();
    }

    private void SpawnPrefab(GameObject prefab, int count, Vector3 position, List<Vector3> itemPositions, string layerName = "")
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition;
            bool positionIsValid;

            do
            {
                spawnPosition = new Vector3(
                    position.x + Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                    position.y,
                    position.z + Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
                );

                positionIsValid = true;

                foreach (Vector3 itemPosition in itemPositions)
                {
                    if (Vector3.Distance(spawnPosition, itemPosition) < minDistanceBetweenItems)
                    {
                        positionIsValid = false;
                        break;
                    }
                }
            } while (!positionIsValid);

            itemPositions.Add(spawnPosition);

            GameObject item = Instantiate(prefab, spawnPosition, Quaternion.identity);
            item.layer = LayerMask.NameToLayer(layerName);
            Items.Add(item);
        }
    }
}