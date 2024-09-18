using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

public class DrunkAIMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float targetMoveSpeed = 75f;
    [SerializeField]
    float moveSpeedIncreaseAmount = 5f;

    [Header("Accelerating")]
    [SerializeField]
    float speedMaxWhenAccelerating = 120;
    [SerializeField]
    float speedAcceleratingIncreaseAmount = 10f;
    [SerializeField]
    float timeForReactionSecAccelerating = 4f;
    [SerializeField]
    float speedAcceleratingDecreaseAmount = 15f;

    [Header("Stop light")]
    [SerializeField]
    float moveSpeedWhenDecelerating = 45f;

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

    MovementState state;
    Rigidbody rb;

    //Current move speed
    float moveSpeed;

    //Waypoints vars
    List<GameObject> waypoints;

    GameObject currentWaypointToFollow;
    DebugDrawCircleRange currentWaypointToFollowData;

    //Acceleration vars
    float timeForReactionSecAcceleratingCounter = 0f;

    //Swerve vars
    bool isTryingToCorrectSwerve;
    int swerveCorrectionInputValue;
    float rotationWhenStartedSwerving;

    private void Awake()
    {
        SetState(MovementState.BeforeTakingOff);
        rb = GetComponent<Rigidbody>();

        TrafficLightController.OnSignalChangeBackToGreen += StartMovingAgain;
    }

    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("GameController").OrderBy(w => w.GetComponent<DebugDrawCircleRange>().Id).ToList();
    }

    void Update()
    {
        Debug.Log($"{state}");
        HandleState();
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case MovementState.TutorialAutoSwerve:

                transform.Rotate(0f, swerveRotationDefaultAmount * tutorialAutoSwerveMultiplier/*swerve force*/, 0f);

                if (transform.rotation.y <= rotationWhenStartedSwerving - tutorialAutoSwerveRotationAmount)
                {
                    SetState(MovementState.TutorialAutoSwerveCorrect);
                }

                break;
            case MovementState.TutorialAutoSwerveCorrect:

                if (transform.rotation.y < rotationWhenStartedSwerving + tutorialAutoSwerveRotationAmount)
                {
                    transform.Rotate(0f, swerveRotationDefaultAmount * tutorialAutoSwerveCorrectionMultiplier/*anti-swerve force*/, 0f);
                }
                else
                {
                    CheckIfReachedWaypoint();
                }

                break;
            case MovementState.TutorialManualSwerve:

                HandleSwerve();

                break;
            case MovementState.Swerving:

                HandleSwerve();

                break;
            case MovementState.PassengerControl:

                transform.Rotate(0f, swerveCorrectionInputValue * playerTurnAmount, 0f);

                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }

        //DialogueManager.instance.StartDialogue("TutorialEnd");
        rb.velocity = transform.forward * moveSpeed;
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

        float playerRotationAmount = isTryingToCorrectSwerve ? Mathf.Abs(swerveRotationAmount) * playerTurnAmount : 0; //player force
        float rotationAmount = swerveRotationAmount + playerRotationAmount; //end force amount depending on input

        transform.Rotate(0f, rotationAmount, 0f);

        if (transform.rotation.y > rotationWhenStartedSwerving) //Has car returned to starting rotation
        {
            SetState(MovementState.PassengerControl);
        }
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
                //Controlled in FixedUpdate

                break;
            case MovementState.PassengerControl:

                HandlePlayerInput();
                CheckIfReachedWaypoint();

                break;
            case MovementState.Accelerating:

                HandleWaypointFollowing();

                moveSpeed += speedAcceleratingIncreaseAmount * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetMoveSpeed, speedMaxWhenAccelerating);

                break;
            case MovementState.WaitingToStartDecceleratingAfterAccelerating:

                timeForReactionSecAcceleratingCounter += Time.deltaTime;
                if (timeForReactionSecAcceleratingCounter >= timeForReactionSecAccelerating)
                {
                    EditorApplication.isPlaying = false;//Game over
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

                break;
            case MovementState.Swerving:

                HandlePlayerInput();
                //Controlled in FixedUpdate

                break;
            case MovementState.TrafficLight:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    moveSpeed = 0f;
                }
                break;
            default:
                break;
        }
    }

    void CheckIfReachedWaypoint()
    {
        var possibleWaypoint = waypoints.FirstOrDefault(w => Vector3.Magnitude(transform.position - w.transform.position) < w.GetComponent<DebugDrawCircleRange>().Radius);
        if (possibleWaypoint != null)
        {
            currentWaypointToFollow = possibleWaypoint;
            currentWaypointToFollowData = possibleWaypoint.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveRange(0, waypoints.IndexOf(currentWaypointToFollow));

            SetState(MovementState.Normal);
        }
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
        transform.LookAt(currentWaypointToFollow.transform);

        if (Vector3.Magnitude(transform.position - currentWaypointToFollow.transform.position) < currentWaypointToFollowData.Radius)
        {
            currentWaypointToFollow = waypoints[0];
            currentWaypointToFollowData = currentWaypointToFollow.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveAt(0);
        }
    }

    void HandlePlayerInput()
    {
        isTryingToCorrectSwerve = Input.GetKeyDown(KeyCode.D);
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
            case MovementState.TutorialAutoSwerve:
                rotationWhenStartedSwerving = transform.rotation.y;
                break;
            case MovementState.TutorialManualSwerve:
                rotationWhenStartedSwerving = transform.rotation.y;
                break;
            case MovementState.Swerving:
                rotationWhenStartedSwerving = transform.rotation.y;
                break;
            case MovementState.TrafficLight:
                moveSpeed = moveSpeedWhenDecelerating;
                break;
            default:
                break;
        }
    }

    void StartMovingAgain()
    {
        SetState(MovementState.Start);
    }

    void OnDestroy()
    {
        TrafficLightController.OnSignalChangeBackToGreen -= StartMovingAgain;
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
    TrafficLight
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