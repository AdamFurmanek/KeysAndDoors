using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighting : MonoBehaviour
{

    void OnMouseEnter()
    {
        if (GameController.Instance.gameContinues)
        {
            Material material = GetComponent<MeshRenderer>().material;
            material.SetColor("_Color", material.color + new Color(0.2f, 0.2f, 0.2f));
        }
    }

    void OnMouseExit()
    {
        if (GameController.Instance.gameContinues)
        {
            Material material = GetComponent<MeshRenderer>().material;
            material.SetColor("_Color", material.color - new Color(0.2f, 0.2f, 0.2f));
        }
    }
}
