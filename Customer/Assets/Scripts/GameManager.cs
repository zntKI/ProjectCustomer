using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject npcCarPrefab;
    [SerializeField]
    float waypointsOffsetBetweenCars;

    // Start is called before the first frame update
    void Start()
    {
        var waypointParent = GameObject.FindGameObjectWithTag("WaypointParent");

        int waypointOffsetCounter = 0;
        for (int i = waypointParent.transform.childCount - 1; i >= 0; i--)
        {
            if (waypointOffsetCounter % waypointsOffsetBetweenCars == 0)
            {
                var waypoint = waypointParent.transform.GetChild(i);

                // Spawn a car at the given waypoint
                Vector3 spawnPos = new Vector3(waypoint.position.x, waypoint.position.y, waypoint.position.z + waypoint.GetComponent<DebugDrawCircleRange>().Radius * 2);
                Quaternion carOrientation = waypoint.rotation * Quaternion.Euler(0, -90, 0); // Rotate it 180 degs on the y axis
                GameObject spawnedCar = Instantiate(npcCarPrefab, spawnPos, carOrientation);

                spawnedCar.GetComponent<NPCCarBehavior>().SetWaypoints(i);
            }
            waypointOffsetCounter++;
        }
    }
}
