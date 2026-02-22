using UnityEngine;

public class ItemWorldObject : MonoBehaviour
{
    public ItemData itemData;
    private GameObject currentVisual;

    public void Setup(ItemData data)
    {
        itemData = data;

        // Remove old visual if it exists (for switching logic)
        if (currentVisual != null) Destroy(currentVisual);

        // Instantiate the visual model defined in the ScriptableObject
        if (itemData.prefab != null)
        {
            currentVisual = Instantiate(
                itemData.prefab,
                transform.position,
                transform.rotation,
                transform
            );
        }

        gameObject.name = "Item_" + itemData.itemName;
    }
}