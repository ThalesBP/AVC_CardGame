﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the motion of a Vector3 element along the time.
/// </summary>
public class Motion : GameBase {

    public enum Status {idle, waiting, moving, updating};
    public Status status;

    private Vector3 initial, value, final;
    private float counter, delay, delta;
    private float lerpScale;

    public float LerpScale
    {
        get { return Mathf.Clamp(lerpScale, 0f, 1f); }
    }
    public bool timeScaled = true;

    public Vector3 Value
    {
        get { return value; }
//        set {this.value = value;}
    }
    public static implicit operator Vector3(Motion motion)
    {
        return motion.value;
    }


    // Use this for initialization
	void Awake () 
    {
        initial = final = Vector3.zero;
        delay = 0f;
        counter = delta = 1f;
        status = Status.idle;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Counter();
        switch (status)
        {
            case Status.idle:
                break;
            case Status.moving:
                value = Vector3.Lerp(initial, final, Mathf.SmoothStep(0f, 1f, lerpScale));
                break;
            case Status.waiting:
                break;
            case Status.updating:
                MoveTo(final);
                status = Status.idle;
                break;
        }
	}

    /// <summary>
    /// Counts and checks the timer.
    /// </summary>
    private void Counter()
    {
        lerpScale = (counter - delay) / delta;

        if (status != Status.idle)
        {
            if (counter < delay)
            {
                status = Status.waiting;
                if (timeScaled)
                    counter += Time.deltaTime;
                else
                    counter += Time.unscaledDeltaTime;
            }
            else if (counter < delay + delta)
            {
                status = Status.moving;

                if (timeScaled)
                    counter += Time.deltaTime;
                else
                    counter += Time.unscaledDeltaTime;
            }
            else
                status = Status.updating;
        }
    }

    #region MoveTo functions
    /// <summary>
    /// Moves the card immediately to a position.
    /// </summary>
    /// <param name="destiny">Destiny place to move.</param>
    public void MoveTo(Vector3 destiny)
    {
        value = initial = final = destiny;
    }

    /// <summary>
    /// Moves the card from where it is to destiny place.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    public void MoveTo(Vector3 destiny, float deltaTime)
    {
        if (deltaTime > 0)
        {
            counter = 0f;
            delta = deltaTime;
            final = destiny;
            status = Status.moving;
            lerpScale = (counter - delay) / delta;
        }
        else
            MoveTo(destiny);
    }

    /// <summary>
    /// Moves the card from where it is to destiny place after some time.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    /// <param name="delayTime">Time it waits to start moving.</param>
    public void MoveTo(Vector3 destiny, float deltaTime, float delayTime)
    {
        delay = delayTime;
        MoveTo(destiny, deltaTime);
    }
    #endregion
}
