using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCarBehavior : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;

    Rigidbody rb;

    List<GameObject> waypoints = new List<GameObject>();

    GameObject currentWaypointToFollow;
    DebugDrawCircleRange currentWaypointToFollowData;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetWaypoints(int startIndex)
    {
        var waypointParent = GameObject.FindGameObjectWithTag("WaypointParent");
        for (int i = startIndex; i >= 0; i--)
        {
            waypoints.Add(waypointParent.transform.GetChild(i).gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed;
    }

    void Update()
    {
        HandleWaypointFollowing();
    }

    void HandleWaypointFollowing()
    {
        if (currentWaypointToFollow == null)
        {
            //Pick the first waypoint
            currentWaypointToFollow = waypoints[0];
            currentWaypointToFollowData = currentWaypointToFollow.GetComponent<DebugDrawCircleRange>();
            waypoints.RemoveAt(0);

            Vector3 forwardDirection = currentWaypointToFollow.transform.localRotation * Vector3.right * -1;

            Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = targetRotation;
        }


        if (Vector3.Magnitude(transform.position - (currentWaypointToFollow.transform.position + (currentWaypointToFollow.transform.localRotation * Vector3.forward * currentWaypointToFollowData.Radius * 2f))) < currentWaypointToFollowData.Radius)
        {
            currentWaypointToFollow = null;
        }
    }
}
