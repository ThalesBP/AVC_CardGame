using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    private int language;
//    private enum PlayStatus {waiting, counting, playing};
  //  public PlayStatus countingStatus;

    #region Interface's texts
    public Text playStatus;
    private Motion statusScale;
    private Outline statusBoarder;

    private Text[] hitRate;
    private Text[] timeRate;
    private Text[] info1;
    private Text[] info2;

    private Text scorePoints;

    private Text metric1;
    private Text metric2;
    private Text metric3;

    [SerializeField]
    private Text gameMessages;

    private Text panelName;
    #endregion

    #region GameVariables
    public int countDownCounter;

    public float hitRateValue;
    public float timeRateValue;
    public float info1Value;
    public float info2Value;

    public float metric1Value;
    public float metric2Value;
    public float metric3Value;

    public int scoreValue;
    public Messages gameMessage = Messages.newGame;
    #endregion

	// Use this for initialization
	void Start () 
    {
        countDownCounter = -1;

        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>();
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusBoarder = playStatus.GetComponent<Outline>();

        hitRate = GameObject.Find("HitRate").GetComponentsInChildren<Text>();
        timeRate = GameObject.Find("TimeRate").GetComponentsInChildren<Text>();
        info1 = GameObject.Find("Info1").GetComponentsInChildren<Text>();
        info2 = GameObject.Find("Info2").GetComponentsInChildren<Text>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>();

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>();
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>();
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>();

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>();

        panelName = GameObject.Find("PanelName").GetComponentInChildren<Text>();
	}
	// Update is called once per frame
	void Update ()
    {
        language = (int)chosenLanguage;
		
        scorePoints.text = scorePointsText[language] + "\n" + scoreValue.ToString("F0"); 

        if (countDownCounter >= 0)
        {
//            if (countDownCounter > CountDown)
  //          {
    //            playStatus.text = readyText[language];
      //      }
        //    else
          //  {
            if (statusScale.status == Motion.Status.idle)
            {
                countDownCounter--;
                statusScale.MoveTo(Vector3.one);
                if (countDownCounter == 0)
                    playStatus.text = goText[language];
                else
                    playStatus.text = countDownCounter.ToString();
                
                statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[MuchLonger]);
                /*                switch (countDownCounter)
                    {
                        case -1:
                            playStatus.text = "";
                            break;
                        case 0:
                            playStatus.text = goText[language];
                            break;
                        default:
                            playStatus.text = countDownCounter.ToString();
                            statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[VeryLong]);
                            break;
                    }*/
            }
            else
            {
                playStatus.transform.localScale = statusScale;
                playStatus.color =  new Color(playStatus.color.r, playStatus.color.g, playStatus.color.b, 1f - statusScale.LerpScale);
                statusBoarder.effectColor = Color.Lerp(Color.black, playStatus.color, statusScale.LerpScale);
            }
            //}
        }

        gameMessages.text = gameMessageTexts[(int)gameMessage, language];
	}
}
