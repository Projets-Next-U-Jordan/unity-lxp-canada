using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 1;
    public float minY = -20f;
    public float maxY = 80f;
    public float distance = 10f;
    private float mouseX, mouseY;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = 0;
        mouseY = 40;
    }

    void LateUpdate()
    {
        CamControl();
    }

    void CamControl()
    {

        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            mouseX = 0;
            mouseY = 40;
        }

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, minY, maxY);

        // Calculate the rotation around the player
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + player.transform.position;

        // Check if there's anything between the camera and the player
        Vector3 directionFromPlayer = position - player.transform.position;
        if (directionFromPlayer.magnitude < distance)
        {
            position = player.transform.position + directionFromPlayer.normalized * distance;
        }

        transform.position = position;
        transform.LookAt(player.transform);
    }
}