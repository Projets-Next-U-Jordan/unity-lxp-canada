using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 1;
    public float minY = -20f;
    public float maxY = 80f;
    public float distance = 10f;

    private float mouseX, mouseY;

    private PlayerInputHandler _playerInputHandler;

    private void OnEnable()
    {
    }

    void Start()
    {
        _playerInputHandler = PlayerInputHandler.Instance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = player.transform.rotation.eulerAngles.y;
        mouseY = 40;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += _playerInputHandler.LookInput.x * rotationSpeed;
        mouseY -= _playerInputHandler.LookInput.y * rotationSpeed;
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