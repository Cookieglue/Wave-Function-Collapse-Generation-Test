using System.Reflection;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime;
using System.IO;

public class TerrainGen : MonoBehaviour
{
    private Dictionary<string, Mesh> meshes = new  Dictionary<string, Mesh>();
    public int chunkSize = 15;
    private Dictionary<string, SuperPositionType> superposition = new  Dictionary<string, SuperPositionType>();

    public List<TerrainComponents> terrainComponents;
    private MeshFilter meshFilter;

    private System.Random rand = new System.Random();
    [SerializeField] private GameObject positionObject;

    private GameObject[][] tempGameobjects;


    CombineInstance[] combine;

    void Start(){


        meshFilter = GetComponent<MeshFilter>();
        combine = new CombineInstance[chunkSize*chunkSize];
        transform.GetComponent<MeshFilter>().mesh = new Mesh();

        //load jsons
        string superJson = File.ReadAllText(Application.dataPath + "/SuperPositionRegistry.json");
        superposition = JsonConvert.DeserializeObject<Dictionary<string,SuperPositionType>>(superJson);

        for(int i = 0 ; i < superposition.Count ; i ++){

            string meshName = superposition.ElementAt(i).Value.mesh;
            meshes.Add(meshName, Resources.Load<Mesh>("Models/" + meshName));

        }

        InitializeChunk();

        int netEntropy = 1;
        while (netEntropy > 0){

            netEntropy = 0;
            CollapseWave(0);
            terrainComponents = DataManagement.InsertionSortCells(terrainComponents);
            netEntropy = DataManagement.getNetEntropy(terrainComponents);


        }

        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

    }

    void InitializeChunk(){

        tempGameobjects = new GameObject[chunkSize][];

        //set list
        for (int x = 0 ; x < chunkSize ;  x++){

            tempGameobjects[x] = new GameObject[chunkSize];
            for (int y = 0 ; y < chunkSize;  y++){

                //gameobject is only for positions (idk any other way)
                tempGameobjects[x][y] = Instantiate(positionObject, new Vector3(2* x, 0 ,2 *y), Quaternion.identity);

                //set terrainComponents and allow all possibilities
                terrainComponents.Add(
                    new TerrainComponents(null, new Dictionary<string, SuperPositionType>(superposition), 0, x + y*chunkSize)
                    //subSuperposition = new Dictionary<string, SuperPositionType>(superposition)
                );
                
            }
            
        }

    }

    void CollapseWave(int index){

        //load mesh randomly from possible states
        string meshPath = 
        terrainComponents[index].subSuperposition.ElementAt(rand.Next(0, terrainComponents[index].subSuperposition.Count)).Value.mesh;
        
        terrainComponents[index].mesh = meshes[meshPath];

        CombineMeshes(terrainComponents[0].order);
        terrainComponents.RemoveAt(index);

    }

    public void CombineMeshes(int index){

        //combine meshes to make compooter happi

        //convert index to 2d array
        int y =  index%chunkSize;
        int x = (index-y)/chunkSize;
        print(x);
        print(y);

        combine[index].mesh = terrainComponents[0].mesh;
        combine[index].transform = tempGameobjects[x][y].transform.localToWorldMatrix;

        Destroy(tempGameobjects[x][y]);

    }

}
