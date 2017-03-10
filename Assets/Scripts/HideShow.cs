using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used to hide and show a 2D object in the scene
/// </summary>
public class HideShow : MonoBehaviour {

	enum Axis {vertical, horizontal};

	private Vector2 hide, show, target;
	private RectTransform transf;
    private bool moving;
	public float slideTime, distance, slideTimeLerp;
    public bool showed;

	[SerializeField] private Axis axis;

	// Use this for initialization
	void Start () 
	{
		transf = GetComponent<RectTransform> ();
        moving = false;
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
        showed = true;
		target = show;
        moving = true;
        slideTimeLerp = 0;
	}

    /// <summary>
    /// Hides this instance.
    /// </summary>
	public void Hide () 
	{
        showed = false;
		target = hide;
        moving = true;
        slideTimeLerp = 0;
	}
}
