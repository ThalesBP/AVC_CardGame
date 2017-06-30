using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlManager : Singleton<ControlManager> {

    [SerializeField]
    private Vector2 position;
    private Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);


    [Range(-0.4f, 0.4f)]
    public float simulateRobotX = 0.0f;
    [Range(-0.6f, 0.6f)]
    public float simulateRobotY = 0.0f;

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
            if ((connection == null) && (!forceConnection))
                return Position;
            else if (forceConnection)
                return new Vector2(simulateRobotX, simulateRobotY);
            else
                return connection.Position;
        }
    }

    public float Loading
    {
        get { return actionCounter / LoadingTime[Medium]; }
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

        ankle = gameObject.AddComponent<AnkleMovement>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if ((connection == null) && (!forceConnection))
        {
            if (joystick)
                position = new Vector2(Input.GetAxis("Horizontal") * 300f, Input.GetAxis("Vertical") * 300f) + center;
            else
                position = Input.mousePosition;
        }
        else
        {
            if (forceConnection)
                position = scale * ankle.ElipseToCircle(new Vector2(simulateRobotX, simulateRobotY)) + center;
            else                
                if (connection.connected)
                {
                    if (connection.Position != null)
                    {
                        Debug.Log(connection.Position);
                        position = scale * ankle.ElipseToCircle(connection.Position) + center;
                    }
                }
                else
                {
                        position = center;
                }
        }
        if (actionCounting)
        {
            actionCounter += Time.deltaTime;
            if (actionCounter < actionCheck + 0.1f)
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
        if ((connection == null) && (!forceConnection))
            return Input.GetMouseButton(0);
        else
        {
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
