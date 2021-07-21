using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent navMesh;

    private static int howManyExist = 0;

    void Start()
    {
        howManyExist++;
        PanelController.Instance.enemies.GetComponent<TextMeshProUGUI>().text = "" + howManyExist;

        navMesh = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(PlayerController.players.Count > 0)
        {
            GameObject target = null;
            float minDistance = 100000;
            foreach(GameObject p in PlayerController.players)
            {
                float distance = Vector3.Distance(gameObject.transform.position, p.transform.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    target = p;
                }
            }

            navMesh.SetDestination(target.transform.position);
        }
    }
}
