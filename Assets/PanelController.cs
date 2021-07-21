using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    private static PanelController _instance;
    public static PanelController Instance { get { return _instance; } }

    public GameObject enemies, keys, health, time;

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
    }

}
