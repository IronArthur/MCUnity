using UnityEngine;
using System.Collections;

// Camera Controller
// Revision 2
// Allows the camera to move left, right, up and down along a fixed axis.
// Attach to a camera GameObject (e.g MainCamera) for functionality.

public class CameraController : MonoBehaviour
{

    // How fast the camera moves
    int cameraVelocity = 10;

    float curZoomPos, zoomTo; // curZoomPos will be the value
    float zoomFrom = 5f; //Midway point between nearest and farthest zoom values (a "starting position")

    Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = this.GetComponent<Camera>();

        // Set the initial position of the camera.
        // Right now we don't actually need to set up any other variables as
        // we will start with the initial position of the camera in the scene editor
        // If you want to create cameras dynamically this will be the place to
        // set the initial transform.positiom.x/y/z
    }

    // Update is called once per frame
    void Update()
    {
        // Left
        if ((Input.GetKey(KeyCode.LeftArrow)) || Input.GetKey(KeyCode.A))
        {
            transform.Translate((Vector3.left * cameraVelocity) * Time.deltaTime);
        }
        // Right
        if ((Input.GetKey(KeyCode.RightArrow))|| Input.GetKey(KeyCode.D))
        {
            transform.Translate((Vector3.right * cameraVelocity) * Time.deltaTime);
        }
        // Up
        if ((Input.GetKey(KeyCode.UpArrow))|| Input.GetKey(KeyCode.W))
        {
            transform.Translate((Vector3.up * cameraVelocity) * Time.deltaTime);
        }
        // Down
        if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
        {
            transform.Translate((Vector3.down * cameraVelocity) * Time.deltaTime);
        }

        //Zooms
        if (Input.GetKey(KeyCode.KeypadPlus))
            cam.orthographicSize -= .1f;
        if (Input.GetKey(KeyCode.KeypadMinus))
            cam.orthographicSize += .1f;

        float y = Input.mouseScrollDelta.y;

        if (y >= 1)
        {
            zoomTo -= 0.1f;
        }

         // If the wheel goes down, increment 5 to "zoomTo"
        else if (y <= -1)
        {
            zoomTo += 0.1f;
        }

        // creates a value to raise and lower the camera's field of view
        curZoomPos = zoomFrom + zoomTo;

        curZoomPos = Mathf.Clamp(curZoomPos, 2f, 10f);

        // Makes the actual change to Field Of View
        cam.orthographicSize = curZoomPos;


    }
}