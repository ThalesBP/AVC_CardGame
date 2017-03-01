using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlInterface : Singleton<ControlInterface> {

    [SerializeField]
    private Vector2 Position;
    [SerializeField]
    private float ActionCounter, ActionCheck;
    [SerializeField]
    private bool ActionTrigger;

	// Use this for initialization
	void Start () 
    {
        ActionCounter = 0f;
        ActionTrigger = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Position = Input.mousePosition;
        if (ActionTrigger)
        {
            if (ActionCheck < ActionCounter)
                ActionCheck = ActionCounter;
            else
            {
                ActionTrigger = false;
                ActionCounter = ActionCheck = 0f;
            }
        }
    }

    /// <summary>
    /// Gets the player's position.
    /// </summary>
    /// <returns>The position.</returns>
    public Vector2 GetPosition()
    {
        return Position;
    }

    /// <summary>
    /// Gets the player's action.
    /// </summary>
    /// <returns><c>true</c>, if action was gotten, <c>false</c> otherwise.</returns>
    public bool GetAction()
    {
        ActionTrigger = true;
        ActionCounter += Time.deltaTime;
        return Input.GetMouseButton(0);
    }
}
