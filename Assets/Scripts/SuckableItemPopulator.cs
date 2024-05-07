using UnityEngine;

public class SuckableItemPopulator : MonoBehaviour
{
    public GameObject suckableItemPrefab; // The SuckableItem prefab
    public int itemCount = 10; // The number of items to spawn
    public Vector3 spawnArea = new Vector3(10, 0, 10); // The area in which to spawn the items

    void Start()
    {
        Populate();
    }

    void Populate()
    {
        for (int i = 0; i < itemCount; i++)
        {
            // Calculate a random position within the spawn area
            Vector3 position = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                0,
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            // Instantiate a new SuckableItem at the calculated position
            GameObject item = Instantiate(suckableItemPrefab, position, Quaternion.identity);
            item.layer = LayerMask.NameToLayer("SuckableItems");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a cube gizmo to visualize the spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}