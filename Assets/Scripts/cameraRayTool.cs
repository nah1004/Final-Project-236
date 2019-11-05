using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRayTool : MonoBehaviour
{
    //references
    public Camera cam;
    public GameObject cursor;
    private cellMain cellMain;
    private blockReference blockR;
    // adjust hit point based on normal vector plus this value
    private const int SCANNORMALADJUSTMENT = 3;
    // how far does the ray go
    private const int RAYMULTIPLIER = 1000;
    // mouse input numbers
    private const int LEFTCLICK = 0;
    private const int RIGHTCLICK = 1;
    private const int MIDDLECLICK = 2;
    // Show cursor or not
    private bool cursorBool;


    public void Start()
    {
        cellMain = gameObject.GetComponent<cellMain>();
    }

    // Updates the cursor for the user so they know where they are looking
    public void updateCursor(bool isRenderable, Vector3 pos)
    {
        // make cursor visible or not
        if (isRenderable)
        {
            cursor.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            cursor.GetComponent<MeshRenderer>().enabled = false;
        }

        cursor.transform.position = pos;
    }

    //Destroy Block
    public void DestroyBlock(GameObject obj) {
        if (obj.tag == "block") {
            //sends to cell main
            blockR = obj.GetComponentInParent<blockReference>();
            cellMain.deleteBlock(blockR.getX(), blockR.getY(), blockR.getZ(), blockR.getType());
        }
    }

    //Add Block

    // Update is called once per frame
    void Update()
    {
            // bitshift layers so we can exclude the raycast ignore layer from calculation
            int layerMask = 1 << 2;
            layerMask = ~layerMask;
            RaycastHit hit;

        //scan
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask) && Input.GetMouseButtonDown(MIDDLECLICK))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.magenta);
            cursorBool = true;
            updateCursor(cursorBool, hit.point);
            //scan the block to add it to inventory
                //TODO
        }
        //delete
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask) && Input.GetMouseButtonDown(RIGHTCLICK))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            cursorBool = true;
            updateCursor(cursorBool, hit.point);
            // Destroy the block
            DestroyBlock(hit.collider.gameObject);
        }
        //place
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask) && Input.GetMouseButtonDown(RIGHTCLICK))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            cursorBool = true;
            updateCursor(cursorBool, hit.point);
            //Add a block
            blockReference thisRef = hit.collider.gameObject.GetComponentInParent<blockReference>();
            cellMain.createBlock(Mathf.RoundToInt(hit.point.x + (hit.normal.x / SCANNORMALADJUSTMENT) + (cellMain.mapX / 2)), Mathf.RoundToInt(hit.point.y + (hit.normal.y / SCANNORMALADJUSTMENT) + (cellMain.mapY / 2)), Mathf.RoundToInt(hit.point.z + (hit.normal.z / SCANNORMALADJUSTMENT) + (cellMain.mapZ / 2)), "dirt");
        }
        //looking at block so render
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);
            cursorBool = true;
            updateCursor(cursorBool, hit.point);
        }
        //looking into the void thinking about your C# grade, crying and eating a box of cookies by yourself.. it doesn't help
        else
        {
            // hide the cursor
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * RAYMULTIPLIER, Color.black);
            cursorBool = false;
            updateCursor(cursorBool, hit.point);
        }

       
    }
}


