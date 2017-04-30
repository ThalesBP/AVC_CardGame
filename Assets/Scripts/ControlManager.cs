using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlManager : Singleton<ControlManager> {

    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private float actionCounter, actionCheck;
    [SerializeField]
    private bool actionCounting, actionTrigger;

    public Vector2 Position
    {
        get { return position; }
    }

    public bool Action
    {
        get { return GetAction(); }
    }

	// Use this for initialization
	void Start () 
    {
        actionCounter = 0f;
        actionCounting = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        position = Input.mousePosition;
        if (actionCounting)
        {
            if (actionCheck < actionCounter)
            {
                actionCheck = actionCounter;
                if (actionCounter > 2f)
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
        actionCounting = true;
        actionCounter += Time.deltaTime;
        return Input.GetMouseButton(0);
    }
}
