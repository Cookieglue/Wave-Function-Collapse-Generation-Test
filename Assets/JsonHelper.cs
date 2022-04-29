using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JsonHelper : MonoBehaviour
{
    [SerializeField] private string key = "Slope";
    void Start()
    {
        string superJson = File.ReadAllText(Application.dataPath + "/SuperPositionRegistry.json");
        Dictionary<string, SuperPositionType> superposition = JsonConvert.DeserializeObject<Dictionary<string,SuperPositionType>>(superJson);

        Dictionary<string, string> faces = superposition[key].faces.Values.ElementAt(0);


        print(
            "\"North\" : " + "\"" + faces.Values.ElementAt(1)+ "\"," + "\n"+
            "\"East\" : " + "\"" + faces.Values.ElementAt(2)+ "\"," + "\n" +
            "\"South\" : " + "\"" + faces.Values.ElementAt(3)+ "\"," +"\n" +
            "\"West\" : " + "\"" + faces.Values.ElementAt(0)+ "\"" +"\n" +
            "\n" +
            "\"North\" : " +"\"" + faces.Values.ElementAt(2) + "\","+ "\n"+
            "\"East\" : " + "\"" + faces.Values.ElementAt(3)+ "\"," +"\n" +
            "\"South\" : " +"\"" + faces.Values.ElementAt(0) + "\","+ "\n" +
            "\"West\" : " + "\"" + faces.Values.ElementAt(1) + "\","+"\n" +
            "\n" +
            "\"North\" : " + "\"" + faces.Values.ElementAt(3)+ "\"," +"\n"+
            "\"East\" : " + "\"" + faces.Values.ElementAt(0)+ "\"," +"\n" +
            "\"South\" : " +"\"" + faces.Values.ElementAt(1)+ "\"," + "\n" +
            "\"West\" : " +"\"" + faces.Values.ElementAt(2)+ "\"," + "\n"
        
        );

    }

}
