using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManagement : MonoBehaviour
{
    
    public static List<TerrainComponents> InsertionSortCells(List<TerrainComponents> input){

        int n = input.Count;
        for (int i = 1; i < n; ++i) {

            TerrainComponents key = input[i];
            int keyCount = key.subSuperposition.Count;
            
            int j = i - 1;
            // Move elements of arr[0..i-1],
            // that are greater than key,
            // to one position ahead of
            // their current position
            while (j >= 0 && input.ElementAt(j).subSuperposition.Count > keyCount) {
            
                input[j + 1] = input[j];
                j --;

            }
            input[j+1] = key;
        }

        return input;

    }

    public static int getNetEntropy(List<TerrainComponents> input){
        
        int netEntropy = 0;

        for (int i = 0 ; i < input.Count ; i ++){

            netEntropy += input[i].subSuperposition.Count;  

        }

        return netEntropy;
        

    }

}
