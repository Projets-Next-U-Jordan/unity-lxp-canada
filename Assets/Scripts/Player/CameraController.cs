using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 1;
    public float minY = -20f;
    public float maxY = 80f;
    public float distance = 10f;

    private float mouseX, mouseY;

    private PlayerInputHandler inputHandler;

    void Awake()
    {
        inputHandler = PlayerInputHandler.Instance;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = player.transform.rotation.eulerAngles.y;
        mouseY = 40;
    }

    void LateUpdate()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += inputHandler.LookInput.x * rotationSpeed;
        mouseY -= inputHandler.LookInput.y * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + player.transform.position;

        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit hit;
        Vector3 raycastOrigin = player.transform.position + new Vector3(0, 1, 0); // Add a small offset to the raycast origin
        if (Physics.Raycast(raycastOrigin, position - raycastOrigin, out hit, distance, layerMask))
        {
            position = hit.point + (hit.normal * 0.5f); // Add a small offset based on the collision normal
        }

        transform.position = position;
        transform.LookAt(player.transform);
    }

}