using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls interface between the game and player.
/// </summary>
public class ControlManager : Singleton<ControlManager> {

    [SerializeField]
    private Vector2 Position;
    [SerializeField]
    private float ActionCounter, ActionCheck;
    [SerializeField]
    private bool ActionCounting, ActionTrigger;

	// Use this for initialization
	void Start () 
    {
        ActionCounter = 0f;
        ActionCounting = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Position = Input.mousePosition;
        if (ActionCounting)
        {
            if (ActionCheck < ActionCounter)
            {
                ActionCheck = ActionCounter;
                if (ActionCounter > 2f)
                    ActionTrigger = true;
            }
            else
            {
                ActionCounting = false;
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
        ActionCounting = true;
        ActionCounter += Time.deltaTime;
        return Input.GetMouseButton(0);
    }
}
