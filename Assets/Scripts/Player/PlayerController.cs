using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public int defaultModelId = 0;
    public VacuumModel CurrentModel;
    public float maxAngle = 45f;
    private bool isSucking = false;
    private Rigidbody rb;
    private SphereCollider triggerCollider = null;
    private int currentModelIndex = 0; // Add this line at the top of your script

    private GameObject modelInstance;


    // Player stats
    public float speed = 0;
    public float range = 0;
    public float suckingPower = 0;
    public bool filterPlants = false;


    void SetModel(VacuumModel model)
    {
        CurrentModel = model;

        GameObject modelPrefab = Resources.Load<GameObject>(model.modelPath);
        if (modelPrefab == null)
        {
            Debug.LogError($"Failed to load model at {model.modelPath}");
            return;
        }

        if (modelInstance != null)
        {
            Destroy(modelInstance);
        }

        modelInstance = Instantiate(modelPrefab, transform.position, transform.rotation);

        modelInstance.transform.SetParent(transform);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (ModelManager.Instance.Models.Count > 0) { 
            if (defaultModelId < ModelManager.Instance.Models.Count)
                SetModel(ModelManager.Instance.Models[defaultModelId]);
            else
                SetModel(ModelManager.Instance.Models[0]); 
        }
        else { Debug.Log("No models available"); }

        GameObject triggerObject = new GameObject("TriggerObject");
        triggerObject.transform.SetParent(transform);
        triggerObject.transform.localPosition = new Vector3(0, 0, 0f); // Change this value to adjust the position

        triggerCollider = triggerObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.8f;
    }

    void Update()
    {
        PlayerMovement();
        isSucking = Input.GetMouseButton(0);
        SuckItems();

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        maxAngle += scrollInput * 10;

        maxAngle = Mathf.Clamp(maxAngle, 0, 180);

        triggerCollider.enabled = isSucking;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentModelIndex++;
            if (currentModelIndex >= ModelManager.Instance.Models.Count) // Assuming 'models' is the list of your models
            {
                currentModelIndex = 0;
            }

            if (currentModelIndex < ModelManager.Instance.Models.Count)
            {
                SetModel(ModelManager.Instance.Models[currentModelIndex]);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (triggerCollider != null) {
            Vector3 triggerPosition = triggerCollider.transform.position;
            Gizmos.color = (isSucking) ? Color.green : Color.red;
            DrawCone(triggerPosition, transform.forward, CurrentModel.range, maxAngle);
        }
    }

    void SuckItems()
    {
        SuckableItem[] items = FindObjectsOfType<SuckableItem>();
        Vector3 triggerPosition = triggerCollider.transform.position;

        foreach (SuckableItem item in items)
        {
            if (CurrentModel.filterPlants && !item.isTrash) { continue; }
            Vector3 directionToItem = item.transform.position - transform.position;
            float distanceToItem = directionToItem.magnitude;
            float angle = Vector3.Angle(transform.forward, directionToItem);

            if (isSucking && directionToItem.magnitude <= CurrentModel.range && angle <= maxAngle)
            {
                float step = CurrentModel.suckingPower * Time.deltaTime;
                Vector3 targetPosition = new Vector3(triggerPosition.x, item.transform.position.y, triggerPosition.z);
                item.transform.position = Vector3.MoveTowards(item.transform.position, targetPosition, step);
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
        float move = Input.GetAxis("Vertical") * CurrentModel.speed;
        float rotate = Input.GetAxis("Horizontal") * CurrentModel.rotationSpeed;

        Vector3 movement = transform.forward * move;
        movement.y = rb.velocity.y; // Preserve the y-component of the velocity

        rb.velocity = movement;

        if (rotate != 0) transform.Rotate(0, rotate, 0);
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
        SuckableItem item = other.GetComponent<SuckableItem>();
        if (item != null)
        {
            Destroy(item.gameObject);
            if (item.isTrash)
                CleaningManager.Instance.IncreaseCleanedTrash();
            else
                CleaningManager.Instance.IncreasePlantSucked();
        }
    }

}