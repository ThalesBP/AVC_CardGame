using UnityEngine;
//using UnityEngine.UI;

using System;
using System.IO;
using System.Text;

using System.Threading;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Linq;


public class Connection : MonoBehaviour {

	enum ConnectStatus {waiting, connecting, connected, disconnected};
    enum RobotIndex {centerspring, freespace, stiff, damp};
    enum GameIndex {position, velocity, acc, force};

	// Indice relativo da variavel
/*	public const int CENTERSPRING = 0; 	// Game
	public const int FREESPACE = 1; 	// Game
	public const int STIFF = 2;			// Game
	public const int DAMP = 3;			// Game

	public const int POSITION = 0; 		// Robo
	public const int VELOCITY = 1; 		// Robo
	public const int ACC = 2;			// Robo
	public const int FORCE = 3;			// Robo
*/
	public const int N_VAR = 4; 		// Numero de variaveis envolvidas
	private const int BIT_SIZE = 4; 	// Numero de bit da mascara; Deve ser multiplo de 2
	private const int INFO_SIZE = 4;	// 4 Float; 8 Double
	
	private int n_dof = 2;              // Degrees of freedon
//	private int mask_size; 
//	private byte[] activeMask;
	private byte[][][] gameStade;
    private byte[] gameStatus;
    private float[][] robotStade;
	private short robotStatus;

	private float delayCount;

	private Thread connectingThread;
	//private volatile bool stopThread;

	//public bool connected;
	ConnectStatus connectStatus;

    public int RobotStatus
    {
        get
        {
            return (int)robotStatus;
        }
    }

    public Vector2 Position 
    {
        get 
        {
            return new Vector2(robotStade[(int)GameIndex.position][0], robotStade[(int)GameIndex.position][1]);
        }
    }

    public Vector2 Velocity
    {
        get 
        {
            return new Vector2(robotStade[(int)GameIndex.velocity][0], robotStade[(int)GameIndex.velocity][1]);
        }
    }

    public Vector2 Acceleration 
    {
        get 
        {
            return new Vector2(robotStade[(int)GameIndex.acc][0], robotStade[(int)GameIndex.acc][1]);
        }
    }

    public Vector2 Force 
    {
        get 
        {
            return new Vector2(robotStade[(int)GameIndex.force][0], robotStade[(int)GameIndex.force][1]);
        }
    }

    public Vector2 CenterSpring
    {
        get{ 
            return CenterSpring; 
        }
        set 
        {
            SetStatus(0, CenterSpring.x, (int)RobotIndex.centerspring);
            SetStatus(1, CenterSpring.y, (int)RobotIndex.centerspring);
        }
    }

    public Vector2 FreeSpace
    {
        get{ 
            return FreeSpace; 
        }
        set 
        {
            SetStatus(0, FreeSpace.x, (int)RobotIndex.freespace);
            SetStatus(1, FreeSpace.y, (int)RobotIndex.freespace);
        }
    }

    public Vector2 Stiffness
    {
        get{ 
            return Stiffness; 
        }
        set 
        {
            SetStatus(0, Stiffness.x, (int)RobotIndex.stiff);
            SetStatus(1, Stiffness.y, (int)RobotIndex.stiff);
        }
    }

    public Vector2 Damping
    {
        get{ 
            return Damping; 
        }
        set 
        {
            SetStatus(0, Damping.x, (int)RobotIndex.damp);
            SetStatus(1, Damping.y, (int)RobotIndex.damp);
        }
    }

    public bool connected
    {
        get { return clientHere.IsConnected(); }
    }

//================================
	private NetworkClientTCP clientHere = new NetworkClientTCP();
//================================

//	private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\RehabLab\RehabSystem\Rehab System\Rehab System IMT\DoublePingPong\Logs\LogFilePos.txt";

	void Start()
	{
//		stopThread = false;
		connectingThread = new Thread (Connect);
		connectingThread.Start ();
		if (clientHere.IsConnected ())
			connectStatus = ConnectStatus.connected;
		else
			connectStatus = ConnectStatus.waiting;
	}


	void FixedUpdate()
	{
		switch (connectStatus)
		{
			case ConnectStatus.connected:
				SendMsg ();
				ReadMsg ();
				break;
		}
	}

    /*public const int N_VAR = 4;         // Numero de variaveis envolvidas
    private const int BIT_SIZE = 4;     // Numero de bit da mascara; Deve ser multiplo de 2
    private const int INFO_SIZE = 4;    // 4 Float; 8 Double
*/

