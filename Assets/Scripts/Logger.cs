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

    private string movementFile, chooseFile, logTime;
    private Vector2 pos, vel, acc;
    private float time1, time0;

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per time step
	void FixedUpdate () 
    {
		
	}

    /// <summary>
    /// Starts the positions's file.
    /// </summary>
    /// <param name="name">Name.</param>
    public void StartFiles(string name)
    {
        pos = vel = acc = Vector2.zero;

        logTime = DateTime.Now.ToString("yy-MM-dd HH-mm");

        movementFile = Application.dataPath + "\\Logs\\" + name + "\\"  + name + " - " + logTime + " - Movements.txt";
        Directory.CreateDirectory(Application.dataPath + "\\Logs\\" + name);
        File.WriteAllText (movementFile, "Time\t" +
            "Pos X\tPos Y\t" +
            "Vel X\tVel Y\t" +
            "Acc X\tAcc Y\t" +
            Environment.NewLine);
        
        chooseFile = Application.dataPath + "\\Logs\\" + name + "\\"  + name + " - " + logTime + " - Choices.txt";
        Directory.CreateDirectory(Application.dataPath + "\\Logs\\" + name);
        File.WriteAllText (chooseFile, "Choice\t" +
            "N cards\t" +
            "Objective\t" +
            "Choice\t" +
            "Match\t" +
            "Time to Choose\t" +
            "Obj Mag\tObj Ang\t" +
            "Cho Mag\tCho Ang\t" +
            Environment.NewLine);
    }


    /// <summary>
    /// Registers based in the specified time and position.
    /// </summary>
    /// <param name="time">Current time.</param>
    /// <param name="position">Current position.</param>
    public void Register(float time, Vector2 position) 
    {
        Vector2 vel_axu = (position - pos) / (time - time1);
        acc = (vel_axu - vel) / (time - time0);
        vel = vel_axu;
        pos = position;

        time0 = time1;
        time1 = time;

        File.AppendAllText(movementFile, 
            time + "\t"
            + pos.x + "\t" 
            + pos.y + "\t"
            + vel.x + "\t"
            + vel.y + "\t"
            + acc.x + "\t"
            + acc.y
        );

        File.AppendAllText(movementFile, Environment.NewLine);
    }

    /// <summary>
    /// Register the specified choice, objective position and choice position.
    /// </summary>
    /// <param name="choice">Current choice.</param>
    /// <param name="angObjective">Objective position.</param>
    /// <param name="angChoice">Choice position.</param>
    public void Register(Choice choice, Vector2 objPos, Vector2 choicePos)
    {
        int match = 0;

        Vector2 obj, cho;

        obj = new Vector2(objPos.magnitude, Mathf.Atan2(objPos.y, objPos.x)) * Mathf.Rad2Deg;
        cho = new Vector2(choicePos.magnitude, Mathf.Atan2(choicePos.y, choicePos.x)) * Mathf.Rad2Deg;

        if (choice.match)
            match = 1;

        File.AppendAllText(chooseFile, 
            choice.order + "\t"
            + choice.numOptions + "\t" 
            + choice.objectiveCard + "\t"
            + choice.choiceCard + "\t"
            + match + "\t"
            + choice.timeToChoose + "\t"
            + obj.x  + "\t" + obj.y + "\t"
            + cho.x  + "\t" + cho.y
        );

        File.AppendAllText(chooseFile, Environment.NewLine);

    }
}
