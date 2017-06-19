using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotCursor : MonoBehaviour {

    public Canvas screen;
    public UserPanel user;
    private RawImage cursor;
    private Image loading;
    private ControlManager input;
    private Vector2 offset;

	// Use this for initialization
	void Start () 
    {
        cursor = GetComponent<RawImage>();
        loading = gameObject.GetComponentInChildren<Image>();
        offset = new Vector2(cursor.rectTransform.rect.width / 2f, -cursor.rectTransform.rect.height / 2f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (user.memberDropdown.value == 0)
        {
            offset = new Vector2(cursor.rectTransform.rect.width / 2f, -cursor.rectTransform.rect.height / 2f);
            cursor.rectTransform.localScale = new Vector3(1f / screen.scaleFactor, 1f / screen.scaleFactor, 1f);
        }
        else
        {
            offset = new Vector2(-cursor.rectTransform.rect.width / 2f, -cursor.rectTransform.rect.height / 2f);
            cursor.rectTransform.localScale = new Vector3(-1f / screen.scaleFactor, 1f / screen.scaleFactor, 1f);
        }
        cursor.rectTransform.position = ControlManager.Instance.Position + offset;
        loading.fillAmount = ControlManager.Instance.Loading;
	}
}
