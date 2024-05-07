using UnityEngine;

public class SuckableItem : MonoBehaviour
{
    private Collider itemCollider;
    private Rigidbody itemRigidbody;

    [SerializeField]
    public bool isTrash = false;

    void Start()
    {
        itemCollider = GetComponent<Collider>();
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public void StartSucking()
    {
        if (itemRigidbody != null)
            itemRigidbody.isKinematic = true;
    }

    public void StopSucking()
    {
        if (itemRigidbody != null)
            itemRigidbody.isKinematic = false;
    }
}