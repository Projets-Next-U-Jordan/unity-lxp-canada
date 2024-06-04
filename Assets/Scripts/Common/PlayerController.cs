using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler _playerInputHandler;
    private CharacterController _characterController;
    private PlayerStateManager _playerStateManager;
    private CleaningManager _cleaningManager;

    private GameObject currentModelObject;
    
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float terrainAdaptSpeed = 200f;
    [SerializeField] private float suckingAngle = 45f;

    private Collider suckingCollider;
    
    private bool isSucking = false;

    private void OnEnable()
    {
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _playerInputHandler = PlayerInputHandler.Instance;
        _playerStateManager = PlayerStateManager.Instance;
        _cleaningManager = CleaningManager.Instance;
        _playerStateManager.modelChangeEvent.AddListener(OnModelChange);
        if (_playerStateManager.currentModel != null)
            OnModelChange(_playerStateManager.currentModel);
    }

    private void OnDestroy()
    {
        _playerStateManager.modelChangeEvent.RemoveListener(OnModelChange);
    }

    void Update()
    {
        HandleMovement();
        HandleSucking();
        HandleTerrain();
        HandleOther();
    }
    
    private void OnModelChange(PlayerModelSO newModel)
    {
        Debug.Log(newModel.modelName);
        if (currentModelObject != null)
        {
            VacuumModel previousVacuumModel = currentModelObject.GetComponent<VacuumModel>();
            if (previousVacuumModel)
                previousVacuumModel.onItemSuck -= HandleItemSuck;
            Destroy(currentModelObject);
        }
        currentModelObject = Instantiate(newModel.modelPrefab, transform.position, transform.rotation);
        currentModelObject.transform.SetParent(transform);

        VacuumModel vacuumModel = currentModelObject.GetComponent<VacuumModel>();

        if (vacuumModel)
        {
            vacuumModel.onItemSuck += HandleItemSuck;
            suckingCollider = vacuumModel.suckingCollider;
        }
    }
    
    private void HandleMovement()
    {
        float speed = _playerStateManager.currentModel.getSpeed();
        Vector2 moveInput = _playerInputHandler.MoveInput;

        if (Mathf.Abs(moveInput.y) < 0.25f)
            moveInput.y = 0;

        float forwardSpeed = moveInput.y * speed;
        Vector3 movement = transform.forward * forwardSpeed * Time.deltaTime;
        movement.y = -gravity * Time.deltaTime;

        _characterController.Move(movement);

        float rotation = moveInput.x * _playerStateManager.currentModel.getRotationSpeed() * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void HandleSucking()
    {
        isSucking = _playerInputHandler.SuckInput;
        if (suckingCollider)
            suckingCollider.enabled = isSucking;
        if (isSucking)
            SuckItems();
    }
    
    void SuckItems()
    {
        SuckableItem[] items = FindObjectsOfType<SuckableItem>();
        Vector3 triggerPosition = suckingCollider.transform.position;

        float range = _playerStateManager.currentModel.getReach();
        float strength = _playerStateManager.currentModel.getStrength();
        
        foreach (SuckableItem item in items)
        {
            Vector3 directionToItem = item.transform.position - transform.position;
            float distanceToItem = directionToItem.magnitude;
            float angle = Vector3.Angle(transform.forward, directionToItem);

            bool canSuck = _cleaningManager.canSuckItem(item);
            
            if (isSucking && distanceToItem <= range && angle <= suckingAngle && canSuck)
            {
                float step = Mathf.Clamp(strength-item.data.weight,1,strength) * Time.deltaTime;
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

    private void HandleTerrain()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Quaternion toRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            Quaternion smoothedRotation = Quaternion.RotateTowards(transform.rotation, toRotation * transform.rotation, terrainAdaptSpeed * Time.deltaTime);
            transform.rotation = smoothedRotation;
        }
    }
    
    public void TeleportPlayer(Vector3 position)
    {
        _characterController.enabled = false;
        transform.position = position;
        _characterController.enabled = true;
    }

    private void HandleOther()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int currentModelIndex = _playerStateManager.modelPool.playerModels.FindIndex((pm) => pm == _playerStateManager.currentModel) + 1;
            
            if (currentModelIndex >= _playerStateManager.modelPool.playerModels.Count) // Assuming 'models' is the list of your models
                currentModelIndex = 0;

            _playerStateManager.SetModel(currentModelIndex);
        }
    }

    private void HandleItemSuck(SuckableItem suckableItem)
    {
        _cleaningManager.suckItem(suckableItem);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (isSucking) ? Color.green :  Color.red;
        if (_playerStateManager != null)
        {
            float range = _playerStateManager.currentModel.getReach();
            DrawCone(suckingCollider.transform.position, transform.forward, range, suckingAngle);
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
