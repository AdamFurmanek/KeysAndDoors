using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

public class PlayerController : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit) && !Array.Exists(Physics.RaycastAll(ray), element => element.transform.gameObject.tag == "Player"))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
        }
    }
}
