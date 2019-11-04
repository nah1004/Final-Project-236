using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeFilter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "block") {
            other.gameObject.layer = 2;
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        other.gameObject.layer = 0;
    }

}
