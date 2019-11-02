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
    [Range(0,10)]
    // players jump force
    public float jumpForce;
    // quaternion for camera rotation
    float camRotX;
    float camRotY;
    // look sensitivity
    [Range(.1f,9.99f)]
    public float lookSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        //hide cursor and lock the curson in the center
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
        else if (Input.GetKey(KeyCode.S)) {
            gameObject.transform.Translate(camHolder.transform.forward * -speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(camHolder.transform.right * -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(camHolder.transform.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            gameObject.transform.Translate(camHolder.transform.up * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            gameObject.transform.Translate(camHolder.transform.up * -speed * Time.deltaTime);
        }
    

        camRotX -= Input.GetAxis("Mouse Y") * lookSensitivity;
        camRotY += Input.GetAxis("Mouse X") * lookSensitivity;
        if (camRotX > 85) {
            camRotX = 85;
        }
        if (camRotX < -85) {
            camRotX = -85;
        }
        camHolder.transform.rotation = Quaternion.Euler(0, camRotY, 0);
        cam.transform.rotation = Quaternion.Euler(camRotX, camRotY, 0);
    }
}
