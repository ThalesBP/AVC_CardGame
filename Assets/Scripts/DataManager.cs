using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DataManager : Singleton<DataManager> {

	public float hit, miss, time, lazy, plantar, dorsi, inver, ever;
	public Text hitText, currentTimeText, pdfText, ievText;
	public Text rateHitText, rateTimeText, rangePdfText, rangeIevText;

	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		rateHitText.text = (hit / (hit + miss)).ToString ("P0");
		rateTimeText.text = (time / (time + lazy)).ToString ("P0");
		rangePdfText.text = ((plantar - dorsi) * 180 / Mathf.PI).ToString ("F1") + "º";
		rangeIevText.text = ((inver - ever) * 180 / Mathf.PI).ToString ("F1") + "º";

		hitText.text = hit.ToString("F1") + " / " + miss.ToString ("F1");
		currentTimeText.text = time.ToString("F1") + " / " + lazy.ToString ("F1");
		pdfText.text = (plantar * 180 / Mathf.PI).ToString("F1") + "º / " + (dorsi * 180 / Mathf.PI).ToString("F1") + "º";
		ievText.text = (inver * 180 / Mathf.PI).ToString("F1") + "º / " + (ever * 180 / Mathf.PI).ToString("F1") + "º";
	}

	public void UpdateVars(float hitNumbers, float missNumers, float playTime, float lazyTime, float maxPlantar, float maxDorsi, float maxInver, float maxEver)
	{
		hit = hitNumbers;
		miss = missNumers;
		time = playTime;
		lazy = lazyTime;
		plantar = maxPlantar;
		dorsi = maxDorsi;
		inver = maxInver;
		ever = maxEver;
	}

	public void UpdateScore(float hitNumbers, float missNumers)
	{
		hit = hitNumbers;
		miss = missNumers;
	}

	public void UpdateROM(float playTime, float lazyTime, float maxPlantar, float maxDorsi, float maxInver, float maxEver)
	{
		time = playTime;
		lazy = lazyTime;
		plantar = maxPlantar;
		dorsi = maxDorsi;
		inver = maxInver;
		ever = maxEver;
	}
}
