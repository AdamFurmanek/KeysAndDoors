using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject EmptyRoom, DoorRoom, KeyRoom, Bridge;

    [SerializeField] private int mapX, mapZ;
    [SerializeField] private float roomX, roomZ, bridgeLength;
    [SerializeField] [Range(0, 1)] private float frontProbability, sideProbability;

    private int[,] map;

    void Awake()
    {
        //Reset map to zeros.
        map = new int[mapX, mapZ];
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                map[x, z] = 0;
            }
        }

        //Initial cross in the center of the map.
        int centrumX = mapX / 2, centrumZ = mapZ / 2;
        map[centrumX, centrumZ] = 1;
        map[centrumX - 1, centrumZ] = 1;
        map[centrumX + 1, centrumZ] = 1;
        map[centrumX, centrumZ - 1] = 1;
        map[centrumX, centrumZ + 1] = 1;

        //Generate empty rooms.
        for (int i = 0; i < 100; i++)
        {
            ExpandRoad(1, true, true);
        }

        //Generate rooms with key and door.
        ExpandRoad(2, true, false);
        ExpandRoad(3, true, false);

        //Istantiating.
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if(map[x, z] != 0)
                {
                    GameObject toSpawn = null;

                    switch(map[x, z])
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
                    }

                    Instantiate(toSpawn, new Vector3((x - centrumX) * (roomX + bridgeLength), 0, (z - centrumZ) * (roomZ + bridgeLength)), Quaternion.Euler(0, 0, 0));
                }

            }
        }

        for (int x = 0; x < mapX - 1; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                if(map[x,z] != 0 && map[x + 1, z] != 0)
                {
                    GameObject bridge = Instantiate(Bridge, new Vector3((x - centrumX) * (roomX + bridgeLength) + roomX/2 + bridgeLength/2, 0, (z - centrumZ) * (roomZ + bridgeLength)), Quaternion.Euler(0, 0, 0));
                    bridge.transform.localScale = new Vector3(bridgeLength/10, 1, 0.2f);
                }
            }
        }

        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ - 1; z++)
            {
                if (map[x, z] != 0 && map[x, z + 1] != 0)
                {
                    GameObject bridge = Instantiate(Bridge, new Vector3((x - centrumX) * (roomX + bridgeLength), 0, (z - centrumZ) * (roomZ + bridgeLength) + roomZ / 2 + bridgeLength / 2), Quaternion.Euler(0, 0, 0));
                    bridge.transform.localScale = new Vector3(0.2f, 1, bridgeLength / 10);
                }
            }
        }

    }

    struct ExpandableRoad
    {
        public Vector2Int expandableRoad; //End of the road that can be expanded.
        public Vector2Int rearRoad; //Only road connected to expandableRoad.
    }

    void ExpandRoad(int type, bool front, bool side)
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
                    newRoads += ExpandSideRoad(type, rearRoad, expandableRoad, newRoad, 1, 0);

                    newRoads += ExpandSideRoad(type, rearRoad, expandableRoad, newRoad, -1, 0);
                }

                else
                {
                    newRoads += ExpandSideRoad(type, rearRoad, expandableRoad, newRoad, 0, 1);

                    newRoads += ExpandSideRoad(type, rearRoad, expandableRoad, newRoad, 0, -1);
                }
            }

            //Placing ahead segment of road (depending on side roads or on the probability).
            if (front)
            {
                if (InBounds(newRoad.x, newRoad.y))
                {
                    if (newRoads == 0 || Random.Range(0.0f, 1.0f) > frontProbability)
                        map[newRoad.x, newRoad.y] = type;
                }
            }
        }
    }

    int ExpandSideRoad(int type, Vector2Int rearRoad, Vector2Int expandableRoad, Vector2Int newRoad, int xOffset, int zOffset)
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
                    map[expandableRoad.x + xOffset, expandableRoad.y + zOffset] = type;
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

}
