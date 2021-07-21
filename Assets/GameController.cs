using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    public GameObject Menu, Restart, Panel;

    TimeSpan timePlaying;
    float elapsedTime;
    bool gameContinues;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        gameContinues = false;
    }

    void Start()
    {
        StartMenu();
    }

    public void StartMenu()
    {
        StartGame(0);
        Menu.SetActive(true);
        Restart.SetActive(false);
        Panel.SetActive(false);
    }

    public void StartGame(int players)
    {
        Menu.SetActive(false);
        Restart.SetActive(false);
        Panel.SetActive(true);

        EnemyController.Restart();
        Keys.Restart();
        PlayerController.Restart();

        StartCoroutine(WorldGenerator.Instance.GenerateWorld(players));
        gameContinues = true;
        elapsedTime = 0;
        StartCoroutine(UpdateTimer());
    }

    public void GameOver()
    {
        Menu.SetActive(false);
        Restart.SetActive(true);
        Panel.SetActive(true);
        gameContinues = false;
    }

    public void GameWon()
    {
        Menu.SetActive(false);
        Restart.SetActive(true);
        Panel.SetActive(true);
        gameContinues = false;
    }
    
    private IEnumerator UpdateTimer()
    {
        while (gameContinues)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timeString = "" + timePlaying.ToString("mm':'ss':'ff");
            if (PanelController.Instance != null)
            {
                PanelController.Instance.time.GetComponent<TextMeshProUGUI>().text = timeString;
            }

            yield return null;
        }
    }
    
}
