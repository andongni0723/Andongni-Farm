using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;
    private bool isMoving;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool inputDisable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    private void Update()
    {
        if (!inputDisable)
            PlayerInput();
        else
            isMoving = false;
  
        SwitchAnimator();
    }

    private void FixedUpdate()
    {
        if(!inputDisable)
            Movement();
    }

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX != 0 && inputY != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }

        // Key Down Change Speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }

        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimator()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("inputX", inputX);
                anim.SetFloat("inputY", inputY);
            }
        }
    }
}
