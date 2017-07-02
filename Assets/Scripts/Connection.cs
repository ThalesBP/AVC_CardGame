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

    public enum GameStatus {Paused, Starting, Playing, Stopped, Restarting};
    enum ConnectStatus {waiting, connecting, connected, disconnected};
    enum RobotIndex {centerspring, freespace, stiff, damp};
    enum GameIndex {position, velocity, acc, force};
    enum Axis {vertical, horizontal};

	private const int N_VAR = 4; 		// Numero de variaveis envolvidas
	private const int  N_DOF = 2;        // Degrees of freedon

	private byte[][][] gameStade;
    private byte[] gameStatus;
    private float[][] robotStade;
	private short robotStatus;

    private const float stepTime = 0.02f;
	private float timeCounter = 0;

	private Thread connectingThread;

	//public bool connected;
	ConnectStatus connectStatus;

    public int RobotStatus
    {
        get
        {
            if (connected)
                return (int)robotStatus;
            else
                return -1;
        }
    }

    public Vector2 Position 
    {
        get 
        {
            if (connected)
                return  new Vector2(robotStade[(int)Axis.horizontal][(int)GameIndex.position], robotStade[(int)Axis.vertical][(int)GameIndex.position]);
            else
                return Vector2.zero;
        }
    }

    public Vector2 Velocity
    {
        get 
        {
            if (connected)
                return new Vector2(robotStade[(int)Axis.horizontal][(int)GameIndex.velocity], robotStade[(int)Axis.vertical][(int)GameIndex.velocity]);
            else
                return Vector2.zero;
        }
    }

    public Vector2 Acceleration 
    {
        get 
        {
            if (connected)
                return new Vector2(robotStade[(int)Axis.horizontal][(int)GameIndex.acc], robotStade[(int)Axis.vertical][(int)GameIndex.acc]);
            else
                return Vector2.zero;
        }
    }

    public Vector2 Force 
    {
        get 
        {
            if (connected)
                return new Vector2(robotStade[(int)Axis.horizontal][(int)GameIndex.force], robotStade[(int)Axis.vertical][(int)GameIndex.force]);
            else
                return Vector2.zero;
        }
    }

    public GameStatus Status
    {
        get
        { 
            return (GameStatus)BitConverter.ToInt16(gameStatus, 0);
        }
        set 
        {
            SetStatus((short)value);
        }
    }

    public Vector2 CenterSpring
    {
        get
        { 
            return new Vector2(BitConverter.ToSingle(gameStade[(int)Axis.horizontal][(int)RobotIndex.centerspring], 0), BitConverter.ToSingle(gameStade[(int)Axis.vertical][(int)RobotIndex.centerspring], 0));
        }
        set 
        {
            SetStatus((int)Axis.horizontal, (int)RobotIndex.centerspring, value.x);
            SetStatus((int)Axis.vertical, (int)RobotIndex.centerspring, value.y);
        }
    }

    public Vector2 FreeSpace
    {
        get
        { 
            return new Vector2(BitConverter.ToSingle(gameStade[(int)Axis.horizontal][(int)RobotIndex.freespace], 0), BitConverter.ToSingle(gameStade[(int)Axis.vertical][(int)RobotIndex.freespace], 0));
        }
        set 
        {
            SetStatus((int)Axis.horizontal, (int)RobotIndex.freespace, value.x);
            SetStatus((int)Axis.vertical, (int)RobotIndex.freespace, value.y);
        }
    }

    public Vector2 Stiffness
    {
        get
        { 
            return new Vector2(BitConverter.ToSingle(gameStade[(int)Axis.horizontal][(int)RobotIndex.stiff], 0), BitConverter.ToSingle(gameStade[(int)Axis.vertical][(int)RobotIndex.stiff], 0));
        }
        set 
        {
            SetStatus((int)Axis.horizontal, (int)RobotIndex.stiff, value.x);
            SetStatus((int)Axis.vertical, (int)RobotIndex.stiff, value.y);
        }

    }

    public Vector2 Damping
    {
        get
        { 
            return new Vector2(BitConverter.ToSingle(gameStade[(int)Axis.horizontal][(int)RobotIndex.damp], 0), BitConverter.ToSingle(gameStade[(int)Axis.vertical][(int)RobotIndex.damp], 0));
        }
        set 
        {
            SetStatus((int)Axis.horizontal, (int)RobotIndex.damp, value.x);
            SetStatus((int)Axis.vertical, (int)RobotIndex.damp, value.y);
        }

    }

    public bool connected;
/*    {
        get { return clientHere.IsConnected(); }
    }
*/
//================================
	private NetworkClientTCP clientHere = new NetworkClientTCP();
