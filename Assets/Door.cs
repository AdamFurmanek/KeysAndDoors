using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(Keys.howManyFounded == Keys.howManyExist)
            {
                GameController.Instance.GameWon();
                PlayerController.players = new List<GameObject>();
                GameObject.Destroy(other.gameObject);
            }
            else
            {

            }
        }
    }
}
