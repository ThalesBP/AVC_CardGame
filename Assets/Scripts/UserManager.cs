using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Class responsible to manage login.
/// </summary>
public class UserManager: GameBase {

    private Info info;
    private string accountsPath;
    private const string MasterPassword = "LabDinRehab";

    public List<string> Managers
    {
        get { return info.managers; }
    }
    public List<string> Players
    {
        get { return info.players; }
    }
    public List<string> Description
    {
        get { return info.description; }
    }
    public int AmountManagers
    {
        get { return info.managers.Count; }
    }
    public int AmountPlayers
    {
        get { return info.players.Count; }
    }

	// Use this for initialization
	void Awake () 
    {
        info = new Info();

        accountsPath = Application.dataPath + "\\Users\\";
        if (!Directory.Exists(accountsPath))
            Directory.CreateDirectory(accountsPath);
        
        accountsPath = accountsPath + "Accounts.dat";
        if (!File.Exists(accountsPath))
            File.Create(accountsPath);

        Load();
    }
    /*
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
    }*/

    /// <summary>
    /// Load current managers and passwords.
    /// </summary>
    void Load()
    {
        FileStream accounts = File.Open(accountsPath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        if (accounts.Length > 0)
            info = (Info)bf.Deserialize(accounts);
        accounts.Close();
    }

    /// <summary>
    /// Saves current managers and passwords.
    /// </summary>
    private void Save()
    {
        FileStream accounts = File.Open(accountsPath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(accounts, info);
        accounts.Close();
    }

    /// <summary>
    /// Checks the password.
    /// </summary>
    /// <returns><c>true</c>, if password was checked, <c>false</c> otherwise.</returns>
    /// <param name="UserNumber">User number.</param>
    /// <param name="password">Password.</param>
    public bool CheckPassword(int userNumber, string password)
    {
        return ((info.passwords[userNumber] == password) || (MasterPassword == password));
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
            info.managers.Add(name);
            info.passwords.Add(password);
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
                info.managers.RemoveAt(userNumber);
                info.passwords.RemoveAt(userNumber);
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
            info.managers[userNumber] = name;
            info.passwords[userNumber] = password;
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
            info.players.Add(name);
            if (information == "")
                info.description.Add("No description");
            else
                info.description.Add(information);
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
            info.players.RemoveAt(userNumber);
            info.description.RemoveAt(userNumber);
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
            info.players[userNumber] = name;
            if (information == "")
                info.description[userNumber] = "No description";
            else
                info.description[userNumber] = information;
            Save();
        }
    }}

[Serializable]
class Info
{
    public List<string> managers, passwords, players, description;

    public Info()
    {
        managers = new List<string>();
        passwords = new List<string>();
        players = new List<string>();
        description = new List<string>();    
    }
}