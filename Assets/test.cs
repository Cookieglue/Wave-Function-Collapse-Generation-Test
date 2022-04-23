using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string json = File.ReadAllText(Application.dataPath + "/SuperPositionRegistry.json");
        var data = JsonConvert.DeserializeObject<Dictionary<string,SuperPositionType>>(json);
        print(data["triangle"].faces[2][3]);
    }

}
