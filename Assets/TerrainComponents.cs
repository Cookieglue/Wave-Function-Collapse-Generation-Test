using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainComponents : MonoBehaviour
{
    public Mesh mesh;
    public Dictionary<string, SuperPositionType> subSuperposition = new  Dictionary<string, SuperPositionType>(); 
    public int rotation;
    public int order;

    public TerrainComponents (Mesh mesh, Dictionary<string, SuperPositionType> subSuperposition, int rotation, int order){

        this.mesh = mesh;
        this.subSuperposition = subSuperposition;
        this.rotation = rotation;
        this.order = order;

    }

}
