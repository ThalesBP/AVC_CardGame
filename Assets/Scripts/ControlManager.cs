﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlManager : Singleton<ControlManager> {

    public enum ControlMode {Mouse, Joystick, Connection, ForceConnection};
    public ControlMode mode;

    [SerializeField]
    private Vector2 position = Vector2.zero;
    public Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);


    [Range(-0.4f, 0.4f)]
    public float simulateRobotX = 0.0f;
    [Range(-0.6f, 0.6f)]
    public float simulateRobotY = 0.0f;
    [HideInInspector]
    public Vector2 simulateRobot;
    public Vector2 freeSpace;
    public Vector2 simulateMouse;
    public float stiff = 0f, damp = 0f;

    [SerializeField]
    private float scale = 0.45f * Screen.height;

    [SerializeField]
    private float actionCounter, actionCheck;
    [SerializeField]
    private bool actionCounting, actionTrigger;

    public bool forceConnection = false;
    public bool joystick = false;
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
        freeSpace = Vector2.zero;
        simulateMouse = new Vector2(Screen.width / 2f, Screen.height / 2f);

        ankle = gameObject.AddComponent<AnkleMovement>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        center = new Vector2(Screen.width / 2f, Screen.height / 2f);
 //       simulateRobot = new Vector2(simulateRobotX, simulateRobotY);
        scale = 0.45f * Screen.height;

        if (connection != null)
            mode = ControlMode.Connection;
        else if (forceConnection)
            mode = ControlMode.ForceConnection;
        else if (joystick)
            mode = ControlMode.Joystick;
        else
            mode = ControlMode.Mouse;

        switch (mode)
        {
            case ControlMode.Connection:
                Cursor.visible = true;
                if (!connection.connected)
                    Debug.Log("Lost of connection");
                    //ankle.Reset();
                position = scale * ankle.ElipseToCircle(connection.Position) + center;

                simulateRobot = ankle.CircleToElipse(simulateMouse - center, Screen.height * 0.45f);
                freeSpace = ankle.CircleToElipse(simulateMouse - center + Vector2.one * Screen.height * 0.05f, Screen.height * 0.45f) - simulateRobot;

                connection.CenterSpring = simulateRobot;
                connection.FreeSpace = freeSpace;
                connection.Stiffness = new Vector2(0f , stiff);
                connection.Damping = new Vector2(0f , damp);

                break;
            case ControlMode.ForceConnection:
                float mag = ((Vector2)Input.mousePosition - center).magnitude;
                /*position = ((Vector2)Input.mousePosition - center).normalized * Mathf.Clamp(mag, 0f, Screen.height * 0.45f) + center;
                if (mag > Screen.height * 0.45f)
                    Cursor.visible = true;
                else
                    Cursor.visible = false;
                */

                simulateRobot = new Vector2(simulateRobotX, simulateRobotY);
                position = scale * ankle.ElipseToCircle(simulateRobot) + center;

                simulateRobot = ankle.CircleToElipse((Vector2)Input.mousePosition - center, Screen.height * 0.45f);
                freeSpace = ankle.CircleToElipse((Vector2)Input.mousePosition - center + Vector2.one * Screen.height * 0.05f, Screen.height * 0.45f) - simulateRobot;
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
}
