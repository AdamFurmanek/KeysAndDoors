using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(Key.HowManyFounded == Key.HowManyExist)
            {
                GameController.Instance.GameWon();
                PlayerController.Restart();
                GameObject.Destroy(other.gameObject);
            }
            else
            {
                //Inform player that he have not enough keys.
            }
        }
    }
}
