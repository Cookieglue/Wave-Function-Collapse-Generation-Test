using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> first;
    public List<string> second;
    void Start()
    {
        second = first;
    }

}
