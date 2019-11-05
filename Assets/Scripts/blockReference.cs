using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockReference : MonoBehaviour
{
    // block x, y, z and block type
    public int x;
    public int y;
    public int z;
    private string type;

    //set
    public void setReference(int x, int y, int z, string type)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.type = type;
    }
    //get
    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public int getZ() {
        return z;
    }

    public string getType() {
        return type;
    }
}