//================================

//	private string textFile = @"D:\Users\Thales\Documents\Faculdade\2015 - 201x - Mestrado\RehabLab\RehabSystem\Rehab System\Rehab System IMT\DoublePingPong\Logs\LogFilePos.txt";

	void Start()
	{
        robotStatus = -1;
        connectingThread = new Thread (Connect);
		connectingThread.Start ();
		if (clientHere.IsConnected ())
			connectStatus = ConnectStatus.connected;
		else
			connectStatus = ConnectStatus.waiting;
	}


	void Update()
	{
        if (timeCounter > stepTime)
        {
            connected = clientHere.IsConnected();
            switch (connectStatus)
            {
                case ConnectStatus.connected:
                    SendMsg();
                    ReadMsg();
                    break;
            }
            timeCounter = 0f;
        }
        else
            timeCounter += Time.unscaledDeltaTime;
	}

	void Connect()
	{
		bool inLoop = true;
		while (inLoop)
		{
			switch (connectStatus)
			{
                case ConnectStatus.waiting:
                    Debug.Log("Starting connection");
                    clientHere.Connect("192.168.0.66", 8000, 0, sizeof(short) + N_VAR * sizeof(float) * N_DOF); // Here 192.168.0.67
					//	clientHere.SendString ("Connected!"); 

                    InitializeVariables();
                    connectStatus = ConnectStatus.connecting;
					break;
                case ConnectStatus.connecting:
                    Debug.Log("Trying to connect");
                    if (clientHere.IsConnected())
                    {
                        connectStatus = ConnectStatus.connected;
                        Debug.Log("Connected");
                    }
					break;
                case ConnectStatus.connected:
                    inLoop = false;
                    break;
                default:
                    Debug.Log("Erro!!");
					break;
			}
		}
	}

    /// <summary>
    /// Sets the status.
    /// </summary>
    /// <param name="dof">Dof.</param>
    /// <param name="variable">Variable.</param>
    /// <param name="mag">Mag.</param>
    public void SetStatus(int dof, int variable, float mag)
	{
        gameStade [dof] [variable] = BitConverter.GetBytes (mag);
		return;
	}

	public void SetStatus(short status)
	{
		gameStatus = BitConverter.GetBytes((short)(status + 1));
	}

	public float ReadStatus(int dof, int variable)
	{
        return robotStade [dof] [variable];
	}

	public short ReadStatus()
	{
		return robotStatus;
	}

	private void SendMsg ()
	{
        byte[] msg = new byte[sizeof(short) + (N_VAR * sizeof(float)) * N_DOF];
		System.Buffer.BlockCopy (gameStatus, 0, msg, 0, sizeof(short));
        for (int i_dof = 0; i_dof < N_DOF; i_dof++) 
		{
            for (int i_var = 0; i_var < N_VAR; i_var++)
			{
                System.Buffer.BlockCopy (gameStade [i_dof][i_var], 0, msg, sizeof(short) + sizeof(float) * (i_var + N_VAR * i_dof), gameStade [i_dof][i_var].Length);
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
            robotStatus = BitConverter.ToInt16(buffer, 0);
            for (int i_dof = 0; i_dof < N_DOF; i_dof++)
            {
                for (int i_var = 0; i_var < N_VAR; i_var++)
                {
                    robotStade[i_dof][i_var] = BitConverter.ToSingle(buffer, sizeof(short) + sizeof(float) * (i_var + N_VAR * i_dof));
                }
                //				Debug.Log ("Robot " + (i+1) + "- Pos: " + robotStade[i][0].ToString() + ", Vel:" + robotStade[i][1].ToString() + ", Acc:" + robotStade[i][2].ToString() + ", For:" + robotStade[i][3].ToString());
            }
/*			for (int j = 0; j < N_VAR; j++)
			{
				for (int i = 0; i < N_DOF; i++)
				{
					File.AppendAllText(textFile, robotStade[i][j] + "\t");
				}
			}
			File.AppendAllText(textFile, Environment.NewLine);*/

            if (robotStatus == 0)
                Debug.Log("Disconnected?");
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
        gameStade = new byte[N_DOF][][];
        robotStatus = 0;
        robotStade = new float[N_DOF][];

        SetStatus (0);
        for (int i_dof = 0; i_dof < N_DOF; i_dof++) 
		{
            gameStade[i_dof] = new byte[N_VAR][];
            robotStade[i_dof] = new float[N_VAR];
            for (int i_var = 0; i_var < N_VAR; i_var++)
			{
                robotStade[i_dof][i_var] = 0f;
                SetStatus (i_dof, i_var, 0f);
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