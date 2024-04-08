using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfoDisplay : MonoBehaviour
{
    [HideInInspector]
    public int id;
    [HideInInspector]
    public float height;
    [HideInInspector]
    public float width;
    [HideInInspector]
    public float hwRatio;
    [HideInInspector]
    public int connections;
    [HideInInspector]
    public float area;

    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    private void Awake()
    {
        

    }

    private void OnMouseOver()
    {

    }

}
