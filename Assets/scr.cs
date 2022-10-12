using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Linq;

public class scr : MonoBehaviour
{
    Mesh mesh;
    Vector3[] mesharr;

    public GameObject pointobj;
    RsPointCloudRenderer rp;
    public GameObject prefab;

    List<GameObject> lastGen;
    List<List<Vector3>> flooded;

    public int objectthreshold = 5;
    public int roundfactor = 20;

    public float timeDelay;
    float timeCounter = 1;


    public AudioSource clip;
    public int beepinterval = 4;
    int framecounter = 0;
    int currentobject = 0;
    int currentvector = 0;

    int w = 640;
    int h = 480;
    
    public int skip = 40;
    public float pitchsensitivity = 1;

    

    void Start()
    {

        rp = pointobj.GetComponent<RsPointCloudRenderer>();
        lastGen = new List<GameObject>();
        flooded = new List<List<Vector3>>();



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        framecounter++;
       

        bool flood = false;

        timeCounter -= Time.deltaTime;
        if (timeCounter < 0)
		{
            timeCounter = timeDelay;
            flood = true;


		}



        if(rp.mesh == null) return;
        if (mesh == null) mesh = rp.mesh;


        

		if (flood)
		{

            mesharr = mesh.vertices;

            Vector3[] l = new Vector3[w * h / (skip * skip)];

            int index = 0;
            Vector3 scaleVector = new Vector3(1, -1, 1);

            for (int i = 0; i < h; i += skip)
            {

                for (int j = 0; j < w; j += skip)
                {

                    Vector3 clone = mesharr[i * w + j];
                    clone.Scale(scaleVector);
                    l[index] = clone;
                    index++;

                }


            }

            List<List<Vector3>> newflooded = floodfill(l);

            newflooded = filter(newflooded);

            flooded = newflooded;

            currentobject = 0;
            currentvector = 0;

        }


        if (framecounter % beepinterval == 0 && flooded.Count > currentobject)
		{

            if(currentvector >= flooded[currentobject].Count)
			{
                flooded.RemoveAt(0);//currentobject++;
                currentvector = 0;
			}
			if(flooded.Count > currentobject && flooded[currentobject].Count > 0)
			{
                AudioSource cloneclip = Instantiate(clip);
                cloneclip.transform.position = flooded[currentobject][currentvector];
                
                flooded[currentobject].RemoveAt(0);//currentvector++;
                
                float distance = Vector3.Distance( cloneclip.transform.position, Camera.main.transform.position );
                cloneclip.pitch = pitchfunction(distance);
            }


        }

        drawmeshes();
         
        

        //print("FPS: " + (1 / Time.deltaTime));



    }

    public float pitchfunction(float distance)
	{

        return (1 / (distance*pitchsensitivity + 1));

	}


    public void drawmeshes()
	{
        foreach (GameObject old in lastGen)
        {
            Destroy(old);
        }


        int ind = 0;

        Color[] colors = { Color.red, Color.cyan, Color.green, Color.magenta, Color.yellow, Color.grey };

        foreach (var part in flooded)
        {

            GameObject shell = Instantiate(prefab);

            shell.transform.position = Vector3.zero;

            Vector3[] points = part.ToArray();

            Mesh genmesh = new Mesh();


            genmesh.vertices = points;

            genmesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10, 10, 10));


            int[] identity = new int[points.Length];

            for (var a = 0; a < identity.Length; a++)
            {
                identity[a] = a;
            }

            genmesh.SetIndices(identity, MeshTopology.Points, 0);

            shell.GetComponent<MeshFilter>().mesh = genmesh;


            shell.GetComponent<MeshRenderer>().material.color = colors[ind % colors.Length];

            lastGen.Add(shell);


            ind++;


        }
    }


    List<List<Vector3>> floodfill(Vector3[] array)
	{

        int[] passed = new int[array.Length];

        List<List<Vector3>> flooded = new List<List<Vector3>>();

        float threshold = 0.15f;

        bool breakout = false;



        while (!breakout)
		{


            List<Vector3> current = new List<Vector3>();
            Stack<Vector3> nextflood = new Stack<Vector3>();

            breakout = true;

            for(var a = 0; a < passed.Length; a++)
			{
                if(passed[a] == 0)
				{
                    nextflood.Push(array[a]);
                    passed[a] = 1;
                    break;
				}
			}


            while(nextflood.Count > 0)
			{

                Vector3 start = nextflood.Pop();

                current.Add(start);


                for (var a = 0; a < array.Length; a++)
				{

                    if(passed[a] == 0)
					{

                        breakout = false;


                        if( Vector3.SqrMagnitude(start - array[a]) < threshold )
						{
                            nextflood.Push(array[a]);
                            passed[a] = 1;
						}


					}


				}


            }

            flooded.Add(current);


		}

        return flooded;

	}

    public List<List<Vector3>> filter(List<List<Vector3>> flooded)
	{

        flooded.Sort(depthsort);

        return flooded.ConvertAll(list =>
        {

            list.Sort(distancesort);

            return list.ConvertAll(round)
            .Distinct()
            .Where(v => v != Vector3.zero).ToList();

            
        }).Where(f => f.Count >= objectthreshold).ToList();

    }
    public Vector3 round(Vector3 vector3)
    {
        distancesort(vector3, vector3);

        vector3.x = Mathf.Round(vector3.x * roundfactor) / roundfactor;
        vector3.y = Mathf.Round(vector3.y * roundfactor) / roundfactor;
        vector3.z = Mathf.Round(vector3.z * roundfactor) / roundfactor;

        return vector3;

    }

    public int distancesort(Vector3 v1, Vector3 v2)
	{
        Vector2 s1 = Camera.main.WorldToScreenPoint(v1);
        Vector2 s2 = Camera.main.WorldToScreenPoint(v2);

        float d1 = Mathf.Floor(s1.x) * Screen.height + s1.y;
        float d2 = Mathf.Floor(s2.x) * Screen.height + s2.y;

        print(s1);



        return d1.CompareTo(d2);

	}

    public int depthsort(List<Vector3> l1, List<Vector3> l2)
	{

        float s1 = 0;
        float s2 = 0;

        l1.ForEach(v => s1 += v.z);
        l2.ForEach(v => s2 += v.z);

        s1 /= l1.Count;
        s2 /= l2.Count;

        return s1.CompareTo(s2);

	}
}
