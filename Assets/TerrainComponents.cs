using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainComponents : MonoBehaviour
{
    public Mesh mesh;
    public Dictionary<string, SuperPositionType> subSuperposition = new  Dictionary<string, SuperPositionType>(); 
    public int rotation;
    public int entropy;

    public int[] position;

    public TerrainComponents (Mesh mesh, Dictionary<string, SuperPositionType> subSuperposition, int rotation, int entropy, int[] position){

        this.mesh = mesh;
        this.subSuperposition = subSuperposition;
        this.rotation = rotation;
        this.entropy = entropy;
        this.position = position;

    }

}
