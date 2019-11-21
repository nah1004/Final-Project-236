using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeFilter : MonoBehaviour
{
    // Set render layer constants
    private const int DEFAULTLAYER = 0;
    private const int IGNORERAYCASTLAYER = 2;

    // if cube enters player trigger
    private void OnTriggerEnter(Collider other)
    {
        // ignore raycast and make invisible
        if (other.gameObject.tag == "block") {
            other.gameObject.layer = IGNORERAYCASTLAYER;
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //acknowledge raycast and make visible
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        other.gameObject.layer = DEFAULTLAYER;
    }

}