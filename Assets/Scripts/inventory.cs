using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour
{
    private cellMain cellMain;

    // Start is called before the first frame update
    void Start()
    {
        cellMain = gameObject.GetComponent<cellMain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            cellMain.updatePlaceIndex("up");
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            cellMain.updatePlaceIndex("down");
        }
    }
}