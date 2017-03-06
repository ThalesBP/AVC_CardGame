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
	private float time;
	public float slideTime, distance;

	[SerializeField] Axis axis;

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
		slideTime = 2.5f;
		time = 0f;
	}

	void Update()
	{
        if (moving)
        {
            transf.anchoredPosition = Vector2.Lerp(transf.anchoredPosition, target, time * slideTime);
            time += Time.unscaledDeltaTime;
            if (time * slideTime > 1f)
                moving = false;
        }
	}
	
	/// <summary>
    /// Shows this instance.
    /// </summary>
	public void Show () 
	{
		target = show;
        moving = true;
		time = 0;
	}

    /// <summary>
    /// Hides this instance.
    /// </summary>
	public void Hide () 
	{
		target = hide;
        moving = true;
		time = 0;
	}
}
