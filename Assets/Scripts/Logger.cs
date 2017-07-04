﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

/// <summary>
/// Registers everything in the game.
/// </summary>
public class Logger : MonoBehaviour {

    private string logTime, movementFile, chooseFile, messageFile, previousMessage, historicFile;
    private Vector2 pos, vel, acc;
    private float time1, time0;
    private int sideMember;

	// Use this for initialization
	void Start () 
    {
        previousMessage = "";
	}

    /// <summary>
    /// Starts the files.
    /// </summary>
    /// <param name="manager">Manager.</param>
    /// <param name="player">Player.</param>
    /// <param name="member">Member.</param>
    /// <param name="side">Side - 0: Left / 1: Right.</param>
    public void StartFiles(string manager, string player, string member, int side)
    {
        pos = vel = acc = Vector2.zero;

        if (side == 0)
            sideMember = -1;
        else
            sideMember = 1;

        logTime = DateTime.Now.ToString("yy-MM-dd HH-mm-ss");
        Directory.CreateDirectory(Application.dataPath + "\\Logs\\" + manager);

        movementFile = Application.dataPath + "\\Logs\\" + manager + "\\"  + player + " - " + member + " - " + logTime + " - Movements.txt";
        File.WriteAllText (movementFile, 
            "Time\t" +
            "Pos X\tPos Y\t" +
            "Vel X\tVel Y\t" +
            "Acc X\tAcc Y" +
            Environment.NewLine);
        
        chooseFile = Application.dataPath + "\\Logs\\" + manager + "\\"  + player + " - " + member + " - " + logTime + " - Choices.txt";
        File.WriteAllText (chooseFile, 
            "Choice\t" +
            "Time\t" +
            "Game Mode\t" +
            "N cards\t" +
            "Objective\t" +
            "Choice\t" +
            "Match\t" +
            "Value Points\t" +
            "Suit Points\t" + 
            "Color Points\t" + 
            "Total Points\t" +
            "Precision\t" +
            "Time to Play\t" +
            "Time to Memorize\t" +
            "Time to Choose\t" +
            "Game Speed\t" +
            "Obj Mag\tObj Ang\t" +
            "Cho Mag\tCho Ang" +
            Environment.NewLine);

        messageFile = Application.dataPath + "\\Logs\\" + manager + "\\"  + player + " - " + member + " - " + logTime + " - GameMessage.txt";
        File.WriteAllText (messageFile, 
            "Time\t" +
            "Message" +
            Environment.NewLine);



        historicFile = Application.dataPath + "\\Logs\\" + manager + "\\"  + player + " - Historic.txt";
        if(!File.Exists(historicFile))
            File.WriteAllText (historicFile,
                manager + "\t" + player + "\t" +
                "Game Results\t\t\t\t\t" +
                "Time to Play\t\t\t" +
                "Time to Memorize\t\t\t" +
                "Time to Choose\t\t\t" +
                "Card Matches\t\t\t" +
                "Movements Amplitude\t\t\t\t" +
                "Movements Plan\t\t\t\t" +
                "Movements Done\t\t\t\t" +
                Environment.NewLine +
                "Date\t" +
                "Member\t" +
                "Score\t" + 
                "Win Rate\t" +
                "Matches\t" +
                "Turns\t" +
                "Game Time\t" +
                "Average Time\t" +
                "Fastest Time\t" +
                "Slowest Time\t" +
                "Average Time\t" +
                "Fastest Time\t" +
                "Slowest Time\t" +
                "Average Time\t" +
                "Fastest Time\t" +
                "Slowest Time\t" +
                "Suit Matches\t" +
                "Value Matches\t" +
                "Color Mathes\t" +
                "Plantar flexion\t" +
                "Dorsiflexion\t" +
                "Inversion\t" +
                "Eversion\t" +
                "Plantar flexion\t" +
                "Dorsiflexion\t" +
                "Inversion\t" +
                "Eversion\t" +
                "Plantar flexion\t" +
                "Dorsiflexion\t" +
                "Inversion\t" +
                "Eversion\t" +
                Environment.NewLine);
    }

