using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public ObjectPool trackPool;
    public Transform player;
    public int segmentsAhead = 6;
    public int maxSegments = 10;

    private float spawnZ = 0f;
    private float segmentLength = 0f;
    private Queue<GameObject> activeSegments = new Queue<GameObject>();

    // Materials: 0 = Red, 1 = Green, 2 = Blue, 3 = Neutral
    public Material[] laneMaterials;
    private int spawnCount = 0;

    void Start()
    {
        GameObject temp = trackPool.GetFromPool();
        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        segmentLength = bounds.size.z - 0.12f;
        trackPool.ReturnToPool(temp);

        for (int i = 0; i < segmentsAhead; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player.position.z + (segmentsAhead * segmentLength) > spawnZ)
        {
            SpawnSegment();

            if (activeSegments.Count > maxSegments)
            {
                GameObject old = activeSegments.Dequeue();
                trackPool.ReturnToPool(old);
            }
        }
    }

    void SpawnSegment()
    {
        GameObject segment = trackPool.GetFromPool();
        segment.transform.position = new Vector3(0, 0, Mathf.Round(spawnZ * 100f) / 100f);
        segment.transform.rotation = Quaternion.Euler(0, 90, 0);

        bool useNeutralOnly = spawnCount < 3;
        ApplyColorsToLanes(segment, useNeutralOnly);

        activeSegments.Enqueue(segment);
        spawnZ += segmentLength;
        spawnCount++;
    }

    void ApplyColorsToLanes(GameObject segment, bool neutralOnly = false)
    {
        Transform lane1 = segment.transform.Find("Track 01");
        Transform lane2 = segment.transform.Find("Track 02");
        Transform lane3 = segment.transform.Find("Track 03");

        SetLaneColor(lane1, neutralOnly);
        SetLaneColor(lane2, neutralOnly);
        SetLaneColor(lane3, neutralOnly);
    }

    void SetLaneColor(Transform lane, bool neutralOnly)
    {
        int colorIndex = neutralOnly ? 3 : Random.Range(0, laneMaterials.Length);
        lane.GetComponent<Renderer>().material = laneMaterials[colorIndex];

        Obstacle obs = lane.GetComponent<Obstacle>();
        if (obs != null)
            obs.colorIndex = colorIndex;
    }
}
