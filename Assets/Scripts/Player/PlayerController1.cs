using UnityEngine;

public class PlayerController1 : MonoBehaviour
{

    public int defaultModelId = 0;
    public VacuumModel CurrentModel;
    public float maxAngle = 45f;
    private int currentModelIndex = 0;
    [SerializeField] private float gravity = 9.81f;
    private GameObject modelInstance;

    private float rotationSpeed = 100.0f;

    // Player stats
    public float speed = 0;
    public float range = 0;
    public float suckingPower = 0;
    public bool filterPlants = false;
    private bool isSucking = false;
    private SphereCollider triggerCollider;

    public CharacterController characterController { get; private set;}
    private PlayerInputHandler inputHandler;
    private Vector3 currentMovement = Vector3.zero;



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
        characterController = GetComponent<CharacterController>();
        inputHandler = PlayerInputHandler.Instance;

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
        triggerObject.layer = LayerMask.NameToLayer("Triggers");
        
        triggerCollider = triggerObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = 0.8f;
    }

    void Update() {
        HandleMovement();
        HandleSucking();
        HandlerOther();
    }

    void HandleMovement() 
    {
        float speed = CurrentModel.speed;
        Vector2 moveInput = inputHandler.MoveInput;

        if (Mathf.Abs(moveInput.y) < 0.25f)
            moveInput.y = 0;

        float forwardSpeed = moveInput.y * speed;
        Vector3 movement = transform.forward * forwardSpeed * Time.deltaTime;
        movement.y -= gravity * Time.deltaTime;

        characterController.Move(movement);

        float rotation = moveInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }
    void HandleSucking() {
        isSucking = inputHandler.SuckInput;
        triggerCollider.enabled = isSucking;
        SuckItems();
    }

    void HandlerOther() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Quaternion toRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            Quaternion smoothedRotation = Quaternion.RotateTowards(transform.rotation, toRotation * transform.rotation, rotationSpeed * Time.deltaTime);
            transform.rotation = smoothedRotation;
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        maxAngle += scrollInput * 10;

        maxAngle = Mathf.Clamp(maxAngle, 0, 180);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentModelIndex++;
            if (currentModelIndex >= ModelManager.Instance.Models.Count) // Assuming 'models' is the list of your models
                currentModelIndex = 0;

            if (currentModelIndex < ModelManager.Instance.Models.Count)
                SetModel(ModelManager.Instance.Models[currentModelIndex]);
        }
    }

    public void TeleportPlayer(Vector3 position)
    {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
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

    // GIZMOS
    void OnDrawGizmos()
    {
        if (triggerCollider != null) {
            Vector3 triggerPosition = triggerCollider.transform.position;
            Gizmos.color = (isSucking) ? Color.green : Color.red;
            DrawCone(triggerPosition, transform.forward, CurrentModel.range, maxAngle);
        }
    }

    void DrawCone(Vector3 position, Vector3 direction, float range, float angle)
    {
        int numLines = 20;
        float angleStep = angle * 2 / numLines;

        for (int i = 0; i <= numLines; i++)
        {
            float currentAngle = -angle + angleStep * i;
            Vector3 lineDirection = Quaternion.Euler(0, currentAngle, 0) * direction;
            Gizmos.DrawRay(position, lineDirection * range);
        }
    }

}