	void Connect()
	{
		bool inLoop = true;
		while (inLoop)
		{
			switch (connectStatus)
			{
				case ConnectStatus.waiting:
					Debug.Log ("Starting connection");
					clientHere.Connect ("192.168.0.66", 8000, 0, BIT_SIZE + N_VAR * INFO_SIZE); // Here 192.168.0.67
					//	clientHere.SendString ("Connected!"); 

					InitializeVariables ();
					connectStatus = ConnectStatus.connecting;
					break;
				case ConnectStatus.connecting:
					Debug.Log ("Trying to connect");
					if (clientHere.IsConnected ())
						connectStatus = ConnectStatus.connected;
					break;
				default:
					inLoop = false;
					break;
			}
		}
	}

	public void SetStatus(int robot, float mag, int variable)
	{
//		activeMask[(BIT_SIZE * robot) / 8] |= (byte)(0x1 << (variable + robot*BIT_SIZE));
		gameStade [robot] [variable] = BitConverter.GetBytes (mag);
		return;
	}

	public void SetStatus(short status)
	{
		gameStatus = BitConverter.GetBytes((short)(status + 1));
	}

	public float ReadStatus(int robot, int variable)
	{
		return robotStade [robot] [variable];
	}

	public short ReadStatus()
	{
		return robotStatus;
	}

	private void SendMsg ()
	{
		byte[] msg = new byte[sizeof(short) + (N_VAR * INFO_SIZE) * n_dof];
		System.Buffer.BlockCopy (gameStatus, 0, msg, 0, sizeof(short));
//		System.Buffer.BlockCopy (activeMask, 0, msg, 0, activeMask.Length);
		for (int i = 0; i < n_dof; i++) 
		{
			for (int j = 0; j < N_VAR; j++)
			{
				System.Buffer.BlockCopy (gameStade [i][j], 0, msg, sizeof(short) + INFO_SIZE * (j + N_VAR * i), gameStade [i][j].Length);
			}
		}
		clientHere.SendByte (msg);
		return;
	}

	private void ReadMsg()
	{
		byte[] buffer = clientHere.ReceiveByte ();

		// Check if message is different than zero
		bool check = false;
		foreach(byte element in buffer)
		{
			if (element != 0x0)
			{
				check = true;
				break;
			}
		}

		if (check)
		{
			robotStatus = BitConverter.ToInt16 (buffer, 0);
			for (int i = 0; i < n_dof; i++)
			{
				for (int j = 0; j < N_VAR; j++)
				{
					robotStade[i][j] = BitConverter.ToSingle (buffer, sizeof(short) + INFO_SIZE*(j + N_VAR*i));
				}
				//				Debug.Log ("Robot " + (i+1) + "- Pos: " + robotStade[i][0].ToString() + ", Vel:" + robotStade[i][1].ToString() + ", Acc:" + robotStade[i][2].ToString() + ", For:" + robotStade[i][3].ToString());
			}
/*			for (int j = 0; j < N_VAR; j++)
			{
				for (int i = 0; i < n_dof; i++)
				{
					File.AppendAllText(textFile, robotStade[i][j] + "\t");
				}
			}
			File.AppendAllText(textFile, Environment.NewLine);*/

			if (robotStatus == 0)
				Debug.Log ("Disconnected?");
	//			connectStatus = ConnectStatus.disconnected;
		}
		return;
	}

    /// <summary>
    /// Initializes the variables.
    /// </summary>
	private void InitializeVariables()
	{
		gameStatus = new byte[sizeof(short)];
        gameStade = new byte[n_dof][][];
        robotStade = new float[n_dof][];

        SetStatus ((short)n_dof);
        for (int i = 0; i < n_dof; i++) 
		{
			gameStade[i] = new byte[N_VAR][];
			robotStade[i] = new float[N_VAR];
			for (int j = 0; j < N_VAR; j++)
			{
				SetStatus (i, 0f, j);
			}
		}
		return;
	}

    /// <summary>
    /// Closes the connection.
    /// </summary>
	public void CloseConnection()
	{
		Destroy (this);
	}

	void OnDestroy()
	{
		connectStatus = ConnectStatus.disconnected;
//		stopThread = true;
		connectingThread.Abort ();
		clientHere.Disconnect ();
	}
}