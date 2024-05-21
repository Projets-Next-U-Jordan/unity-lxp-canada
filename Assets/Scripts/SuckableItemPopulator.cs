using UnityEngine;
using System.Collections.Generic;

public class SuckableItemPopulator : MonoBehaviour
{
    public GameObject trashPrefab;
    public GameObject[] plantPrefabs;
    public Vector3 spawnArea = new Vector3(10, 0, 10); // The area in which to spawn the items
    void Start()
    {
        Populate();
    }

    void Populate()
    {
        Vector3 position = transform.position;
        int itemCount = CleaningManager.Instance.totalTrash;
        List<Vector3> itemPositions = new List<Vector3>();
        float minDistanceBetweenItems = 1f; // Minimum distance between items

        for (int i = 0; i < itemCount; i++)
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
            }
            while (!positionIsValid);

            itemPositions.Add(spawnPosition);

            // var trashPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

            GameObject item = Instantiate(trashPrefab, spawnPosition, Quaternion.identity);
            item.layer = LayerMask.NameToLayer("TrashItems");
        }

        // Do the same for plants
        int plantCount = Random.Range(itemCount/3, itemCount/2);
        for (int i = 0; i<plantCount; i++) {
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
            }
            while (!positionIsValid);

            itemPositions.Add(spawnPosition);

            var plantPrefab = plantPrefabs[Random.Range(0, plantPrefabs.Length)];

            GameObject item = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
            item.layer = LayerMask.NameToLayer("PlantItems");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a cube gizmo to visualize the spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}