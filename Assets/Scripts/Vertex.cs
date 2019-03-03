using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    //Each vertex stores it's position
    public Vector3 pos;

    public bool active;

    public Vertex(Vector3 _pos, bool _active)
    {
        pos = _pos;
        active = _active;
    }
}
