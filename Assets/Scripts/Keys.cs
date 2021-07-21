using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keys : MonoBehaviour
{
    public static int howManyExist = 0;
    public static int howManyFounded = 0;

    public static void Restart()
    {
        howManyExist = 0;
        howManyFounded = 0;
    }

    void Start()
    {
        howManyExist++;
        if (PanelController.Instance != null)
        {
            PanelController.Instance.keys.GetComponent<TextMeshProUGUI>().text = howManyFounded + "/" + howManyExist;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            howManyFounded++;
            PanelController.Instance.keys.GetComponent<TextMeshProUGUI>().text = howManyFounded + "/" + howManyExist;
            gameObject.SetActive(false);
        }
    }

}
