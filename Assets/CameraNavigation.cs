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
                trackedObject = hit.transform.gameObject;
                ResetZoom();
            }
        }

        //Rotating camera.
        if (Input.GetMouseButton(2))
        {
            Vector3 newRotation = directionPoint.transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotationSensitivity;
            newRotation.x = Mathf.Clamp(newRotation.x, 270, 359);

            directionPoint.transform.localEulerAngles = newRotation;
        }
    }

    public void ResetZoom()
    {
        Vector3 newPosition = camera.transform.localPosition;
        newPosition.y = 3;
        newPosition.y = Mathf.Clamp(newPosition.y, 1, 200);
        camera.transform.localPosition = newPosition;
    }
}
