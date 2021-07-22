using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject EmptyRoom, DoorRoom, KeyRoom, EnemyRoom, PlayerRoom, Bridge;

    [SerializeField] private int mapX, mapZ;
    [SerializeField] private float roomX, roomZ, bridgeLength;
    [SerializeField] [Range(0, 1)] private float frontProbability, sideProbability;
    [SerializeField] private int rooms, keys, doors, enemies;

    private int[,] map;

    private List<GameObject> roomsList, bridgesList;

    public IEnumerator GenerateWorld(int players)
    {
        //Clearing the map.
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Need to wait 1 frame, otherwise NavMesh will bake on old objects.
        yield return 0;

        ResetMap();

        //Initial cross in the center of the map.
        int centrumX = mapX / 2, centrumZ = mapZ / 2;
        map[centrumX, centrumZ] = 1;
        map[centrumX - 1, centrumZ] = 1;
        map[centrumX + 1, centrumZ] = 1;
        map[centrumX, centrumZ - 1] = 1;
        map[centrumX, centrumZ + 1] = 1;

        //Generate rooms, doors, keys.
        for (int i = 0; i < rooms; i++)
        {
            ExpandRoad(true, true);
        }
        for (int i = 0; i < enemies; i++)
        {
            SwapRoom(1, 4);
        }
        for (int i = 0; i < doors; i++)
        {
            SwapRoom(1, 2);
        }
        for (int i = 0; i < keys; i++)
        {
            SwapRoom(1, 3);
        }
        //Player in the centrum.
        if (players > 0)
        {
            map[centrumX, centrumZ] = 5;
        }
        //Additional players.
        for (int i = 1; i < players; i++)
        {
            SwapRoom(1, 5);
        }

        //Istantiating.
        IstantiateRooms();
        if (bridgeLength > 0)
        {
            IstantiateBridges();
        }

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    void ResetMap()
    {
        map = new int[mapX, mapZ];
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                map[x, z] = 0;
            }
        }
    }

    struct ExpandableRoad
    {
        public Vector2Int expandableRoad; //End of the road that can be expanded.
        public Vector2Int rearRoad; //Only road connected to expandableRoad.
    }

    void ExpandRoad(bool front, bool side)
    {
        //Ends of the roads.
        List<ExpandableRoad> expandableRoads = new List<ExpandableRoad>();
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (IsRoad(x, z))
                {
                    //How many connected roads.
                    List<Vector2Int> connectedRoads = new List<Vector2Int>();
                    if (IsRoad(x - 1, z))
                        connectedRoads.Add(new Vector2Int(x - 1, z));
                    if (IsRoad(x + 1, z))
                        connectedRoads.Add(new Vector2Int(x + 1, z));
                    if (IsRoad(x, z - 1))
                        connectedRoads.Add(new Vector2Int(x, z - 1));
                    if (IsRoad(x, z + 1))
                        connectedRoads.Add(new Vector2Int(x, z + 1));

                    //If connected with only one road, it's end of the road.
                    if (connectedRoads.Count == 1)
                    {
                        //Add end of the road with connected segment of road.
                        ExpandableRoad expandableRoad = new ExpandableRoad() { expandableRoad = new Vector2Int(x, z), rearRoad = connectedRoads[0] };
                        expandableRoads.Add(expandableRoad);
                    }
                }
            }
        }

        //If there are any expandable roads.
        if (expandableRoads.Count > 0)
        {
            //Computing direction of expanding.
            ExpandableRoad ER = expandableRoads[Random.Range(0, expandableRoads.Count)];
            Vector2Int rearRoad = ER.rearRoad;
            Vector2Int expandableRoad = ER.expandableRoad;
            Vector2Int newRoad = expandableRoad + expandableRoad - rearRoad; //Ahead road coordinates.

            //How many segments of roads are placed.
            int newRoads = 0;

            //Placing side segment of roads (depending on the probability).
            if (side)
            {
                if (rearRoad.x == expandableRoad.x)
                {
                    newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 1, 0);

                    newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, -1, 0);
                }

                else
                {
                    newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 0, 1);

                    newRoads += ExpandSideRoad(rearRoad, expandableRoad, newRoad, 0, -1);
                }
            }

            //Placing ahead segment of road (depending on side roads or on the probability).
            if (front)
            {
                if (InBounds(newRoad.x, newRoad.y))
                {
                    if (newRoads == 0 || Random.Range(0.0f, 1.0f) > frontProbability)
                        map[newRoad.x, newRoad.y] = 1;
                }
            }
        }
    }

    int ExpandSideRoad(Vector2Int rearRoad, Vector2Int expandableRoad, Vector2Int newRoad, int xOffset, int zOffset)
    {
        //Check if potential side road won't connect with existing road making loop.
        if (!IsRoad(rearRoad.x + xOffset, rearRoad.y + zOffset))
        {
            if (InBounds(expandableRoad.x + xOffset, expandableRoad.y + zOffset))
            {
                //Depenging on probability.
                if (Random.Range(0.0f, 1.0f) < sideProbability)
                {
                    //Placing side road.
                    map[expandableRoad.x + xOffset, expandableRoad.y + zOffset] = 1;
                    return 1;
                }
            }
        }

        return 0;
    }

    bool IsRoad(int x, int z)
    {
        if (!InBounds(x, z))
            return false;
        return (GetSegment(x, z) == 1 || GetSegment(x, z) == 2);
    }

    bool InBounds(int x, int z)
    {
        return !(x < 0 || z < 0 || x >= mapX || z >= mapZ);
    }

    int GetSegment(int x, int z)
    {
        if (!InBounds(x, z))
            return -1;
        else
            return map[x, z];
    }

    void SwapRoom(int destination, int value)
    {
        while (true)
        {
            int x = Random.Range(0, mapX);
            int z = Random.Range(0, mapZ);
            if(map[x,z] == destination)
            {
                map[x, z] = value;
                break;
            }
        }
    }

    void IstantiateRooms()
    {
        roomsList = new List<GameObject>();
        int centrumX = mapX / 2, centrumZ = mapZ / 2;

        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (map[x, z] != 0)
                {
                    GameObject toSpawn = null;

                    switch (map[x, z])
                    {
                        case 1:
                            toSpawn = EmptyRoom;
                            break;
                        case 2:
                            toSpawn = DoorRoom;
                            break;
                        case 3:
                            toSpawn = KeyRoom;
                            break;
                        case 4:
                            toSpawn = EnemyRoom;
                            break;
                        case 5:
                            toSpawn = PlayerRoom;
                            break;
                    }

                    GameObject room = Instantiate(toSpawn, new Vector3((x - centrumX) * (roomX + bridgeLength), 0, (z - centrumZ) * (roomZ + bridgeLength)), Quaternion.Euler(0, 0, 0));
                    roomsList.Add(room);
                    room.transform.parent = gameObject.transform;
                }

            }
        }
    }

    void IstantiateBridges()
    {
        bridgesList = new List<GameObject>();
        int centrumX = mapX / 2, centrumZ = mapZ / 2;

        for (int x = 0; x < mapX - 1; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if (map[x, z] != 0 && map[x + 1, z] != 0)
                {
                    GameObject bridge = Instantiate(Bridge, new Vector3((x - centrumX) * (roomX + bridgeLength) + roomX / 2 + bridgeLength / 2, 0, (z - centrumZ) * (roomZ + bridgeLength)), Quaternion.Euler(0, 0, 0));
                    bridge.transform.localScale = new Vector3(bridgeLength, 1, 2f);
                    bridge.transform.parent = gameObject.transform;
                    bridgesList.Add(bridge);
                }
            }
        }

        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ - 1; z++)
            {
                if (map[x, z] != 0 && map[x, z + 1] != 0)
                {
                    GameObject bridge = Instantiate(Bridge, new Vector3((x - centrumX) * (roomX + bridgeLength), 0, (z - centrumZ) * (roomZ + bridgeLength) + roomZ / 2 + bridgeLength / 2), Quaternion.Euler(0, 90, 0));
                    bridge.transform.localScale = new Vector3(bridgeLength, 1, 2f);
                    bridge.transform.parent = gameObject.transform;
                    bridgesList.Add(bridge);
                }
            }
        }
    }

    private void AllWalls()
    {
        foreach (GameObject room in roomsList)
        {
            room.GetComponent<SwitchingWalls>().AllWalls();
        }
        foreach (GameObject bridge in bridgesList)
        {
            bridge.GetComponent<SwitchingWalls>().AllWalls();
        }
    }

    private void OnlyInsideWalls()
    {
        foreach (GameObject room in roomsList)
        {
            room.GetComponent<SwitchingWalls>().OnlyInsideWalls();
        }
        foreach (GameObject bridge in bridgesList)
        {
            bridge.GetComponent<SwitchingWalls>().OnlyInsideWalls();
        }
    }

    private void NoWalls()
    {
        foreach (GameObject room in roomsList)
        {
            room.GetComponent<SwitchingWalls>().NoWalls();
        }
        foreach (GameObject bridge in bridgesList)
        {
            bridge.GetComponent<SwitchingWalls>().NoWalls();
        }
    }

    private void Update()
    {
        if (GameController.Instance.gameContinues)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("1");
                AllWalls();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("2");
                OnlyInsideWalls();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("3");
                NoWalls();
            }
        }
    }

}
