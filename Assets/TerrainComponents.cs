using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainComponents : MonoBehaviour
{
    public Mesh mesh;
    
    public Dictionary<string, Mesh> subSuperposition = new  Dictionary<string, Mesh>();
    public int rotation;

}
