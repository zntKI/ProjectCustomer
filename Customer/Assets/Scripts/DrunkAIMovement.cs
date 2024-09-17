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
    float swerveMultiplierMin;
    [SerializeField]
    float swerveMultiplierMax;
    [SerializeField]
    float swerveRotationDefaultAmount = 0.1f;
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
    float swerveInputValue;
    float currentSwerveMultipier;
    float rotationWhenStartedSwerving;

    private void Awake()
    {
        state = MovementState.Normal;
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
        //switch (state)
        //{
        //    case MovementState.Swerving:

        //        float swerveRotationAmount = swerveRotationDefaultAmount * currentSwerveMultipier; //swerve force
        //        float playerRotationAmount = Mathf.Abs(swerveRotationAmount) * 2 * swerveInputValue; //player force
        //        float rotationAmount = swerveRotationAmount + playerRotationAmount; //end force amount depending on input

        //        transform.Rotate(0f, rotationAmount, 0f);

        //        bool hasCarReturnedToStartingRotation = currentSwerveMultipier < 0 ? transform.rotation.y > rotationWhenStartedSwerving
        //            : transform.rotation.y < rotationWhenStartedSwerving;
        //        if (hasCarReturnedToStartingRotation)
        //        {
        //            SetState(MovementState.PassengerControl);
        //        }

        //        break;
        //    case MovementState.PassengerControl:

        //        transform.Rotate(0f, swerveInputValue * playerTurnAmount, 0f);

        //        break;
        //    case MovementState.Accelerating:
        //        break;
        //    default:
        //        break;
        //}

        //DialogueManager.instance.StartDialogue("TutorialEnd");
        rb.velocity = transform.forward * moveSpeed;
    }

    void HandleState()
    {
        switch (state)
        {
            case MovementState.Start:
                HandleWaypointFollowing();

                moveSpeed += moveSpeedIncreaseAmount * Time.deltaTime;
                if (moveSpeed >= targetMoveSpeed)
                {
                    moveSpeed = targetMoveSpeed;
                    SetState(MovementState.Accelerating);
                }
                break;
            case MovementState.Accelerating:
                HandleWaypointFollowing();

                moveSpeed += speedAcceleratingIncreaseAmount * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetMoveSpeed, speedMaxWhenAccelerating);

                if (moveSpeed >= speedMaxWhenAccelerating)
                {
                    Debug.Log($"Reached speed of: {moveSpeed} and started counting for reaction: {timeForReactionSecAcceleratingCounter} secs");
                    timeForReactionSecAcceleratingCounter += Time.deltaTime;
                    if (timeForReactionSecAcceleratingCounter >= timeForReactionSecAccelerating)
                    {
                        EditorApplication.isPlaying = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.S))
                    {
                        state = MovementState.DecceleratingAfterAccelerating;

                        timeForReactionSecAcceleratingCounter = 0;
                    }
                }
                break;
            case MovementState.DecceleratingAfterAccelerating:
                HandleWaypointFollowing();

                moveSpeed -= speedAcceleratingDecreaseAmount * Time.deltaTime;
                moveSpeed = Mathf.Clamp(moveSpeed, targetMoveSpeed, speedMaxWhenAccelerating);

                Debug.Log($"Started deccelerating with current speed of: {moveSpeed}");

                if (moveSpeed <= targetMoveSpeed)
                {
                    EditorApplication.isPlaying = false;
                }

                break;
            case MovementState.Swerving:
                HandlePlayerInput();
                break;
            case MovementState.PassengerControl:
                HandlePlayerInput();
                CheckIfReachedWaypoint();
                break;
            case MovementState.Normal:
                HandleWaypointFollowing();

                // Implement traffic light
                var trafficLights = GameObject.FindGameObjectsWithTag("Finish");
                var trafficLightInRange = trafficLights.FirstOrDefault(t => Vector3.Magnitude(transform.position - t.transform.position) < t.GetComponent<TrafficLightController>().Range);
                if (trafficLightInRange != null)
                {
                    SetState(MovementState.TrafficLight);
                    trafficLightInRange.GetComponent<TrafficLightController>().ChangeSignal(TrafficLightSignal.Red);
                }
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
        swerveInputValue = Input.GetAxisRaw("Horizontal");
    }

    [YarnCommand("setState")]
    public void SetState(int stateToChangeTo)
    {
        state = (MovementState)stateToChangeTo;
        switch (state)
        {
            case MovementState.Start:
                break;
            case MovementState.Swerving:
                rotationWhenStartedSwerving = transform.rotation.y;
                currentSwerveMultipier = Random.Range(swerveMultiplierMin, swerveMultiplierMax);
                break;
            case MovementState.TrafficLight:
                moveSpeed = moveSpeedWhenDecelerating;
                break;
            case MovementState.PassengerControl:
                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }
    }

    void SetState(MovementState stateToChangeTo)
    {
        state = stateToChangeTo;
        switch (state)
        {
            case MovementState.Start:
                break;
            case MovementState.Swerving:
                rotationWhenStartedSwerving = transform.rotation.y;
                currentSwerveMultipier = Random.Range(swerveMultiplierMin, swerveMultiplierMax);
                break;
            case MovementState.TrafficLight:
                moveSpeed = moveSpeedWhenDecelerating;
                break;
            case MovementState.PassengerControl:
                break;
            case MovementState.Accelerating:
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
    Start,
    Accelerating,
    DecceleratingAfterAccelerating,
    Normal,
    PassengerControl,
    Swerving,
    TrafficLight
}