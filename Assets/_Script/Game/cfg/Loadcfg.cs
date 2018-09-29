using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadcfg : MonoBehaviour
{
	IEnumerator Start ()
    {
        Configuration.LoadConfig();
        while (!Configuration.IsDone)
            yield return null;
        print(Configuration.GetString("Job", "Job1"));
		
	}
}
