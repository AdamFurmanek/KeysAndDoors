using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

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
        StartGame();
    }

    public void StartGame()
    {
        WorldGenerator.Instance.GenerateWorld(1);
        gameContinues = true;
        elapsedTime = 0;
        StartCoroutine(UpdateTimer());
    }

    public void GameOver()
    {
        gameContinues = false;
    }

    public void GameWon()
    {
        gameContinues = false;
    }
    
    private IEnumerator UpdateTimer()
    {
        while (gameContinues)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timeString = "" + timePlaying.ToString("mm':'ss':'ff");
            PanelController.Instance.time.GetComponent<TextMeshProUGUI>().text = timeString;

            yield return null;
        }
    }
    
}
