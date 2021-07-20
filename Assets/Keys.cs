using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    public static int howManyExist = 0;
    public static int howManyFounded = 0;

    void Start()
    {
        howManyExist++;
    }

    void OnTriggerEnter(Collider other)
    {
        howManyFounded++;
        gameObject.SetActive(false);
    }

}
