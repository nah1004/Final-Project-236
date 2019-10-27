using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterMovement : MonoBehaviour
{
    public GameObject cam;
    public GameObject camHolder;
    public float speed;
    [Range(0,10)]
    public float jumpForce;
    float camRotX;
    float camRotY;
    [Range(.1f,9.99f)]
    public float lookSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
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

        if (Input.GetKeyDown(KeyCode.Space)) {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce * 100, 0));
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
        Debug.Log(camRotX);
    }
}
