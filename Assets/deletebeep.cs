using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deletebeep : MonoBehaviour
{
    // Start is called before the first frame update

    float time = 0;
    float endtime = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > endtime) Destroy(gameObject);


    }
}