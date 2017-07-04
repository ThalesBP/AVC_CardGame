using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : GameBase {

    private int language;

    private Text[] hitRate;
    private Text[] timeRate;
    private Text[] info1;
    private Text[] info2;

    public HideShow visibility;

	void Start () 
    {
        hitRate = GameObject.Find("HitRate").GetComponentsInChildren<Text>();
        timeRate = GameObject.Find("TimeRate").GetComponentsInChildren<Text>();
        info1 = GameObject.Find("Info1").GetComponentsInChildren<Text>();
        info2 = GameObject.Find("Info2").GetComponentsInChildren<Text>();

        visibility = GetComponent<HideShow>();
	}
	
	void Update () 
    {
        language = (int)chosenLanguage;

        if (visibility.showed)
        {
            hitRate[0].text = hitRateText[language];
            timeRate[0].text = timeRateText[language];
            info1[0].text = info1Text[language];
            info2[0].text = info2Text[language];

            hitRate[1].text = (100f * Choice.totalMatches / Choice.orderCounter).ToString("F0") + "%";
            timeRate[1].text = Choice.AverageTimeToChoose.ToString("F1");
            info1[1].text = ControlManager.Instance.ankle.Bases.y.ToString("F1") + "%";
            info2[1].text = ControlManager.Instance.ankle.Bases.x.ToString("F1") + "%";

            hitRate[2].text = Choice.totalMatches.ToString("F0") + ofText[language] + Choice.orderCounter.ToString("F0");
            timeRate[2].text = fromText[language] + Choice.RangeTimeToChoose[0].ToString("F1") + toText[language] + Choice.RangeTimeToChoose[1].ToString("F1");

            info1[2].text = fromText[language] + ControlManager.Instance.ankle.Min.y.ToString("F1") + toText[language] + ControlManager.Instance.ankle.Max.y.ToString("F1");
            info2[2].text = fromText[language] + ControlManager.Instance.ankle.Min.x.ToString("F1") + toText[language] + ControlManager.Instance.ankle.Max.x.ToString("F1");
        }
	}
}
