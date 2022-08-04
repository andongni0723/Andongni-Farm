using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AnFarm.AStar;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;
    [SceneName]public string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;

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

    private Grid grid;
    private Stack<MovementStep> movementSteps;

    private bool isInitialised;
    private bool npcMove;
    private bool sceneLoaded;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }


    private void FixedUpdate()
    {
        if(sceneLoaded)
            Movement();
    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }

        sceneLoaded = true;
    }

    private void CheckVisiable()
    {
        
        Debug.Log(currentScene);
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }

    private void InitNPC()
    {
        targetScene = currentScene;

        // Put the NPC in the grid center
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        targetGridPosition = currentGridPosition;
    }

    /// <summary>
    /// According to the schedule to build a path
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;

        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }
        //TODO: move to the other scenes
        if (movementSteps.Count > 1)
        {
            // Update the timestamp corresponding to each step
            UpdateTimeOnPath();
        }
    }

    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();

                currentScene = step.sceneName;

                CheckVisiable();

                nextGridPosition = (Vector3Int)step.gridCoodinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);

        // Have enough to move
        if (stepTime > GameTime)
        {
            // Using to moving time different (second)
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            // Real moving distance
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));

            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // If time is over, flash at next grid position 
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        npcMove = false;
    }

    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;

        TimeSpan currentGameTime = GameTime;

        foreach (MovementStep step in movementSteps)
        {
            if (previousStep == null)
                previousStep = step;

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;

            if (MoveInDiagona(step, previousStep))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));

            // Get the timestamp about next step
            currentGameTime = currentGameTime.Add(gridMovementStepTime);

            // To next step
            previousStep = step;
        }

    }

    private bool MoveInDiagona(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoodinate.x != previousStep.gridCoodinate.x) && (currentStep.gridCoodinate.y != previousStep.gridCoodinate.y);
    }

    #region Set NPC active in scene
    public void SetActiveInScene()
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

    /// <summary>
    /// Input grid position and return world position center
    /// </summary>
    /// <param name="gridPos">Grid Position</param>
    /// <returns>world position center</returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    #endregion
}
