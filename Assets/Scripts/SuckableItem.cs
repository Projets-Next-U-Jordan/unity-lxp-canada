using UnityEngine;

public class SuckableItem : MonoBehaviour
{
    private Collider itemCollider;

    void Start()
    {
        itemCollider = GetComponent<Collider>();
    }

    public void StartSucking()
    {
        // if (itemCollider != null)
        // {
        //     itemCollider.enabled = false;
        // }
    }

    public void StopSucking()
    {
        // if (itemCollider != null)
        // {
        //     itemCollider.enabled = true;
        // }
    }
}