using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible to manage login.
/// </summary>
public class UserManager: GameBase {

    public List<string> managers, passwords, players, description;
    private string accountsPath = "\\Users\\";
    private string managersFile = "Managers";
    private string playersFile = "Players";
    private string dotData = ".data";

    public List<string> Managers
    {
        get { return managers; }
    }
    public List<string> Players
    {
        get { return players; }
    }
    public List<string> Description
    {
        get { return description; }
    }
    public int AmountManagers
    {
        get { return managers.Count; }
    }
    public int AmountPlayers
    {
        get { return players.Count; }
    }

	// Use this for initialization
	void Awake () 
    {
        managers = new List<string>();
        passwords = new List<string>();
        players = new List<string>();
        description = new List<string>();

        accountsPath = Application.dataPath + accountsPath;
        if (!Directory.Exists(accountsPath))
        {
            Directory.CreateDirectory(accountsPath);
        }

        managersFile = accountsPath + managersFile + dotData;
        if (!File.Exists(managersFile))
        {
            File.Create(managersFile);
        }

        playersFile = accountsPath + playersFile + dotData;
        if (!File.Exists(playersFile))
        {
            File.Create(playersFile);
        }

        string[,] accounts;
        accounts = Read(managersFile);
        for (int i = 0; i < accounts.GetLength(0); i++)
        {
            managers.Add(accounts[i, 0]);
            passwords.Add(accounts[i, 1]);
        }

        accounts = Read(playersFile);
        for (int i = 0; i < accounts.GetLength(0); i++)
        {
            players.Add(accounts[i, 0]);
            description.Add(accounts[i, 1]);
        }
    }

    /// <summary>
    /// Reads the specified file.
    /// </summary>
    /// <param name="file">File path.</param>
    private string[,] Read(string file)
    {
        string[,] result;
        string[] lines, line;
        int length = 2;

        line = new string[length];

        if (!File.Exists(file))
        {
            return null;
        }

        lines = File.ReadAllLines(file);
        result = new string[lines.Length, length];

        for (int iUser = 0; iUser < lines.Length; iUser++)
        {
            line = lines[iUser].Split('\x09');


            for (int iInfo = 0; iInfo < line.Length; iInfo++)
            {
                result[iUser, iInfo] = line[iInfo];
            }
        }
        return result;
    }

    /// <summary>
    /// Saves current managers and passwords.
    /// </summary>
    private void Save()
    {
        if (!File.Exists(accountsPath + "Temp" + dotData))
            File.Create(accountsPath + "Temp" + dotData);

        for (int i = 0; i < AmountManagers; i++)
        {
            File.AppendAllText(accountsPath + "Temp.data", managers[i] + "\t" + passwords[i] + Environment.NewLine);
        }
        File.Replace(accountsPath + "Temp" + dotData, managersFile, accountsPath + "Backup_managers" + dotData);
        File.Delete(accountsPath + "Temp" + dotData);

        File.Create(accountsPath + "Temp" + dotData);

        for (int i = 0; i < AmountPlayers; i++)
        {
            File.AppendAllText(accountsPath + "Temp.data", players[i] + "\t" + description[i] + Environment.NewLine);
        }
        File.Replace(accountsPath + "Temp" + dotData, playersFile, accountsPath + "Backup_players" + dotData);
        File.Delete(accountsPath + "Temp" + dotData);
    }

    /// <summary>
    /// Checks the password.
    /// </summary>
    /// <returns><c>true</c>, if password was checked, <c>false</c> otherwise.</returns>
    /// <param name="UserNumber">User number.</param>
    /// <param name="password">Password.</param>
    public bool CheckPassword(int userNumber, string password)
    {
        return (passwords[userNumber] == password);
    }

    /// <summary>
    /// Adds a new manager.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="password">Password.</param>
    public void AddManager(string name, string password)
    {
        if ((name != "") && (password != ""))
        {
            managers.Add(name);
            passwords.Add(password);
            Save();
        }
    }

    /// <summary>
    /// Removes a manager.
    /// </summary>
    /// <param name="userNumber">User number.</param>
    /// <param name="password">Password.</param>
    public void RemoveManager(int userNumber, string password)
    {
        if (userNumber < AmountManagers)
            if (CheckPassword(userNumber, password))
            {
                managers.RemoveAt(userNumber);
                passwords.RemoveAt(userNumber);
                Save();
            }
    }

    /// <summary>
    /// Changes a manager.
    /// </summary>
    /// <param name="userNumber">User number.</param>
    /// <param name="name">Name.</param>
    /// <param name="password">Password.</param>
    public void ChangeManager(int userNumber, string name, string password)
    {
        if ((name != "") && (password != "") && (userNumber < AmountManagers))
        {
            managers[userNumber] = name;
            passwords[userNumber] = password;
            Save();
        }
    }

    /// <summary>
    /// Adds a player.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="information">Information.</param>
    public void AddPlayer(string name, string information)
    {
        if (name != "")
        {
            players.Add(name);
            if (information == "")
                description.Add("No description");
            else
                description.Add(information);
            Save();
        }
    }
    /// <summary>
    /// Removes a player.
    /// </summary>
    /// <param name="userNumber">User number.</param>
    /// <param name="password">Password.</param>
    public void RemovePlayer(int userNumber)
    {
        if (userNumber < AmountPlayers)
        {
            players.RemoveAt(userNumber);
            description.RemoveAt(userNumber);
            Save();
        }
    }

    /// <summary>
    /// Changes a player.
    /// </summary>
    /// <param name="userNumber">User number.</param>
    /// <param name="name">Name.</param>
    /// <param name="password">Password.</param>
    public void ChangePlayer(int userNumber, string name, string information)
    {
        if ((name != "") && (userNumber < AmountManagers))
        {
            players[userNumber] = name;
            if (information == "")
                description[userNumber] = "No description";
            else
                description[userNumber] = information;
            Save();
        }
    }}
