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
    public bool gameContinues;

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
        if (players > 0)
        {
            PlaySong(1);
        }
        else
        {
            PlaySong(0);
        }

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
        PlaySound(5);
        PlaySong(3);
        menu.SetActive(false);
        restart.SetActive(true);
        panel.SetActive(true);
        gameContinues = false;
        gameEnds.GetComponent<TextMeshProUGUI>().text = "Game Over";
        bestScore.GetComponent<TextMeshProUGUI>().text = "best score: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("HighScore", 3599.99f)).ToString("mm':'ss':'ff");

    }

    public void GameWon()
    {
        PlaySound(4);
        PlaySong(2);
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

    void PlaySong(int index)
    {
        for(int i = 0; i < 4; i++)
        {
            if(i != index)
            {
                transform.GetChild(i).GetComponent<Song>().Stop();
            }
            else
            {
                transform.GetChild(i).GetComponent<Song>().Play();
            }
        }
    }

    void PlaySound(int index)
    {
        transform.GetChild(index).GetComponent<Song>().Play();
    }
}
