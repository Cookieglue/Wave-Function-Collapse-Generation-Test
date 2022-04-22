using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime;

public class TerrainGen : MonoBehaviour
{
    
    public int chunkSize = 15;
    private Dictionary<string, Mesh> superposition = new  Dictionary<string, Mesh>();

    private TerrainComponents[][] terrainComponents;
    private MeshFilter meshFilter;

    private System.Random rand = new System.Random();
    [SerializeField] private GameObject positionObject;

    [SerializeField] private Mesh[] meshes;

    void Start(){

        meshFilter = GetComponent<MeshFilter>();
        InitializeDictionary();
        InitializeChunk();

    }

    void InitializeChunk(){

        terrainComponents = new TerrainComponents[chunkSize][];
        GameObject[][] tempGameobjects = new GameObject[chunkSize][];

        //set arrays
        for (int x = 0 ; x < terrainComponents.Length ;  x++){

            terrainComponents[x] = new TerrainComponents[chunkSize];
            tempGameobjects[x] = new GameObject[chunkSize];
            for (int y = 0 ; y < terrainComponents[x].Length ;  y++){

                //gameobject is only for positions (idk any other way)
                tempGameobjects[x][y] = Instantiate(positionObject, new Vector3(x, 0 ,y), Quaternion.identity);

                //set terrainComponents and allow all possibilities
                terrainComponents[x][y] = new TerrainComponents();
                terrainComponents[x][y].subSuperposition = new Dictionary<string, Mesh>(superposition);
            }
            
        }
        //generate
        for (int x = 0 ; x < terrainComponents.Length ;  x++){

            for (int y = 0 ; y < terrainComponents[x].Length ;  y++){

                CollapseWave(x,y);
                
            }

        }
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
    void InitializeDictionary(){

        superposition.Add("flat", meshes[0]);
        superposition.Add("medium", meshes[1]);
        superposition.Add("tall", meshes[2]);

    }

    void CollapseWave(int x, int y){

        terrainComponents[x][y].mesh = terrainComponents[x][y].subSuperposition.ElementAt(rand.Next(0, terrainComponents[x][y].subSuperposition.Count)).Value;

        int xnew = Mathf.Clamp(x+1,0, chunkSize-1);
        int ynew = Mathf.Clamp(y+1,0, chunkSize-1);

        if (terrainComponents[x][y].mesh == superposition["flat"]){

            if (terrainComponents[xnew][y].subSuperposition.ContainsKey("tall")){

                terrainComponents[xnew][y].subSuperposition.Remove("tall");
            }
            if (terrainComponents[x][ynew].subSuperposition.ContainsKey("tall")){

                terrainComponents[x][ynew].subSuperposition.Remove("tall");

            }
            

        }
        if (terrainComponents[x][y].mesh == superposition["tall"]){

            if (terrainComponents[x][ynew].subSuperposition.ContainsKey("flat")){

                terrainComponents[x][ynew].subSuperposition.Remove("flat");

            }
            if (terrainComponents[xnew][y].subSuperposition.ContainsKey("flat")){

                terrainComponents[xnew][y].subSuperposition.Remove("flat");
            }
        }

    }


}
