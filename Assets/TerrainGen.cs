using System.Xml.Linq;
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

    [SerializeField] private GameObject positionObject;
    [SerializeField] private int chunkSize = 15;
    private MeshFilter meshFilter;
    private System.Random rand = new System.Random();
    private int initialEntropy = 0;

    //Dictionaries, Arrays, and Lists
    private Dictionary<string, Mesh> meshes = new  Dictionary<string, Mesh>();
    private Dictionary<string, SuperPositionType> superposition = new  Dictionary<string, SuperPositionType>();
    private Dictionary<string, string> socketPairs = new Dictionary<string, string>();
    public List<TerrainComponents> terrainComponents;
    private GameObject[][] tempGameobjects;

TerrainComponents comp;

    CombineInstance[] combine;

    void Start(){


        meshFilter = GetComponent<MeshFilter>();
        combine = new CombineInstance[chunkSize*chunkSize];
        transform.GetComponent<MeshFilter>().mesh = new Mesh();

        JsonSetup();
        InitializeChunk();
/**
        int netEntropy = 1;
        while (netEntropy > 0){

            netEntropy = 0;
            CollapseWave();
            terrainComponents = DataManagement.InsertionSortCells(terrainComponents);
            netEntropy = DataManagement.getNetEntropy(terrainComponents);


        }

        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        **/
    }
    void Update()
    {
        if(Input.GetMouseButton(0)){
            
            int netEntropy = 0;
            CollapseWave();
            terrainComponents = DataManagement.InsertionSortCells(terrainComponents);
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            netEntropy = DataManagement.getNetEntropy(terrainComponents);
        }
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
                    tempGameobjects[x][y].GetComponent<TerrainComponents>()
                );
                terrainComponents[x*chunkSize + y].position = new int[] {x,y};
                terrainComponents[x*chunkSize + y].subSuperposition = new Dictionary<string, SuperPositionType>(superposition);
                terrainComponents[x*chunkSize + y].entropy = initialEntropy;
            }
            
        }

    }

    void CollapseWave(){

        //load mesh randomly from possible states
        SuperPositionType meshPath = 
        terrainComponents[0].subSuperposition.ElementAt(rand.Next(0, terrainComponents[0].subSuperposition.Count)).Value;
        
        //set mesh
        terrainComponents[0].mesh = meshes[meshPath.mesh];
        //get random possible faces, and find the key which will be the rotation
        int rot = meshPath.faces.ElementAt(rand.Next(0, meshPath.faces.Count)).Key;
        terrainComponents[0].rotation = rot;

        //logic and reducing entropy
        int[] position = terrainComponents.ElementAt(0).position;
        
        //Right
        if (position[0]+1 < chunkSize && tempGameobjects[position[0]+1][position[1]] != null){

            TerrainComponents right = tempGameobjects[position[0]+1][position[1]].GetComponent<TerrainComponents>();
            EliminateEntropy(right.subSuperposition, "West", meshPath.faces[rot]["East"]);
            
        }
        //Down
        if (position[1]+1 < chunkSize && tempGameobjects[position[0]][position[1]+1] != null){

            TerrainComponents down = tempGameobjects[position[0]][position[1]+1].GetComponent<TerrainComponents>();
            EliminateEntropy(down.subSuperposition, "North", meshPath.faces[rot]["South"]);
            
        }
        //Left
        if (position[0]-1 > 0 && tempGameobjects[position[0]-1][position[1]] != null){

            TerrainComponents right = tempGameobjects[position[0]-1][position[1]].GetComponent<TerrainComponents>();
            EliminateEntropy(right.subSuperposition, "East", meshPath.faces[rot]["West"]);
            
        }
        //Up
        if (position[1]-1 > 0 && tempGameobjects[position[0]][position[1]-1] != null){

            TerrainComponents down = tempGameobjects[position[0]][position[1]-1].GetComponent<TerrainComponents>();
            EliminateEntropy(down.subSuperposition, "South", meshPath.faces[rot]["North"]);
            
        }

        CombineMeshes(position);
        terrainComponents.RemoveAt(0);


    }

    void JsonSetup(){

        //load jsons
        string superJson = File.ReadAllText(Application.dataPath + "/SuperPositionRegistry.json");
        superposition = JsonConvert.DeserializeObject<Dictionary<string,SuperPositionType>>(superJson);

        string pairsJson = File.ReadAllText(Application.dataPath + "/Pairs.json");
        socketPairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(pairsJson);


        for(int i = 0 ; i < superposition.Count ; i ++){

            //get all models
            string meshName = superposition.ElementAt(i).Value.mesh;
            meshes.Add(meshName, Resources.Load<Mesh>("Models/" + meshName));

            //add 4 entropy for each of the 4 faces
            initialEntropy +=4;

        }

    }


    void EliminateEntropy(Dictionary<string, SuperPositionType> subsuperposition,string direction, string socket){

        Dictionary<string, SuperPositionType> temp = subsuperposition;

        for (int i = 0; i < subsuperposition.Count ; i ++){

            for (int j = 0 ; j < subsuperposition.ElementAt(i).Value.faces.Count ; j++){

                //destroy rotations that dont have the right socket
                Dictionary<string, string> faces = subsuperposition.ElementAt(i).Value.faces.ElementAt(j).Value;
                if (faces[direction] != socketPairs[socket]){

                    temp.ElementAt(i).Value.faces.Remove(j);

                }

            }
            //destroy meshtype if no possible orientations
            if (temp.ElementAt(i).Value.faces.Count == 0){

                subsuperposition.Remove(temp.ElementAt(i).Key);

            }

        }
        DataManagement.recalculateEntropy(subsuperposition);
        superposition = temp;
    }

    public void CombineMeshes(int[] index){

        //combine meshes to make compooter happi

        //convert index to 2d array
        int y =  index[1];
        int x = index[0];

        int order = x*chunkSize + y;

        tempGameobjects[x][y].transform.rotation = Quaternion.Euler(0, terrainComponents[0].rotation, 0);

        combine[order].mesh = terrainComponents[0].mesh;
        combine[order].transform = tempGameobjects[x][y].transform.localToWorldMatrix;

        
        Destroy(tempGameobjects[x][y]);

    }

}
