using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static List<GameObject> players = new List<GameObject>();
    [HideInInspector] public float Health { get; set; }
    [SerializeField] private float walkingSpeed, runningSpeed;
    [HideInInspector] public bool running = false;

    private float lastTimeClicked = 0;

    public static void Restart()
    {
        players = new List<GameObject>();
    }

    private void Start()
    {
        Health = 100;
        players.Add(gameObject);
    }

    void Update()
    {
        if(players.Count == 1)
        {
            if (PanelController.Instance != null)
            {
                PanelController.Instance.health.GetComponent<TextMeshProUGUI>().text = "" + (int)Health;
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
                running = true;
                GetComponent<NavMeshAgent>().speed = runningSpeed;
            }
            else
            {
                running = false;
                GetComponent<NavMeshAgent>().speed = walkingSpeed;
            }

            lastTimeClicked = Time.time;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast(ray, out hit) && !Array.Exists(Physics.RaycastAll(ray), element => element.transform.gameObject.tag == "Player"))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
        }

        if(Health <= 1)
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
            Health -= Time.deltaTime * 10;
            StartCoroutine(GameController.Instance.cameraNavigation.GetComponent<CameraShaking>().Shake(0.15f, 0.4f));
            GetComponent<ParticleSystem>().Play();
        }
    }

}
