using DG.Tweening;
using EasyRoads3Dv3;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using static Unity.VisualScripting.Member;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("StartScene")]
    [SerializeField]
    GameObject screenBlackout;
    [SerializeField]
    float startDialogueTime = 3f;

    [Header("NPC Cars")]
    [SerializeField]
    GameObject npcCarPrefab;
    [SerializeField]
    int numOfCarsToSpawn = 10;
    [SerializeField]
    int waypointsOffsetBetweenCarsMin;
    [SerializeField]
    int waypointsOffsetBetweenCarsMax;
    [SerializeField] bool spawnCars;

    [Header("Police")]
    [SerializeField]
    GameObject policeLightsPrefab;

    public static event Action OnPlaySiren;
    public static event Action OnPlayCrash;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DrunkAIMovement.OnTutorialEndSpawnNPCs += SpawnNPCs;
    }

    void SpawnNPCs()
    {
        if (!spawnCars)
        {
            return;
        }

        var waypointParent = GameObject.FindGameObjectWithTag("WaypointParent");

        int numOfCarsSpawnedCounter = 0;

        int waypointOffsetCounter = 0;
        int currentWaypointOffset = UnityEngine.Random.Range(waypointsOffsetBetweenCarsMin, waypointsOffsetBetweenCarsMax + 1);
        for (int i = waypointParent.transform.childCount - 1; i >= 0; i--)
        {
            waypointOffsetCounter++;

            if (waypointOffsetCounter == currentWaypointOffset)
            {
                if (numOfCarsSpawnedCounter >= numOfCarsToSpawn)
                {
                    break;
                }

                var waypoint = waypointParent.transform.GetChild(i);

                // Spawn a car at the given waypoint
                Vector3 vector = waypoint.transform.localPosition + waypoint.transform.right * -1f * waypoint.GetComponent<DebugDrawCircleRange>().Radius * 2f;
                //Vector3 newPos = waypoint.transform.TransformVector(vector);
                //Vector3 spawnPos = new Vector3(waypoint.position.x - waypoint.GetComponent<DebugDrawCircleRange>().Radius * 3, waypoint.position.y, waypoint.position.z);
                //Quaternion carOrientation = waypoint.rotation * Quaternion.Euler(0, -90, 0); // Rotate it 180 degs on the y axis
                GameObject spawnedCar = Instantiate(npcCarPrefab, vector, Quaternion.identity);
                numOfCarsSpawnedCounter++;

                spawnedCar.GetComponent<NPCCarBehavior>().SetWaypoints(i);

                waypointOffsetCounter = 0;
                currentWaypointOffset = UnityEngine.Random.Range(waypointsOffsetBetweenCarsMin, waypointsOffsetBetweenCarsMax + 1);
            }
        }
    }

    [YarnCommand("disableScreenBlackout")]
    public void DisableScreenBlackout()
    {
        screenBlackout.SetActive(false);
    }

    [YarnCommand("enableScreenBlackout")]
    public void EnableScreenBlackout()
    {
        screenBlackout.SetActive(true);
    }

    [YarnCommand("spawnPoliceLights")]
    public void SpawnLights()
    {
        //play sound
        Vector3 spawnOffset = new Vector3(-6, 0, 9);

        Instantiate(policeLightsPrefab, transform.position + spawnOffset, transform.rotation, transform);
        OnPlaySiren.Invoke();
    }

    [YarnCommand("getInCar")]
    public void GetInCar()
    {
        StartCoroutine(GetInCarCoroutine());
    }

    IEnumerator GetInCarCoroutine()
    {
        yield return new WaitForSeconds(startDialogueTime);

        DialogueNodeManager.instance.StartDialogue("TutorialSetup");
    }

    [YarnCommand("crash")]
    public void Crash()
    {
        OnPlayCrash.Invoke();
        EnableScreenBlackout();

        StartCoroutine(CrashCoroutine());
    }

    IEnumerator CrashCoroutine()
    {
        while (AudioManager.instance.IsSFXOn())
        {
            yield return null;
        }

        DialogueNodeManager.instance.StartDialogue("BadEnding");
        
    }

    [YarnCommand("endGame")]
    public void EndGame()
    { 
        SceneManager.LoadScene(2);
    }

    void OnDestroy()
    {
        DrunkAIMovement.OnTutorialEndSpawnNPCs -= SpawnNPCs;
    }
}
