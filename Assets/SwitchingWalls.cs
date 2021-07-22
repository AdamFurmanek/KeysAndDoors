using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWalls : MonoBehaviour
{
    public List<GameObject> InsideWalls, OutsideWalls;

    public void AllWalls()
    {
        foreach (GameObject wall in InsideWalls)
        {
            wall.SetActive(true);
        }
        foreach (GameObject wall in OutsideWalls)
        {
            wall.SetActive(true);
        }
    }

    public void OnlyInsideWalls()
    {
        foreach (GameObject wall in InsideWalls)
        {
            wall.SetActive(true);
        }
        foreach (GameObject wall in OutsideWalls)
        {
            wall.SetActive(false);
        }
    }

    public void NoWalls()
    {
        foreach (GameObject wall in InsideWalls)
        {
            wall.SetActive(false);
        }
        foreach (GameObject wall in OutsideWalls)
        {
            wall.SetActive(false);
        }
    }
}
