using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnFarm.AStar;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    private string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;

    public string StartScene { set => currentScene = value; }

    [Header("Movement Details")]
    public float normalSpeed = 2f;
    private float minSpeed = 1;
    private float maxSpeed = 3;

    private Vector2 dir;
    public bool isMoving;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;

    private Stack<MovementStep> movementSteps;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable() {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable() {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        CheckVisiable();
    }

    private void CheckVisiable()
    {
        if(currentScene == SceneManager.GetActiveScene().name)
            SetActionInScene();
        else
            SetInactiveInScene();
    }
    #region Set NPC action in scene
    public void SetActionInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        //TODO: Shadow unenable
        // transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        //TODO: Shadow unenable
        // transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion


}
