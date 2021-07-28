using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Key : MonoBehaviour
{
    public static int HowManyExist { get; set; }
    public static int HowManyFounded { get; set; }

    public static void Restart()
    {
        HowManyExist = 0;
        HowManyFounded = 0;
    }

    void Start()
    {
        HowManyExist++;
        PanelController.Instance.keys.GetComponent<TextMeshProUGUI>().text = HowManyFounded + "/" + HowManyExist;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            HowManyFounded++;
            PanelController.Instance.keys.GetComponent<TextMeshProUGUI>().text = HowManyFounded + "/" + HowManyExist;
            gameObject.SetActive(false);
        }
    }

}
