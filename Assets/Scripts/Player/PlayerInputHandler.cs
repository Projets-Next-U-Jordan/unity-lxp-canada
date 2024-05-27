using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    [Header("Inout Action Assets")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName;

    [Header("Action Name References")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string lookActionName = "Look";
    [SerializeField] private string suckActionName = "Suck";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction suckAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SuckInput { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeControls();
        }
    }
    
    private void InitializeControls()
    {
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(moveActionName);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(lookActionName);
        suckAction = playerControls.FindActionMap(actionMapName).FindAction(suckActionName);
        RegisterInputEvents();
    }

    private void RegisterInputEvents()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;
        suckAction.performed += context => SuckInput = true;
        suckAction.canceled += context => SuckInput = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        suckAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        suckAction.Disable();
    }



}
