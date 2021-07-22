using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private float health = 100;
    public static List<GameObject> players = new List<GameObject>();
    float lastTimeClicked = 0;

    public static void Restart()
    {
        players = new List<GameObject>();
    }

    private void Start()
    {
        players.Add(gameObject);
    }

    void Update()
    {
        if(players.Count == 1)
        {
            if (PanelController.Instance != null)
            {
                PanelController.Instance.health.GetComponent<TextMeshProUGUI>().text = "" + (int)health;
            }
        }
        else
        {
            if (PanelController.Instance != null)
            {
                PanelController.Instance.health.GetComponent<TextMeshProUGUI>().text = "" + players.Count;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastTimeClicked < 0.2)
            {
                GetComponent<NavMeshAgent>().speed = 40;
            }
            else
            {
                GetComponent<NavMeshAgent>().speed = 20;
            }

            lastTimeClicked = Time.time;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit) && !Array.Exists(Physics.RaycastAll(ray), element => element.transform.gameObject.tag == "Player"))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
        }

        if(health <= 1)
        {
            GameController.Instance.GameOver();
            Restart();
            GameObject.Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            health -= Time.deltaTime * 10;
            StartCoroutine(GameController.Instance.cameraNavigation.GetComponent<CameraShaking>().Shake(0.15f, 0.4f));
            GetComponent<ParticleSystem>().Play();
        }
    }

}
