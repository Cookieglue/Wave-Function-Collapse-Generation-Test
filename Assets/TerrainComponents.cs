using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainComponents : MonoBehaviour
{
    public Mesh mesh;
    public Dictionary<string, SuperPositionType> subSuperposition = new  Dictionary<string, SuperPositionType>();
    public int rotation;

}
