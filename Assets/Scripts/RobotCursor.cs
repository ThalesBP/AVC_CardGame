using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotCursor : MonoBehaviour {

    public Canvas screen;
    public UserPanel user;
    private RawImage cursor;
    private Image loading;
    private Vector2 offset;

    public Vector3 rotation;

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
        if (user.memberDropdown.value == 1)
        {
            offset = new Vector2(cursor.rectTransform.rect.width / 2f, -cursor.rectTransform.rect.height / 2f);
            cursor.rectTransform.localScale = new Vector3(1f / screen.scaleFactor, 1f / screen.scaleFactor, 1f);
//            rotation = new Vector3(1f, 1f, - 60f * Mathf.Clamp((ControlManager.Instance.Position.x - Screen.width / 2f) / Screen.width, -30f, 30f));
        }
        else
        {
            offset = new Vector2(-cursor.rectTransform.rect.width / 2f, -cursor.rectTransform.rect.height / 2f);
            cursor.rectTransform.localScale = new Vector3(-1f / screen.scaleFactor, 1f / screen.scaleFactor, 1f);
//            rotation = new Vector3(1f, 1f, - 60f * Mathf.Clamp((ControlManager.Instance.Position.x - Screen.width / 2f) / Screen.width, -30f, 30f));
        }
        cursor.rectTransform.position = ControlManager.Instance.Position + offset;
        loading.fillAmount = ControlManager.Instance.Loading;

//        cursor.rectTransform.localRotation = Quaternion.Euler(rotation);
	}
}
