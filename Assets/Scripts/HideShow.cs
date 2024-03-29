﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to hide and show a 2D object in the scene
/// </summary>
public class HideShow : MonoBehaviour {

	enum Axis {vertical, horizontal};

	private Vector2 hide, show, target;
	private RectTransform transf;
    [SerializeField]
    private bool moving;
	public float slideTime, distance, slideTimeLerp;
    public bool showed, locked;

	[SerializeField] private Axis axis;

	// Use this for initialization
	void Start () 
	{
		transf = GetComponent<RectTransform> ();
        moving = locked = false;
		show = transf.anchoredPosition;
		if (axis == Axis.horizontal)
			hide = transf.anchoredPosition + Vector2.Scale (transf.sizeDelta, Vector2.right * distance);
		else
			hide = transf.anchoredPosition + Vector2.Scale (transf.sizeDelta, Vector2.up * distance);
		transf.anchoredPosition = target = hide;
		slideTime = 0.4f;
        slideTimeLerp = 1f;
	}

	void Update()
	{
        if (moving)
        {
            transf.anchoredPosition = Vector2.Lerp(transf.anchoredPosition, target, slideTimeLerp);
            slideTimeLerp += Time.unscaledDeltaTime / slideTime;
            if (slideTimeLerp > 1f)
                moving = false;
        }
	}
	
	/// <summary>
    /// Shows this instance.
    /// </summary>
	public void Show () 
	{
        if (!showed && !locked)
        {
            showed = true;
            target = show;
            moving = true;
            slideTimeLerp = 0;
        }
	}

    /// <summary>
    /// Hides this instance.
    /// </summary>
	public void Hide () 
	{
        if (showed && !locked)
        {
            showed = false;
            target = hide;
            moving = true;
            slideTimeLerp = 0;
        }
	}

    /// <summary>
    /// Toggles between hidden and showed.
    /// </summary>
    public void Toggle()
    {
        if (showed)
            Hide();
        else
            Show();
    }

    /// <summary>
    /// Moves to a relative position.
    /// </summary>
    /// <param name="posRel">Position relative to showed in percent.</param>
    public void MoveTo (float posRel)
    {
        if (!locked)
        {
            if (axis == Axis.horizontal)
                target = transf.anchoredPosition + Vector2.Scale(transf.sizeDelta, Vector2.right * (1f - 0.01f * posRel));
            else
                target = transf.anchoredPosition + Vector2.Scale(transf.sizeDelta, Vector2.up * (1f - 0.01f * posRel));
            showed = true;
            moving = true;
            slideTimeLerp = 0;
        }
    }
}
