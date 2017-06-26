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
    [SerializeField]
    private Vector2 scale = 3000f * Vector2.one;
    [SerializeField]
    private float actionCounter, actionCheck;
    [SerializeField]
    private bool actionCounting, actionTrigger;

    public bool forceActionCounter = false;
    //public float scale = 3000f;
    public Connection connection;

    public Vector2 Position
    {
        get { return position; }
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
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (connection == null)
        {
            position = Input.mousePosition;
        }
        else
        {
            if (connection.connected)
            {
                if (connection.Position != null)
                {
                    Debug.Log(connection.Position);
                    position = Vector2.Scale(connection.Position, scale) + center;
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
        if ((connection == null) && (!forceActionCounter))
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
