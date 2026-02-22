using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Pool")]
    public List<ItemData> itemPool;
    public GameObject baseItemPrefab; // A generic prefab with the ItemController script

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints;

    void Start()
    {
        SpawnRandomItems();
    }

    public void SpawnRandomItems()
    {
        if (spawnPoints.Count == 0 || itemPool.Count == 0) return;

        foreach (Transform point in spawnPoints)
        {
            // Pick a random item from the pool
            ItemData randomData = itemPool[Random.Range(0, itemPool.Count)];

            // Spawn the base container
            GameObject newItem = Instantiate(
                baseItemPrefab,
                point.position,
                point.rotation
            );

            // Initialize the item with the random data
            newItem.GetComponent<ItemWorldObject>().Setup(randomData);
        }
    }
}