using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float playerRadius;
    [SerializeField] private float playerHeight;
    [SerializeField] private float movSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Animator playerAnimator;

    [HideInInspector] public GameObject heldCell;
    
    public GameObject holdPoint;

    public event EventHandler OnInteractPerformed;
    public event EventHandler OnPausePerformed;

    private GameInputSystem gameInputSystem;
    private bool canMove;
    public bool isMoving;
    public static PlayerInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameInputSystem = new GameInputSystem();
        gameInputSystem.GameMap.Enable();
        gameInputSystem.GameMap.Interact.performed += Interact_performed;
        gameInputSystem.GameMap.Pause.performed += Pause_performed;
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPausePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        
        isMoving = false;
        //Movement
        
        Vector2 readVec = gameInputSystem.GameMap.Movement.ReadValue<Vector2>();
        Vector3 movVec = new Vector3(readVec.x,0,readVec.y);

        Quaternion cameraRot = Camera.main.transform.rotation;
        float cameraAngle;
        Vector3 cameraAxis;
        cameraRot.ToAngleAxis(out cameraAngle, out cameraAxis);
        

        Quaternion cameraRotFlat = Quaternion.Euler(0,cameraAngle*cameraAxis.y,0);

        movVec = cameraRotFlat * movVec;
        float movDist = movSpeed * Time.deltaTime;

        
        bool canMove = !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, playerRadius, movVec, movDist);

        this.transform.forward = Vector3.Slerp(this.transform.forward, movVec, Time.deltaTime* turnSpeed);

        if (!canMove)
        {
            Vector3 movVecX = new Vector3(movVec.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, playerRadius, movVecX, movDist);
            if (canMove)
            {
                movVec = movVecX;
            }
        }

        if (canMove)
        {
            this.transform.position += movVec * movSpeed * Time.deltaTime;
            if(movVec.magnitude>0 && Time.timeScale>0) isMoving = true;
        }

        //Animation
        
        if (readVec.magnitude > 0)
        {
            
            playerAnimator.SetBool("IsWalking", true);
           
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }
        
        
    }
    private void OnDisable()
    {
        gameInputSystem.GameMap.Interact.performed -= Interact_performed;
        gameInputSystem.GameMap.Pause.performed -= Pause_performed;
    }
}
