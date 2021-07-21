using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    public GameObject menu, restart, panel;
    public GameObject gameEnds, bestScore;
    public GameObject worldGenerator, cameraNavigation;

    TimeSpan timePlaying;
    float elapsedTime;
    public static bool gameContinues;

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
        menu.SetActive(true);
        restart.SetActive(false);
        panel.SetActive(false);
        gameContinues = false;
    }

    public void StartGame(int players)
    {
        menu.SetActive(false);
        restart.SetActive(false);
        panel.SetActive(true);

        EnemyController.Restart();
        Keys.Restart();
        PlayerController.Restart();

        StartCoroutine(worldGenerator.GetComponent<WorldGenerator>().GenerateWorld(players));
        gameContinues = true;
        elapsedTime = 0;
        StartCoroutine(UpdateTimer());

        cameraNavigation.GetComponent<CameraNavigation>().ResetCamera();
    }

    public void GameOver()
    {
        menu.SetActive(false);
        restart.SetActive(true);
        panel.SetActive(true);
        gameContinues = false;
        gameEnds.GetComponent<TextMeshProUGUI>().text = "Game Over";
        bestScore.GetComponent<TextMeshProUGUI>().text = "best score: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("HighScore", 3599.99f)).ToString("mm':'ss':'ff");

    }

    public void GameWon()
    {
        menu.SetActive(false);
        restart.SetActive(true);
        panel.SetActive(true);
        gameContinues = false;
        if(elapsedTime < PlayerPrefs.GetFloat("HighScore", 3599.99f))
        {
            PlayerPrefs.SetFloat("HighScore", elapsedTime);
        }
        gameEnds.GetComponent<TextMeshProUGUI>().text = "You won!";
        bestScore.GetComponent<TextMeshProUGUI>().text = "best score: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("HighScore", 3599.99f)).ToString("mm':'ss':'ff");
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