    /// <summary>
    /// Register the specified time and message.
    /// </summary>
    /// <param name="time">Time.</param>
    /// <param name="message">Message.</param>
    public void Register(float time, string message)
    {
        if (!String.Equals(message, previousMessage))
        {
            File.AppendAllText(messageFile,
                time + "\t"
                + message +
                Environment.NewLine);
            previousMessage = message;
        }
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
    public void Register(float time, string gameMode, Choice choice, Vector2 objPos, Vector2 choicePos)
    {
        int match = 0;

        Vector2 obj, cho;

        obj = new Vector2(objPos.magnitude, GameBase.Atan2(objPos.y, objPos.x)) * Mathf.Rad2Deg;
        cho = new Vector2(choicePos.magnitude, GameBase.Atan2(choicePos.y, choicePos.x)) * Mathf.Rad2Deg;

        if (choice.match)
            match = 1;

        File.AppendAllText(chooseFile, 
            choice.order + "\t"
            + time + "\t"
            + gameMode + "\t"
            + choice.numOptions + "\t" 
            + choice.objectiveCard + "\t"
            + choice.choiceCard + "\t"
            + match + "\t"
            + choice.valueMatch + "\t"
            + choice.suitMatch + "\t"
            + choice.colorMatch + "\t"
            + choice.pointMatch + "\t"
            + Choice.precision + "\t"
            + choice.TimeToPlay + "\t"
            + choice.TimeToMemorize + "\t"
            + choice.TimeToChoose + "\t"
            + Time.timeScale + "\t"
            + obj.x  + "\t" + obj.y + "\t"
            + cho.x  + "\t" + cho.y
        );

        File.AppendAllText(chooseFile, Environment.NewLine);

    }

    public void Register(string member, float gameTime, ChallengeManager history)
    {
        if (Choice.orderCounter == 0)
        {
            File.Delete(movementFile);
            File.Delete(chooseFile);
            File.Delete(messageFile);
        }
        else
            File.AppendAllText(historicFile, 
                logTime + "\t" +
                member + "\t" +
                Choice.totalPoints + "\t" +
                (100f * Choice.totalMatches / Choice.orderCounter) + "\t" +
                Choice.totalMatches + "\t" +
                Choice.orderCounter + "\t" +
                gameTime + "\t" +
                Choice.AverageTimeToPlay + "\t" +
                Choice.RangeTimeToPlay[0] + "\t" +
                Choice.RangeTimeToPlay[1] + "\t" +
                Choice.AverageTimeToMemorize + "\t" +
                Choice.RangeTimeToMemorize[0] + "\t" +
                Choice.RangeTimeToMemorize[1] + "\t" +
                Choice.AverageTimeToChoose + "\t" +
                Choice.RangeTimeToChoose[0] + "\t" +
                Choice.RangeTimeToChoose[1] + "\t" +
                (100f * Choice.suitCounter / Choice.orderCounter) + "\t" +
                (100f * Choice.valueCounter / Choice.orderCounter) + "\t" +
                (100f * Choice.colorCounter / Choice.orderCounter) + "\t" +
                ControlManager.Instance.ankle.Max.y + "\t" +
                ControlManager.Instance.ankle.Min.y + "\t" +
                ControlManager.Instance.ankle.Min.x * sideMember + "\t" +
                ControlManager.Instance.ankle.Max.x * sideMember + "\t" +
                (100f * history.Plan[1]) + "\t" +
                (100f * history.Plan[3]) + "\t" +
                (100f * history.Plan[1 + sideMember])+ "\t" +
                (100f * history.Plan[1 - sideMember]) + "\t" +
                (100f * history.Done[1]) + "\t" +
                (100f * history.Done[3]) + "\t" +
                (100f * history.Done[1 + sideMember]) + "\t" +
                (100f * history.Done[1 - sideMember]) + "\t" +
                Environment.NewLine);
    }
}
