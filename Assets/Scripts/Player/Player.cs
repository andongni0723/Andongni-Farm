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

    // Use tool animator
    private float mouseX;
    private float mouseY;
    private bool useTool;

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
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;

    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        if(useTool) return;
        
        //TODO:Event animation
        if(itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - transform.position.y;

            if(Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else 
                mouseX = 0;

            StartCoroutine(UseToolRoutime(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }

    }

    private IEnumerator UseToolRoutime(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = false;
        inputDisable = true;
        yield return null;

        foreach(var anim in animators)
        {
            anim.SetTrigger("useTool");
            // Player rotate direction
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        // Wait animator done
        useTool = false;
        inputDisable = false;

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
        if (!inputDisable)
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
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);

            if (isMoving)
            {
                anim.SetFloat("inputX", inputX);
                anim.SetFloat("inputY", inputY);
            }
        }
    }
}
