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

    public int amount;
    public List<string> User
    {
        get { return users; }
    }

	// Use this for initialization
	void Start () 
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
        amount = accounts.GetLength(0);

        for (int i = 0; i < amount; i++)
        {
            users.Add(accounts[i, 0]);
            passwords.Add(accounts[i, 1]);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
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
}
