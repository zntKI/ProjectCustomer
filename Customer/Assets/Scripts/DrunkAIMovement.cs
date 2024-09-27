using EasyRoads3Dv3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Yarn.Unity;
using static UnityEngine.GraphicsBuffer;

public class DrunkAIMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float targetMoveSpeed = 75f;
    [SerializeField]
    float moveSpeedIncreaseAmount = 5f;
    [SerializeField]
    float rotationSpeed = 5f;

    [Header("Accelerating")]
    [SerializeField]
    float speedMaxWhenAccelerating = 120;
    [SerializeField]
    float speedAcceleratingIncreaseAmount = 10f;
    [SerializeField]
    float timeForReactionSecAccelerating = 4f;
    [SerializeField]
    float speedAcceleratingDecreaseAmount = 15f;

    [Header("Swerving")]
    [SerializeField]
    float swerveRotationDefaultAmount = 0.1f;
    [SerializeField]
    float tutorialAutoSwerveMultiplier = -1f;
    [SerializeField]
    float tutorialAutoSwerveRotationAmount = 20;
    [SerializeField]
    float tutorialAutoSwerveCorrectionMultiplier = -1.1f;
    [SerializeField]
    float tutorialManualSwerveMultiplier = -1.5f;
    [SerializeField]
    float normalSwerveMultiplier = -2f;
    [SerializeField]
    float playerTurnAmount = 2f;
    [SerializeField]
    float playControlTurnAmount = 2f;

    [Header("TrafficLight")]
    [SerializeField]
    GameObject trafficLightPrefab; // The object you want to spawn
    [SerializeField]
    float trafficLightDistanceForSpawn = 200f; // Distance ahead from the car in meters
    [SerializeField]
    float trafficLightOffsetToTheRight = 5f;
    [SerializeField]
    float timeForReactionSecStopLight = 4f;
    [SerializeField]
    float stopDistanceFromStopLight = 10f;

    [Header("StopSign")]
    [SerializeField] GameObject stopSignPrefab;
    [SerializeField] float stopSignDistanceForSpawn = 50;
    [SerializeField] float stopSignOffsetToTheRight = 5f;

    MovementState state;
    Rigidbody rb;

    //Current move speed
    float moveSpeed;

    public static event Action<float> onMovespeedChange;

    //Waypoints vars
    List<GameObject> waypoints = new List<GameObject>();

    GameObject currentWaypointToFollow;
    DebugDrawCircleRange currentWaypointToFollowData;

    //Acceleration vars
    float timeForReactionSecAcceleratingCounter = 0f;

    //Swerve vars
    int swerveCorrectionInputValue;
    int playerControlInputValue;
    float rotationWhenStartedSwerving;

    //StopLight vars
    float timeForReactionSecStopLightCounter = 0f;
    GameObject spawnedTrafficLight;
    GameObject trafficLightWaypoint;
    float deccelerationAmountBeforeTrafficLight = 0f;


    // Events
    public static event Action OnStartCarPlaySound;
    public static event Action OnSwervePlaySound;

    public static event Action OnTutorialEndSpawnNPCs;

    private void Awake()
    {
        SetState(MovementState.BeforeTakingOff);
        rb = GetComponent<Rigidbody>();

        TrafficLightController.OnSignalChangeBackToGreen += StartMovingAgain;
    }

    void Start()
    {
        var waypointParent = GameObject.FindGameObjectWithTag("WaypointParent");
        for (int i = 0; i < waypointParent.transform.childCount; i++)
        {
            waypoints.Add(waypointParent.transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        //Debug.Log(transform.eulerAngles.y);
        Debug.Log($"{state}");
        HandleState();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case MovementState.TutorialAutoSwerve:

                transform.Rotate(0f, swerveRotationDefaultAmount * tutorialAutoSwerveMultiplier/*swerve force*/, 0f);

                if ((transform.localEulerAngles.y >= 180 ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y)/*Converted degrees*/
                    <= rotationWhenStartedSwerving - tutorialAutoSwerveRotationAmount)
                {
                    SetState(MovementState.TutorialAutoSwerveCorrect);
                }

                break;
            case MovementState.TutorialAutoSwerveCorrect:

                transform.Rotate(0f, swerveRotationDefaultAmount * tutorialAutoSwerveCorrectionMultiplier/*anti-swerve force*/, 0f);
                if (CheckIfReachedWaypoint())
                {
                    DialogueNodeManager.instance.StartDialogue("TutorialSwerving");
                }

                break;
            //case MovementState.TutorialManualSwerve:

            //    HandleSwerve();

            //    break;
            //case MovementState.Swerving:

            //    HandleSwerve();

            //    break;
            case MovementState.PassengerControl:

                transform.Rotate(0f, playerControlInputValue * playControlTurnAmount, 0f);

                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }


        rb.velocity = transform.forward * moveSpeed;
    }

    void HandleState()
    {
        switch (state)
        {
            case MovementState.BeforeTakingOff:

                //The car doesn't move

                break;
            case MovementState.Start:

                HandleWaypointFollowing();

                moveSpeed += moveSpeedIncreaseAmount * Time.deltaTime;
                if (moveSpeed >= targetMoveSpeed)
                {
                    moveSpeed = targetMoveSpeed;
                    SetState(MovementState.Normal);
                }

                onMovespeedChange?.Invoke(moveSpeed);

                break;
            case MovementState.Normal:

                HandleWaypointFollowing();

                //// Implement traffic light
                //var trafficLights = GameObject.FindGameObjectsWithTag("Finish");
                //var trafficLightInRange = trafficLights.FirstOrDefault(t => Vector3.Magnitude(transform.position - t.transform.position) < t.GetComponent<TrafficLightController>().Range);
                //if (trafficLightInRange != null)
                //{
                //    SetState(MovementState.TrafficLight);
                //    trafficLightInRange.GetComponent<TrafficLightController>().ChangeSignal(TrafficLightSignal.Red);
                //}

                break;
            case MovementState.TutorialAutoSwerve:

                //Controlled in FixedUpdate

                break;
            case MovementState.TutorialAutoSwerveCorrect:

                //Controlled in FixedUpdate

                break;
            case MovementState.TutorialManualSwerve:
                HandlePlayerInput();
                HandleSwerve();

                break;
            case MovementState.PassengerControl:
                //HandlePlayerInput();

                playerControlInputValue = Input.GetKey(KeyCode.D) ? 1 : 0;
                CheckIfReachedWaypoint();
                break;
            case MovementState.Accelerating:

                HandleWaypointFollowing();

                moveSpeed += speedAcceleratingIncreaseAmount * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetMoveSpeed, speedMaxWhenAccelerating);

                onMovespeedChange?.Invoke(moveSpeed);

                break;
            case MovementState.WaitingToStartDecceleratingAfterAccelerating:

                timeForReactionSecAcceleratingCounter += Time.deltaTime;
                if (timeForReactionSecAcceleratingCounter >= timeForReactionSecAccelerating)
                {
#if UNITY_EDITOR
                    //EditorApplication.isPlaying = false;
                    GameManager.instance.Crash();
#else
        // For quitting the built application
        Application.Quit();
#endif
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    SetState(MovementState.DecceleratingAfterAccelerating);
                    timeForReactionSecAcceleratingCounter = 0;
                }

                break;
            case MovementState.DecceleratingAfterAccelerating:

                HandleWaypointFollowing();

                moveSpeed -= speedAcceleratingDecreaseAmount * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetMoveSpeed, speedMaxWhenAccelerating);

                Debug.Log($"Started deccelerating with current speed of: {moveSpeed}");

                if (moveSpeed <= targetMoveSpeed)
                {
                    SetState(MovementState.Normal);
                }

                onMovespeedChange?.Invoke(moveSpeed);

                break;
            case MovementState.Swerving:

                HandlePlayerInput();
                HandleSwerve();

                break;
            case MovementState.SpawnTrafficLight:

                spawnedTrafficLight = SpawnPrefab(trafficLightPrefab, trafficLightDistanceForSpawn, trafficLightOffsetToTheRight);
                SetState(MovementState.Normal);

                break;
            case MovementState.TrafficLight:

                HandleWaypointFollowing();

                timeForReactionSecStopLightCounter += Time.deltaTime;
                if (timeForReactionSecStopLightCounter >= timeForReactionSecStopLight)
                {
                    GameManager.instance.Crash();
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    float distanceFromStopLightCounter = 0f;

                    int waypointIndex = waypoints.IndexOf(trafficLightWaypoint);
                    for (int i = waypointIndex; i >= 0; i--)
                    {
                        distanceFromStopLightCounter += waypoints[i].GetComponent<DebugDrawCircleRange>().Radius * 2;
                        if (distanceFromStopLightCounter >= stopDistanceFromStopLight)
                        {
                            float distanceToCompleteStop = 0f;
                            for (int j = i - 1; j >= 0; j--)
                            {
                                distanceToCompleteStop += waypoints[i].GetComponent<DebugDrawCircleRange>().Radius * 2;
                            }

                            deccelerationAmountBeforeTrafficLight = (moveSpeed * moveSpeed) / (2 * distanceToCompleteStop);

                            break;
                        }
                    }

                    SetState(MovementState.DecceleratingBeforeStopLight);
                    timeForReactionSecAcceleratingCounter = 0;
                }

                break;
            case MovementState.DecceleratingBeforeStopLight:

                HandleWaypointFollowing();

                moveSpeed -= deccelerationAmountBeforeTrafficLight * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, 0, targetMoveSpeed);

                onMovespeedChange?.Invoke(moveSpeed);

                break;
            default:
                break;
        }
    }

    void HandleSwerve()
    {
        float swerveRotationAmount = 0f; //swerve force
        switch (state)
        {
            case MovementState.TutorialManualSwerve:
                swerveRotationAmount = swerveRotationDefaultAmount * tutorialManualSwerveMultiplier;
                break;
            case MovementState.Swerving:
                swerveRotationAmount = swerveRotationDefaultAmount * normalSwerveMultiplier;
                break;
            default:
                Debug.LogError("Not in correct state for handling swerving!");
                break;
        }

        //swerveCorrectionInputValue = Input.GetKeyDown(KeyCode.D) ? 1 : 0;
        //Debug.Log(swerveCorrectionInputValue);
        float playerRotationAmount = swerveCorrectionInputValue == 1 ? Mathf.Abs(swerveRotationAmount) * playerTurnAmount : 0; //player force
        float rotationAmount = swerveRotationAmount + playerRotationAmount; //end force amount depending on input

        transform.Rotate(0f, rotationAmount, 0f);

        if (transform.localEulerAngles.y > rotationWhenStartedSwerving) //Has car returned to starting rotation
        {
            switch (state)
            {
                case MovementState.TutorialManualSwerve:
                    DialogueNodeManager.instance.StartDialogue("TutorialEnd");
                    OnTutorialEndSpawnNPCs?.Invoke();
                    break;
                case MovementState.Swerving:
                    DialogueNodeManager.instance.StartDialogue("SwervingEnd");
                    break;
                default:
                    break;
            }

            SetState(MovementState.PassengerControl);
        }
    }

    bool CheckIfReachedWaypoint()
    {
        var possibleWaypoint = waypoints.FirstOrDefault(w => Vector3.Magnitude(transform.position - w.transform.position) < w.GetComponent<DebugDrawCircleRange>().Radius);
        if (possibleWaypoint != null)
        {
            currentWaypointToFollow = possibleWaypoint;
            currentWaypointToFollowData = possibleWaypoint.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveRange(0, waypoints.IndexOf(currentWaypointToFollow));

            SetState(MovementState.Normal);
            return true;
        }

        return false;
    }

    void HandleWaypointFollowing()
    {
        if (currentWaypointToFollow == null)
        {
            //Pick the first waypoint
            currentWaypointToFollow = waypoints[0];
            currentWaypointToFollowData = currentWaypointToFollow.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveAt(0);
        }
        //transform.LookAt(currentWaypointToFollow.transform);

        // Calculate the direction to the target
        Vector3 direction = currentWaypointToFollow.transform.position - transform.position;
        // Calculate the target rotation using the direction vector
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        if (Vector3.Magnitude(transform.position - currentWaypointToFollow.transform.position) < currentWaypointToFollowData.Radius)
        {
            currentWaypointToFollow = waypoints[0];
            currentWaypointToFollowData = currentWaypointToFollow.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveAt(0);
        }
    }

    void HandlePlayerInput()
    {
        swerveCorrectionInputValue = Input.GetKey(KeyCode.D) ? 1 : 0;
    }

    [YarnCommand("setState")]
    public void SetState(int stateToChangeTo)
    {
        state = (MovementState)stateToChangeTo;
        SetupState();
    }

    void SetState(MovementState stateToChangeTo)
    {
        state = stateToChangeTo;
        SetupState();
    }

    void SetupState()
    {
        switch (state)
        {
            case MovementState.Start:
                OnStartCarPlaySound?.Invoke();
                break;
            case MovementState.TutorialAutoSwerve:
                rotationWhenStartedSwerving = transform.localEulerAngles.y >= 180 ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y;
                OnSwervePlaySound?.Invoke();
                break;
            case MovementState.TutorialManualSwerve:
                rotationWhenStartedSwerving = transform.localEulerAngles.y;
                OnSwervePlaySound?.Invoke();
                break;
            case MovementState.Swerving:
                rotationWhenStartedSwerving = transform.localEulerAngles.y;
                OnSwervePlaySound?.Invoke();
                break;
            case MovementState.TrafficLight:
                spawnedTrafficLight.GetComponent<TrafficLightController>().ChangeSignal(TrafficLightSignal.Red);
                break;
            default:
                break;
        }
    }

    void StartMovingAgain()
    {
        SetState(MovementState.Start);
        DialogueNodeManager.instance.StartDialogue("TrafficLightEnd");
    }

    void OnDestroy()
    {
        TrafficLightController.OnSignalChangeBackToGreen -= StartMovingAgain;
    }

    [YarnCommand("spawnStopSign")]
    public void SpawnStopSign()
    {
        SpawnPrefab(stopSignPrefab, stopSignDistanceForSpawn, stopSignOffsetToTheRight);
    }

    GameObject SpawnPrefab(GameObject prefab, float distanceAhead, float offset)
    {
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        float distanceCounter = 0f;
        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypoint = waypoints[i];

            distanceCounter += waypoint.GetComponent<DebugDrawCircleRange>().Radius * 2;
            if (distanceCounter >= distanceAhead)
            {
                //spawnPosition = waypoint.transform.position + waypoint.transform.right * (offset + prefab.transform.lossyScale.z / 2);
                spawnPosition = waypoint.transform.position + waypoint.transform.right * (offset + prefab.transform.lossyScale.z / 2);

                Quaternion rotationOffset = Quaternion.Euler(0, 0, 0);
                spawnRotation = waypoint.transform.rotation * rotationOffset;

                trafficLightWaypoint = waypoint;

                break;
            }
        }

        return Instantiate(prefab, spawnPosition, spawnRotation);
    }

    bool isCrashCouroutineRunning = false;
    IEnumerator CrashTimerCoroutine()
    {
        isCrashCouroutineRunning = true;
        yield return new WaitForSeconds(4f);
        isCrashCouroutineRunning = false;
        Debug.Log("crashed");
        //GameManager.instance.Crash();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
        {
            if (isCrashCouroutineRunning) {
                StopCoroutine(CrashTimerCoroutine());
                isCrashCouroutineRunning = false;
            }
        }
        else
        {
            if (!isCrashCouroutineRunning && state != MovementState.BeforeTakingOff)
            {
                StartCoroutine(CrashTimerCoroutine());
            }
        }

        if(collision.gameObject.CompareTag("Guardrail"))
        {
            GameManager.instance.Crash();
        }
    }
}


public enum MovementState
{
    BeforeTakingOff,
    Start,
    Normal,
    TutorialAutoSwerve,
    TutorialAutoSwerveCorrect,
    TutorialManualSwerve,
    PassengerControl,
    Accelerating,
    WaitingToStartDecceleratingAfterAccelerating,
    DecceleratingAfterAccelerating,
    Swerving,
    SpawnTrafficLight,
    TrafficLight,
    DecceleratingBeforeStopLight
}

/*
    BeforeTakingOff,
    Start,
    Normal,
    TutorialAutoSwerve,
    TutorialAutoSwerveCorrect,
    Normal,
    TutorialManualSwerve,
    PassengerControl,
    Normal,
    Accelerating,
    WaitingToStartDecceleratingAfterAccelerating,
    DecceleratingAfterAccelerating,
    Normal,
    Swerving,
    PassengerControl,
    Normal,
    TrafficLight,
    Normal,
    ...(about consciousness game)
*/