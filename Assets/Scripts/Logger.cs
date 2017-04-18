using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

/// <summary>
/// Registers everything in the game.
/// </summary>
public class Logger : MonoBehaviour {

    private string textFile = Application.dataPath + "\\Logs\\Session" + DateTime.Now.ToString("yy-MM-dd HH-mm") + ".txt";

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per time step
	void FixedUpdate () 
    {
		
	}
}
