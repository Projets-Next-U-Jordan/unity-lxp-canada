using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public VacuumModel CurrentModel;
    public float GarbagePoints;
    public float rotationSpeed = 100.0f;
    public float maxAngle = 45f; 

    void SetModel(VacuumModel model)
    {
        CurrentModel = model;

        GameObject modelPrefab = Resources.Load<GameObject>(model.modelPath);
        if (modelPrefab == null)
        {
            Debug.LogError($"Failed to load model at {model.modelPath}");
            return;
        }

        GameObject modelInstance = Instantiate(modelPrefab, transform.position, transform.rotation);

        modelInstance.transform.SetParent(transform);

        Debug.Log($"Set model to {model.name}");
    }

    void Start()
    {
        if (ModelManager.Instance.Models.Count > 0) { SetModel(ModelManager.Instance.Models[0]); }
        else { Debug.Log("No models available"); }

        GameObject triggerObject = new GameObject("TriggerObject");
        triggerObject.transform.SetParent(transform);
        triggerObject.transform.localPosition = new Vector3(0, 0, 0.8f); // Change this value to adjust the position

        SphereCollider sphereCollider = triggerObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 0.8f;

    }

    void Update()
    {
        PlayerMovement();
        SuckItems();
    }

    void OnDrawGizmos()
    {
        // Draw a ray in the direction the player is facing
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * CurrentModel.range);

        // Draw a sphere at the maximum distance
        Gizmos.color = Color.blue;
        DrawCone(transform.position, transform.forward, CurrentModel.range, maxAngle);
    }

    void SuckItems()
    {
        SuckableItem[] items = FindObjectsOfType<SuckableItem>();

        foreach (SuckableItem item in items)
        {
            Vector3 directionToItem = item.transform.position - transform.position;
            float distanceToItem = directionToItem.magnitude;
            float angle = Vector3.Angle(transform.forward, directionToItem);

            // Check if the item is within the distance and angle
            if (directionToItem.magnitude <= CurrentModel.range && angle <= maxAngle)
            {
                float step = CurrentModel.suckingPower * Time.deltaTime;
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, step);
                item.StartSucking();
            }
            else
            {
                item.StopSucking();
            }
        }
    }

    void PlayerMovement()
    {
        float move = Input.GetAxis("Vertical") * CurrentModel.speed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, move);
        transform.Rotate(0, rotate, 0);
    }

    void DrawCone(Vector3 position, Vector3 direction, float range, float angle)
    {
        // Calculate the number of lines to draw for the cone
        int numLines = 20;
        float angleStep = angle * 2 / numLines;

        // Draw each line
        for (int i = 0; i <= numLines; i++)
        {
            float currentAngle = -angle + angleStep * i;
            Vector3 lineDirection = Quaternion.Euler(0, currentAngle, 0) * direction;
            Gizmos.DrawRay(position, lineDirection * range);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the other object is a SuckableItem
        SuckableItem item = other.GetComponent<SuckableItem>();
        if (item != null)
        {
            // Destroy the item
            Destroy(item.gameObject);
        }
    }

}