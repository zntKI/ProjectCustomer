using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DrunkAIMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float targetMoveSpeed = 75f;
    [SerializeField]
    float moveSpeedIncreaseAmount = 5f;

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

    float moveSpeed;

    float swerveInputValue;
    float currentSwerveMultipier;
    float rotationWhenStartedSwerving;

    List<GameObject> waypoints;

    GameObject currentWaypointToFollow;
    DebugDrawCircleRange currentWaypointToFollowData;

    private void Awake()
    {
        state = MovementState.Start;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("GameController").OrderBy(w => w.GetComponent<DebugDrawCircleRange>().Id).ToList();
    }

    void Update()
    {
        HandleState();
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case MovementState.Swerving:

                float swerveRotationAmount = swerveRotationDefaultAmount * currentSwerveMultipier; //swerve force
                float playerRotationAmount = Mathf.Abs(swerveRotationAmount) * 2 * swerveInputValue; //player force
                float rotationAmount = swerveRotationAmount + playerRotationAmount; //end force amount depending on input
                
                transform.Rotate(0f, rotationAmount, 0f);

                bool hasCarReturnedToStartingRotation = currentSwerveMultipier < 0 ? transform.rotation.y > rotationWhenStartedSwerving
                    : transform.rotation.y < rotationWhenStartedSwerving;
                if (hasCarReturnedToStartingRotation)
                {
                    SetState(MovementState.PassengerControl);
                }

                break;
            case MovementState.PassengerControl:

                transform.Rotate(0f, swerveInputValue * playerTurnAmount, 0f);

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
            case MovementState.Start:
                HandleWaypointFollowing();

                moveSpeed += moveSpeedIncreaseAmount * Time.deltaTime;
                if (moveSpeed >= targetMoveSpeed)
                {
                    moveSpeed = targetMoveSpeed;
                    SetState(MovementState.Swerving/*(MovementState)Random.Range(2, 4)*/);
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
            case MovementState.PassengerControl:
                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }
    }
}


public enum MovementState
{
    Start,
    Normal,
    PassengerControl,
    Swerving,
    Accelerating
}