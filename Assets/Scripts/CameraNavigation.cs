using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNavigation : MonoBehaviour
{
    private GameObject directionPoint;
    private Camera camera;
    private GameObject trackedObject;
    [SerializeField] private float zoomSensitivity, moveSensitivity, rotationSensitivity;

    void Awake()
    {
        directionPoint = transform.Find("DirectionPoint").gameObject;
        camera = directionPoint.transform.Find("Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (GameController.Instance.GameContinues)
        {

            //Zooming camera.
            Vector3 newPosition = camera.transform.localPosition + Vector3.down * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSensitivity;
            newPosition.y = Mathf.Clamp(newPosition.y, 1, 200);
            camera.transform.localPosition = newPosition;

            //Tracking object.
            if (trackedObject != null)
            {
                directionPoint.transform.position = trackedObject.transform.position;
            }

            //Moving camera.
            if (Input.GetMouseButton(1))
            {
                trackedObject = null;
                Vector3 desiredMove = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")) * Time.deltaTime * moveSensitivity;

                desiredMove = Quaternion.Euler(new Vector3(0f, directionPoint.transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = directionPoint.transform.InverseTransformDirection(desiredMove);

                directionPoint.transform.Translate(desiredMove, Space.Self);
            }

            //Selecting object.
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    if (hit.transform.gameObject.tag == "Player")
                    {
                        trackedObject = hit.transform.gameObject;
                    }
                }
            }

            //Rotating camera.
            if (Input.GetMouseButton(2))
            {
                Vector3 newRotation = directionPoint.transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSensitivity;
                newRotation.x = Mathf.Clamp(newRotation.x, 270, 359);

                directionPoint.transform.localEulerAngles = newRotation;
            }

            //Moving camera with keyboard + Escape for back to menu.
            Keyboard();

        }
        else
        {
            //Rotating camera in menu/endgame board.
            directionPoint.transform.eulerAngles += new Vector3(0, Time.deltaTime * 4, 0);
        }
    }

    public void Keyboard()
    {
        //Back to menu.
        if (Input.GetKeyDown(KeyCode.Escape) && GameController.Instance.GameContinues)
        {
            GameController.Instance.StartMenu();
        }

        //WASD to move camera.
        float x = 0, z = 0;

        if (Input.GetKey(KeyCode.W))
        {
            z += Time.deltaTime * moveSensitivity / 20;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x -= Time.deltaTime * moveSensitivity / 20;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z -= Time.deltaTime * moveSensitivity / 20;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += Time.deltaTime * moveSensitivity / 20;
        }

        if(x != 0 || z != 0)
        {
            trackedObject = null;

            Vector3 desiredMove = new Vector3(x, 0, z) * Time.deltaTime * moveSensitivity;

            desiredMove = Quaternion.Euler(new Vector3(0f, directionPoint.transform.eulerAngles.y, 0f)) * desiredMove;
            desiredMove = directionPoint.transform.InverseTransformDirection(desiredMove);

            directionPoint.transform.Translate(desiredMove, Space.Self);
        }

        //Rotating camera with Q and E.
        float y = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            y += Time.deltaTime * rotationSensitivity / 20;
        }
        if (Input.GetKey(KeyCode.E))
        {
            y -= Time.deltaTime * rotationSensitivity / 20;
        }
        Vector3 newRotation = directionPoint.transform.localEulerAngles + new Vector3(0, y, 0) * Time.deltaTime * rotationSensitivity;
        newRotation.x = Mathf.Clamp(newRotation.x, 270, 359);

        directionPoint.transform.localEulerAngles = newRotation;

        //Tracking object with R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (PlayerController.players.Count > 0)
            {
                trackedObject = PlayerController.players[0];
            }
        }
    }

    public void ResetCamera()
    {
        directionPoint.transform.position = new Vector3(0, 0, 0);
        camera.transform.localPosition = new Vector3(0, 200, 0);
        directionPoint.transform.eulerAngles = new Vector3(-60, 0, 0);
    }

}
