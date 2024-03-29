﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlManager : Singleton<ControlManager> {

    public enum ControlMode {Mouse, Joystick, Connection, ForceConnection};
    public enum HelperMode {None, GoIn, GoOut};


    [Space(5)]
    [Header("Input Mode")]
    public ControlMode mode;
    public bool joystick = false;

    [Space(5)]
    [Header("Spatial Infos")]
    [SerializeField]
    private Vector2 position = Vector2.zero;
    public Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
    [SerializeField]
    private float scale = 0.45f * Screen.height;

    [Space(5)]
    [Header("Simulator")]
    public bool forceConnection = false;
    [Range(-0.4f, 0.4f)]
    public float simulateRobotX = 0.0f;
    [Range(-0.6f, 0.6f)]
    public float simulateRobotY = 0.0f;
    [HideInInspector]
    public Vector2 simulateRobot;

    [Space(5)]
    [Header("Robot")]
    public int statusRobo = 1;
    public Vector2 centerSpring;
    public Vector2 freeSpace;
    /// <summary>
    /// X = Stiffness / Y = Damping
    /// </summary>
    public Vector2 impedance;
    public Vector2 outFreeSpace;

    public float stiffness = 0f;
    public float antiFriction = 0f;

    [Space(5)]
    [Header("Helper")]
    public HelperMode helper = HelperMode.None;
    public Motion helperPosition;
    [Range(0f, 1f)]
    public float helperLerp;
    public float forceLerp;

    public Vector2 objective;
    [Range(0.01f, 1.00f)]
    public float freeSpaceRadius = 0.10f;
    [Range(0.01f, 1.00f)]
    public float outFreeSpaceRadius = 0.80f;


    [Space(5)]
    [Header("Trigger")]
    [SerializeField]
    private float actionCounter;
    private float actionCheck;
    [SerializeField]
    private bool actionCounting, actionTrigger;

    [Space(5)]
    [Header("Others")]
    public Connection connection;
    public AnkleMovement ankle;

    public Vector2 Position
    {
        get { return position; }
    }

    public Vector2 RawPosition
    {
        get
        {
            switch (mode)
            {
                case ControlMode.Connection:
                    return connection.Position;
                case ControlMode.ForceConnection:
                    //return ankle.CircleToElipse((Position - center).normalized * Mathf.Clamp((Position - center).magnitude, 0f, Screen.height * 0.45f), Screen.height * 0.45f);
                    return ankle.CircleToElipse(Position - center, Screen.height * 0.45f);
                case ControlMode.Joystick:
                    return Position;
                case ControlMode.Mouse:
                    return ankle.CircleToElipse((Position - center).normalized * Mathf.Clamp((Position - center).magnitude, 0f, Screen.height * 0.45f), Screen.height * 0.45f);
                default:
                    return Vector2.zero;
            }
        }
    }

    public float Loading
    {
        get { return actionCounter / LoadingTime[Medium]; }
    }

    public float RawLoading
    {
        get { return actionCounter; }
    }

    public bool Action
    {
        get { return GetAction(); }
    }

	// Use this for initialization
	void Start () 
    {
        actionCounter = actionCheck = 0f;
        actionCounting = actionTrigger = false;

        simulateRobot = Vector2.zero;
        freeSpace = outFreeSpace = Vector2.zero;

        ankle = gameObject.AddComponent<AnkleMovement>();

        helperPosition = gameObject.AddComponent<Motion>();
        helperPosition.timeScaled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        simulateRobot = new Vector2(simulateRobotX, simulateRobotY);
        scale = 0.45f * Screen.height;

        if (connection != null)
            mode = ControlMode.Connection;
        else if (forceConnection)
            mode = ControlMode.ForceConnection;
        else if (joystick)
            mode = ControlMode.Joystick;
        else
            mode = ControlMode.Mouse;

        forceLerp = Mathf.Clamp(forceLerp + Time.deltaTime, 0f, 1f);
        switch (helper)
        {
            case HelperMode.None:
                impedance = Vector2.Lerp(impedance, Vector2.zero, forceLerp);
                centerSpring = ankle.CircleToElipse(helperPosition.Value, Screen.height * 0.45f);

                if (connection != null)
                    connection.Status = Connection.ControlStatus.noHelper;
                break;
            case HelperMode.GoIn:
                impedance = Vector2.Lerp(impedance, new Vector2(stiffness, 0f), forceLerp);
                centerSpring = ankle.CircleToElipse(helperPosition.Value, Screen.height * 0.45f);
                freeSpaceRadius = Mathf.Lerp(1.50f, 0.10f, helperLerp);
                outFreeSpaceRadius = 0.95f;

                freeSpace = freeSpaceRadius * ankle.bases;
                outFreeSpace = outFreeSpaceRadius * ankle.bases;

                if (connection != null)
                    connection.Status = Connection.ControlStatus.helperIn;
                break;
            case HelperMode.GoOut:
                impedance = Vector2.Lerp(impedance, new Vector2(antiFriction, 0f), forceLerp);;

                freeSpaceRadius = 0.10f;
                outFreeSpaceRadius = 0.80f;

                centerSpring = new Vector2(freeSpaceRadius, freeSpaceRadius + 0.1f);
                freeSpace = new Vector2(outFreeSpaceRadius, outFreeSpaceRadius + 0.1f);

             //   centerSpring = ankle.CircleToElipse(Vector2.zero, Screen.height * 0.45f);
             //   freeSpace = freeSpaceRadius * ankle.bases;
                outFreeSpace = outFreeSpaceRadius * ankle.bases;

                if (connection != null)
                    connection.Status = Connection.ControlStatus.helperOut;
                break;
        }

        switch (mode)
        {
            case ControlMode.Connection:
                Cursor.visible = true;
                if (!connection.connected)
                    Debug.Log("Lost of connection");
                    //ankle.Reset();
                position = scale * ankle.ElipseToCircle(connection.Position) + center;

                connection.CenterSpring = centerSpring;
                connection.FreeSpace = freeSpace;
                connection.Impedance = impedance;
                connection.OutFreeSpace = outFreeSpace;
                break;
            case ControlMode.ForceConnection:
                float mag = ((Vector2)Input.mousePosition - center).magnitude;
                /*position = ((Vector2)Input.mousePosition - center).normalized * Mathf.Clamp(mag, 0f, Screen.height * 0.45f) + center;
                if (mag > Screen.height * 0.45f)
                    Cursor.visible = true;
                else
                    Cursor.visible = false;
                */

                Cursor.visible = true;
                position = scale * ankle.ElipseToCircle(simulateRobot) + center;

          //      simulateRobot = ankle.CircleToElipse((Vector2)Input.mousePosition - center, Screen.height * 0.45f);
          //      freeSpace = ankle.CircleToElipse(Vector2.one * Screen.height * 0.05f, Screen.height * 0.45f);
         
                //simulateRobot = ankle.CircleToElipse(simulateMouse - center, Screen.height * 0.45f);
                //freeSpace = ankle.CircleToElipse(simulateMouse - center + Vector2.one * Screen.height * 0.05f, Screen.height * 0.45f) - simulateRobot;
                //outFreeSpace = ankle.CircleToElipse(simulateMouse - center + Vector2.one * Screen.height * 0.40f, Screen.height * 0.45f) - simulateRobot;

                simulateRobot = ankle.CircleToElipse(Vector2.zero, Screen.height * 0.45f);
                freeSpace = 0.10f * ankle.bases;
                outFreeSpace = 0.80f * ankle.bases;
                break;
            case ControlMode.Joystick:
                Cursor.visible = true;
                position = new Vector2(Input.GetAxis("Horizontal") * 300f, Input.GetAxis("Vertical") * 300f) + center;
                break;
            case ControlMode.Mouse:
                Cursor.visible = false;
                ankle.SetRadius(0.5f);
                position = Input.mousePosition;
                break;
        }

        if (actionCounting)
        {
            actionCounter += Time.deltaTime;
            if (actionCounter < actionCheck + 5f * Time.unscaledDeltaTime)
            {   
                if (actionCounter > LoadingTime[Medium])
                    actionTrigger = true;
            }
            else
            {
                actionCounting = false;
                actionCounter = actionCheck = 0f;
            }
        }
    }

    /// <summary>
    /// Gets the player's action.
    /// </summary>
    /// <returns><c>true</c>, if action was gotten, <c>false</c> otherwise.</returns>
    private bool GetAction()
    {
        switch (mode)
        {
            case ControlMode.Mouse:
                return Input.GetMouseButton(0);
            default:
                actionCounting = true;
                if (actionTrigger)
                {
                    actionTrigger = false;
                    actionCounting = false;
                    actionCounter = actionCheck = 0f;       
                    return true;
                }
                else
                {
                    actionCheck = actionCounter;
                    return false;
                }
        }
    }

    public void SetHelper(HelperMode mode, Vector2 center)
    {
        if (helper != mode)
        {
            helperPosition.MoveTo((Vector3)center, 1f);
            helper = mode;
            forceLerp = 0f;
        }
    }
}
