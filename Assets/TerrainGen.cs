using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime;

public class TerrainGen : MonoBehaviour
{
    public int chunkSize = 15;
    private Dictionary<string, SuperPositionType> superposition = new  Dictionary<string, SuperPositionType>();

    private TerrainComponents[][] terrainComponents;
    private MeshFilter meshFilter;

    private System.Random rand = new System.Random();
    [SerializeField] private GameObject positionObject;

    [SerializeField] private Mesh[] meshes;

    private GameObject[][] tempGameobjects;

    void Start(){

        meshFilter = GetComponent<MeshFilter>();

        //superposition = JsonConvert.DeserializeObject<Dictionary<string, SuperPositionType>>("/SuperPositionRegistry.json");

        InitializeChunk();

        int netEntropy = 1;
        while (netEntropy > 0){
            netEntropy = 0;
            CollapseWave();
        }

        CombineMeshes();

    }

    void InitializeChunk(){

        terrainComponents = new TerrainComponents[chunkSize][];
        tempGameobjects = new GameObject[chunkSize][];

        //set arrays
        for (int x = 0 ; x < terrainComponents.Length ;  x++){

            terrainComponents[x] = new TerrainComponents[chunkSize];
            tempGameobjects[x] = new GameObject[chunkSize];
            for (int y = 0 ; y < terrainComponents[x].Length ;  y++){

                //gameobject is only for positions (idk any other way)
                tempGameobjects[x][y] = Instantiate(positionObject, new Vector3(x, 0 ,y), Quaternion.identity);

                //set terrainComponents and allow all possibilities
                terrainComponents[x][y] = new TerrainComponents();
                terrainComponents[x][y].subSuperposition = new Dictionary<string, SuperPositionType>(superposition);
            }
            
        }


    }

    void CollapseWave(){

        int x = 0;
        int y = 0;

        //load mesh randomly from possible states
        string meshPath = terrainComponents[x][y].subSuperposition.ElementAt(rand.Next(0, terrainComponents[x][y].subSuperposition.Count)).Value.mesh;
        terrainComponents[x][y].mesh = Resources.Load<Mesh>(meshPath);




    }

    void CombineMeshes(){

        //combine meshes to make compooter happi

        CombineInstance[] combine = new CombineInstance[chunkSize*chunkSize];

        for (int x = 0 ; x <= chunkSize-1 ;  x++){

            for (int y = 0 ; y <= chunkSize-1 ;  y++){
                
                combine[x + y * chunkSize].mesh = terrainComponents[x][y].mesh;
                combine[x + y * chunkSize].transform = tempGameobjects[x][y].transform.localToWorldMatrix;

                Destroy(tempGameobjects[x][y]);
            }

        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

    }


}
