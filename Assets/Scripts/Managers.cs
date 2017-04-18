using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible to manage login.
/// </summary>
public class Managers : GameBase {

    private List<string> users, passwords;
    private string accountsPath = "\\Users\\Managers\\";
    private string accountsFile = "Accounts.data";

    public List<string> Users
    {
        get { return users; }
    }
    public int Amount
    {
        get { return users.Count; }
    }

	// Use this for initialization
	void Awake () 
    {
        users = new List<string>();
        passwords = new List<string>();

        string[,] accounts;

        accountsPath = Application.dataPath + accountsPath;
        if (!Directory.Exists(accountsPath))
        {
            Directory.CreateDirectory(accountsPath);
        }

        accountsFile = accountsPath + accountsFile;
        if (!File.Exists(accountsFile))
        {
            File.Create(accountsFile);
        }

        accounts = Read(accountsFile);

        for (int i = 0; i < accounts.GetLength(0); i++)
        {
            users.Add(accounts[i, 0]);
            passwords.Add(accounts[i, 1]);
        }

        Debug.Log(Amount);
    }

    /// <summary>
    /// Reads the specified file.
    /// </summary>
    /// <param name="file">File path.</param>
    private string[,] Read(string file)
    {
        string[,] result;
        string[] lines, line;
        int lenght = 2;

        line = new string[lenght];

        if (!File.Exists(file))
        {
            return null;
        }

        lines = File.ReadAllLines(file);
        result = new string[lines.Length, lenght];

        for (int i = 0; i < lines.Length; i++)
        {
            line = lines[i].Split('\x09');
            for (int j = 0; j < lenght; j++)
                result[i, j] = line[j];
        }
        return result;
    }

    /// <summary>
    /// Saves current users and passwords.
    /// </summary>
    private void Save()
    {
        if (!File.Exists(accountsPath + "Temp.data"))
            File.Create(accountsPath + "Temp.data");

        for (int i = 0; i < Amount; i++)
        {
            File.AppendAllText(accountsPath + "Temp.data", users[i] + "\t" + passwords[i] + Environment.NewLine);
        }
        File.Replace(accountsPath + "Temp.data", accountsFile, accountsPath + "Backup.data");
        File.Delete(accountsPath + "Temp.data");
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
    /// Adds the specified name and password.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="password">Password.</param>
    public void Add(string name, string password)
    {
        users.Add(name);
        passwords.Add(password);
        Save();
    }

    /// <summary>
    /// Trys to remove the specified user, checks the password, return true if is correct.
    /// </summary>
    /// <param name="userNumber">User number.</param>
    /// <param name="password">Password.</param>
    public bool Remove(int userNumber, string password)
    {
        Debug.Log(Amount);

        if (userNumber >= Amount)
        {
            Debug.Log("Maior");
            return false;
        }

        if (CheckPassword(userNumber, password))
        {
            users.RemoveAt(userNumber);
            passwords.RemoveAt(userNumber);
            Save();
            return true;
        }
        else
        {
            return false;
            Debug.Log("Senha errada");
        }
    }
}
