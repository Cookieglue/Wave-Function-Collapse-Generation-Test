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
    [SerializeField] private int chunkSize = 20;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private System.Random rand = new System.Random();
    private int initialEntropy = 0;

    //Dictionaries, Arrays, and Lists
    private Dictionary<string, Mesh> meshes = new  Dictionary<string, Mesh>();
    private Dictionary<string, SuperPositionType> superposition = new  Dictionary<string, SuperPositionType>();
    private Dictionary<string, string> socketPairs = new Dictionary<string, string>();
    public List<TerrainComponents> terrainComponents;
    private GameObject[][] tempGameobjects;
    string superJson;

TerrainComponents comp;

    CombineInstance[] combine;

    void Start(){


        meshFilter = GetComponent<MeshFilter>();
        combine = new CombineInstance[chunkSize*chunkSize];
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        mesh = transform.GetComponent<MeshFilter>().mesh;

        JsonSetup();
        InitializeChunk();

        RandElement();
        CollapseWave();


        
    }
    void Update()
    {
        if(Input.GetMouseButton(0)){
        int netEntropy = 1;
        
            UpdateEntropy();
            terrainComponents = DataManagement.InsertionSortCells(terrainComponents);
            netEntropy = 0;
            CollapseWave();
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
                    tempGameobjects[x][y].GetComponent<TerrainComponents>()
                );
                int index = x*chunkSize + y;
                terrainComponents[index].position = new int[] {x,y};
                terrainComponents[index].subSuperposition = superposition = JsonConvert.DeserializeObject<Dictionary<string,SuperPositionType>>(superJson);
                terrainComponents[index].entropy = initialEntropy;
            }
            
        }

    }
    void CollapseWave(){

        try{
        int random = rand.Next(0, terrainComponents[0].subSuperposition.Count);
        //load mesh randomly from possible states
        SuperPositionType meshPath = 
        terrainComponents[0].subSuperposition.ElementAt(random).Value;
        

        //set mesh
        terrainComponents[0].mesh = meshes[meshPath.mesh];
        //get random possible faces, and find the key which will be the rotation
        int rot = meshPath.faces.ElementAt(rand.Next(0, meshPath.faces.Count)).Key;
        terrainComponents[0].rotation = rot;

        //logic and reducing entropy
        int[] position = terrainComponents[0].position;
        //Right
        if (position[0]+1 < chunkSize && tempGameobjects[position[0]+1][position[1]] != null){

            TerrainComponents right = tempGameobjects[position[0]+1][position[1]].GetComponent<TerrainComponents>();
            EliminateEntropy(right.subSuperposition, "West", meshPath.faces[rot]["East"]);
            right.entropy = DataManagement.recalculateEntropy( right.subSuperposition);
            
        }
        //Down
        if (position[1]-1 > 0 && tempGameobjects[position[0]][position[1]-1] != null){

            TerrainComponents down = tempGameobjects[position[0]][position[1]-1].GetComponent<TerrainComponents>();
            EliminateEntropy(down.subSuperposition, "North", meshPath.faces[rot]["South"]);
            down.entropy = DataManagement.recalculateEntropy( down.subSuperposition);
            
        }
        //Left
        if (position[0]-1 > 0 && tempGameobjects[position[0]-1][position[1]] != null){

            TerrainComponents left = tempGameobjects[position[0]-1][position[1]].GetComponent<TerrainComponents>();
            EliminateEntropy(left.subSuperposition, "East", meshPath.faces[rot]["West"]);
            left.entropy = DataManagement.recalculateEntropy( left.subSuperposition);
            
        }
        //Up
        if (position[1]+1 < chunkSize && tempGameobjects[position[0]][position[1]+1] != null){

            TerrainComponents up = tempGameobjects[position[0]][position[1]+1].GetComponent<TerrainComponents>();
            EliminateEntropy(up.subSuperposition, "South", meshPath.faces[rot]["North"]);
            up.entropy = DataManagement.recalculateEntropy( up.subSuperposition);
            
        }
        MergeMeshes(position);
        }
        catch{}
        terrainComponents.RemoveAt(0);
        

    }

    void RandElement(){

        TerrainComponents key = terrainComponents[0];
        int keyNum = rand.Next(0, terrainComponents.Count);
        terrainComponents[0] = terrainComponents[keyNum];
        terrainComponents[keyNum] = key;

    }
    void JsonSetup(){

        //load jsons
        superJson = File.ReadAllText(Application.dataPath + "/SuperPositionRegistry.json");
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

    void UpdateEntropy(){

        foreach (TerrainComponents comp in terrainComponents){
            comp.entropy = DataManagement.recalculateEntropy(comp.subSuperposition);
        }

    }
    void EliminateEntropy(Dictionary<string, SuperPositionType> input, string direction, string socket){
        
        foreach (KeyValuePair<string, SuperPositionType> superPositionType in input.ToList()){
            
            foreach(KeyValuePair<int, Dictionary<string, string>> rotation in superPositionType.Value.faces.ToList()){

                if (rotation.Value[direction] != socketPairs[socket]){

                   input[superPositionType.Key].faces.Remove(rotation.Key);

                }

            }
            if (superPositionType.Value.faces.Count == 0){

                input.Remove(superPositionType.Key);

            }

        }

    
    }

    public void MergeMeshes(int[] index){

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
