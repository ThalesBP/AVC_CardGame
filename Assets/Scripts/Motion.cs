using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the motion of a Vector3 element along the time.
/// </summary>
public class Motion : GameBase {

    private enum MotionStatus {idle, waiting, moving, updating};
    private MotionStatus status;

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

    /// <summary>
    /// Returns true if status is idle.
    /// </summary>
    /// <value><c>true</c> if idle; otherwise, <c>false</c>.</value>
    public bool Idle
    {
        get { return (status == MotionStatus.idle); }
    }


    // Use this for initialization
	void Awake () 
    {
        initial = final = Vector3.zero;
        delay = 0f;
        counter = delta = 1f;
        status = MotionStatus.idle;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Counter();
        switch (status)
        {
            case MotionStatus.idle:
                break;
            case MotionStatus.moving:
                value = Vector3.Lerp(initial, final, Mathf.SmoothStep(0f, 1f, lerpScale));
                break;
            case MotionStatus.waiting:
                break;
            case MotionStatus.updating:
                MoveTo(final);
                status = MotionStatus.idle;
                break;
        }
	}

    /// <summary>
    /// Counts and checks the timer.
    /// </summary>
    private void Counter()
    {
        lerpScale = (counter - delay) / delta;

        if (status != MotionStatus.idle)
        {
            if (counter < delay)
            {
                status = MotionStatus.waiting;
                if (timeScaled)
                    counter += Time.deltaTime;
                else
                    counter += Time.unscaledDeltaTime;
            }
            else if (counter < delay + delta)
            {
                status = MotionStatus.moving;

                if (timeScaled)
                    counter += Time.deltaTime;
                else
                    counter += Time.unscaledDeltaTime;
            }
            else
                status = MotionStatus.updating;
        }
    }

    #region MoveTo functions
    /// <summary>
    /// Moves the card immediately to a position.
    /// </summary>
    /// <param name="destiny">Destiny place to move.</param>
    public float MoveTo(Vector3 destiny)
    {
        value = initial = final = destiny;
        return 0f;
    }

    /// <summary>
    /// Moves the card from where it is to destiny place.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    public float MoveTo(Vector3 destiny, float deltaTime)
    {
        if (deltaTime > 0)
        {
            counter = 0f;
            delta = deltaTime;
            final = destiny;
            status = MotionStatus.moving;
            lerpScale = (counter - delay) / delta;
            return deltaTime;
        }
        else
            return MoveTo(destiny);
    }

    /// <summary>
    /// Moves the card from where it is to destiny place after some time.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    /// <param name="delayTime">Time it waits to start moving.</param>
    public float MoveTo(Vector3 destiny, float deltaTime, float delayTime)
    {
        delay = delayTime;
        return MoveTo(destiny, deltaTime) + delayTime;
    }
    #endregion
}
