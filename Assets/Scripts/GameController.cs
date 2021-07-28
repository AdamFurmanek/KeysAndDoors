using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    [SerializeField] private GameObject menu, restart, panel, instruction;
    [SerializeField] private GameObject gameEnds, bestScore;
    public GameObject worldGenerator, cameraNavigation;

    private TimeSpan timePlaying;
    private float elapsedTime;
    public bool GameContinues { get; set; }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        StartMenu();
    }

    public void StartMenu()
    {
        StartGame(0);

        //Setting visible panels.
        menu.SetActive(true);
        restart.SetActive(false);
        panel.SetActive(false);

        //Setting GameContinues property.
        GameContinues = false;
    }

    public void StartGame(int players)
    {
        //If players > 0 - we are in game
        if (players > 0)
        {
            PlaySong(1);
        }
        //Else - we are i menu
        else
        {
            PlaySong(0);
        }

        //Setting visible panels.
        menu.SetActive(false);
        restart.SetActive(false);
        panel.SetActive(true);

        //Restarting values.
        EnemyController.Restart();
        Key.Restart();
        PlayerController.Restart();

        //Generating map.
        StartCoroutine(worldGenerator.GetComponent<WorldGenerator>().GenerateWorld(players));

        //Setting GameContinues property.
        GameContinues = true;

        //Timer.
        elapsedTime = 0;
        StartCoroutine(UpdateTimer());

        //Reset camera settings.
        cameraNavigation.GetComponent<CameraNavigation>().ResetCamera();
    }

    public void GameOver()
    {
        PlaySound(5);
        PlaySong(3);
        menu.SetActive(false);
        restart.SetActive(true);
        panel.SetActive(true);
        GameContinues = false;
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
        GameContinues = false;
        if(elapsedTime < PlayerPrefs.GetFloat("HighScore", 3599.99f))
        {
            PlayerPrefs.SetFloat("HighScore", elapsedTime);
        }
        gameEnds.GetComponent<TextMeshProUGUI>().text = "You won!";
        bestScore.GetComponent<TextMeshProUGUI>().text = "best score: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("HighScore", 3599.99f)).ToString("mm':'ss':'ff");
    }

    public void OpenInstruction()
    {
        instruction.SetActive(true);
    }

    public void CloseInstruction()
    {
        instruction.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
    IEnumerator UpdateTimer()
    {
        while (GameContinues)
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

    //Turn off all songs except choosen one.
    void PlaySong(int index)
    {
        //Childs with indexes 0 - 3 are songs.
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

    //Just play sound.
    void PlaySound(int index)
    {
        //Childs with indexes > 3 are sounds.
        transform.GetChild(index).GetComponent<Song>().Play();
    }
}
