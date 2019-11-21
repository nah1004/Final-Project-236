using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterMovement : MonoBehaviour
{
    // reference to the scenes camera
    public GameObject cam;
    // reference to the camera container
    public GameObject camHolder;
    //players movement speed
    public float speed;
    private int clampAngle;

    // quaternion for camera rotation
    float camRotX;
    float camRotY;
    // look sensitivity
    [Range(.1f,9.99f)]
    public float lookSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        clampAngle = 90;
        //hide cursor and lock the curson in the center screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        //if press escape unlock cursor and make it visible
        if (Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //press w
        if (Input.GetKey(KeyCode.W))
        {
            //move the character forward relative to the camera container by speed per unit time
            gameObject.transform.Translate(camHolder.transform.forward * speed * Time.deltaTime);
        }
        //press s
        else if (Input.GetKey(KeyCode.S)) {
            // move backwards
            gameObject.transform.Translate(camHolder.transform.forward * -speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            //move left
            gameObject.transform.Translate(camHolder.transform.right * -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // move right
            gameObject.transform.Translate(camHolder.transform.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            // move up
            gameObject.transform.Translate(camHolder.transform.up * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            // move down
            gameObject.transform.Translate(camHolder.transform.up * -speed * Time.deltaTime);
        }
    
        // get mouse position
        camRotX -= Input.GetAxis("Mouse Y") * lookSensitivity;
        camRotY += Input.GetAxis("Mouse X") * lookSensitivity;
        // clamp input to look angle so it doesnt exceed reasonable angles
        if (camRotX > clampAngle) {
            camRotX = clampAngle;
        }
        if (camRotX < -clampAngle) {
            camRotX = -clampAngle;
        }
        // rotate camera accordingly
        camHolder.transform.rotation = Quaternion.Euler(0, camRotY, 0);
        cam.transform.rotation = Quaternion.Euler(camRotX, camRotY, 0);
    }
}