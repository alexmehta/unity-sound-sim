using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
public static class Filter{
    static int threshold = 5;
    public static void filter(ref List<List<Vector3>> flooded){
        for (int z = 0; z<flooded.Count;z++)
        {
	    HashSet<Vector3> bucket = new HashSet<Vector3>();
            for(int i = 0; i<flooded[z].Count;i++)
            {
                flooded[z][i] = Round(flooded[z][i]);
		if(bucket.Contains(flooded[z][i])||flooded[z][i]==new Vector3(0,0,0)){
		   flooded[z].RemoveAt(i);
		   i--;
		}
		else{
		    bucket.Add(flooded[z][i]);
		}
            }      
	    if(flooded[z].Count < threshold){
	        flooded.RemoveAt(z);
		z--;
	    }
        }     

    }
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 3)
    {
         float multiplier = 1;
         for (int i = 0; i < decimalPlaces; i++)
         {
             multiplier *= 10f;
         }
         return new Vector3(
             Mathf.Round(vector3.x * multiplier) / multiplier,
             Mathf.Round(vector3.y * multiplier) / multiplier,
             Mathf.Round(vector3.z * multiplier) / multiplier);
    } 
}
