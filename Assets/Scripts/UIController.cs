using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public RawImage img;
    public Text txt;
    private cellMain cellMain;
    public Texture2D[] textures;

    private void Start()
    {
        cellMain = gameObject.GetComponent<cellMain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cellMain.placeIndex == 3) {
            txt.text = "Grassy Dirt";
            img.texture = textures[3];
        }
        else if (cellMain.placeIndex == 4) {
            txt.text = "Wood";
            img.texture = textures[4];
        }
        else if (cellMain.placeIndex == 5){
            txt.text = "Stone";
            img.texture = textures[5];
        }
        else if (cellMain.placeIndex == 6){
            txt.text = "Flowing Water";
            img.texture = textures[6];
        }
        else if (cellMain.placeIndex == 7)
        {
            txt.text = "Sitting Water";
            img.texture = textures[7];
        }
        else if (cellMain.placeIndex == 8)
        {
            txt.text = "Leaf";
            img.texture = textures[8];
        }
        else if (cellMain.placeIndex == 9)
        {
            txt.text = "Vine";
            img.texture = textures[9];
        }
        else if (cellMain.placeIndex == 10)
        {
            txt.text = "Vine Head";
            img.texture = textures[10];
        }
    }
}
