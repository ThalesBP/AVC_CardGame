using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    private enum Status {begin, playing, paused};
    private Status currentStatus = Status.begin;
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
    private Text timeCounter;

    private Text metric1;
    private Text metric2;
    private Text metric3;

    private Text gameMessages;

    private Text panelName;

    private Text connect;
    private Text start;
    private Text stop;
    private Text playTime;
    private Text playTimeInput;
    private Text help;
    private Text numOfCards;
    #endregion

    #region GameVariables
    private int countDownCounter;   // Counter for count down
    private float gameTime;         // Current game time
    private float totalGameTime;    // Time to end the game

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

    #region Interactive objects
    public HideShow panelVisibility;
    public Button connectButton, startButton, stopButton;
    public InputField playTimeField;
    public Toggle helpToggle;
    public Slider numOfCardsSlider;
    public Texture2D mouseDefault;
    #endregion

	// Use this for initialization
	void Start () 
    {
        
        Cursor.SetCursor(mouseDefault, Vector2.zero, CursorMode.ForceSoftware);
        countDownCounter = -1;

        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>();
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusBoarder = playStatus.GetComponent<Outline>();

        hitRate = GameObject.Find("HitRate").GetComponentsInChildren<Text>();
        timeRate = GameObject.Find("TimeRate").GetComponentsInChildren<Text>();
        info1 = GameObject.Find("Info1").GetComponentsInChildren<Text>();
        info2 = GameObject.Find("Info2").GetComponentsInChildren<Text>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>();
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>();

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>();
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>();
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>();

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>();

        panelName = GameObject.Find("PanelName").GetComponentInChildren<Text>();

        connect = GameObject.Find("Connect").GetComponentInChildren<Text>();
        start = GameObject.Find("Start").GetComponentInChildren<Text>();
        stop = GameObject.Find("Stop").GetComponentInChildren<Text>();
        playTime = GameObject.Find("PlayTimePlaceholder").GetComponentInChildren<Text>();
        playTimeInput = GameObject.Find("PlayTime").GetComponentInChildren<Text>();
        help = GameObject.Find("VisualHelp").GetComponentInChildren<Text>();
        numOfCards = GameObject.Find("NumOfCards").GetComponentInChildren<Text>();

        Time.timeScale = 0f;
        connectButton.interactable = false;
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        }
	// Update is called once per frame
	void Update ()
    {
        language = (int)chosenLanguage;
		
        if (countDownCounter >= 0)
        {
            if (statusScale.status == Motion.Status.idle)
            {
                countDownCounter--;
                statusScale.MoveTo(Vector3.one);
                switch (countDownCounter)
                {
                    case -1:
                        playStatus.text = "";
                        break;
                    case 0:
                        playStatus.text = goText[language];
                        break;
                    default:
                        playStatus.text = countDownCounter.ToString();
                        break;
                }
                statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[MuchLonger]);
            }
            else
            {
                playStatus.color = SetAlpha(YellowText, 1f - statusScale.LerpScale);
                statusBoarder.effectColor = Color.Lerp(Color.black, playStatus.color, statusScale.LerpScale + 0.2f);
            }
        }
        playStatus.transform.localScale = statusScale;

        panelName.text = panelText[language];

        switch (currentStatus)
        {
            case Status.begin:
                playStatus.text = readyText[language];
                gameMessages.text = gameMessageTexts[(int)gameMessage, language];
                start.text = startText[language];
                break;
            case Status.paused:
                gameMessages.text = pausedText[language];
                start.text = startText[language];
                break;
            case Status.playing:
                gameMessages.text = gameMessageTexts[(int)gameMessage, language];

                if (countDownCounter < 0)
                    gameTime += Time.deltaTime;

                if (totalGameTime > 0f)
                    if (gameTime > 60f * totalGameTime)
                    {
                        FinishGame();
                    }
                start.text = pauseText[language];
                break;
        }

        scorePoints.text = scorePointsText[language] + "\n" + scoreValue.ToString("F0"); 

        if (panelVisibility.showed)
        {
            timeCounter.color = SetAlpha(YellowText, panelVisibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + gameTime.ToString("F1");
        }
        else
        {
            timeCounter.color = SetAlpha(YellowText, 1f - panelVisibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + gameTime.ToString("F1");
        }
            

        metric1.text = metric1Text[language] + "\n" + metric1Value.ToString("F0") + "%";
        metric2.text = metric2Text[language] + "\n" + metric2Value.ToString("F0") + "%";
        metric3.text = metric3Text[language] + "\n" + metric3Value.ToString("F0") + "%";

        connect.text = connectText[language];
        stop.text = stopText[language];
        help.text = helpText[language];
        numOfCards.text = numOfCardsText[language];
	}

    /// <summary>
    /// Starts count down with 'time' seconds.
    /// </summary>
    /// <param name="time">Time in seconds.</param>
    public void StartCountDown(int time)
    {
        countDownCounter = time + 1;
    }

    /// <summary>
    /// Switchs between start and pause status.
    /// </summary>
    private void SwitchStartPause()
    {
        if (Time.timeScale == 1f)
        {
            playStatus.text = pausedText[language];

            statusScale.MoveTo(Vector3.one);
            playStatus.color = YellowText;
            statusBoarder.effectColor = Color.black;

            currentStatus = Status.paused;
            Time.timeScale = 0f;
        }
        else 
        {
            statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[VeryLong]);
            StartCountDown(CountDown);

            playTimeField.interactable = false;
            if (playTimeInput.text == "")
            {
                totalGameTime = 0f;
                playTime.text = infTimeText[language];
            }
            else
            {
                totalGameTime = (float)int.Parse(playTimeInput.text);
                playTime.text = "";
            }

            currentStatus = Status.playing;
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Finishes the game.
    /// </summary>
    public static void FinishGame()
    {
        Debug.Log("Finished");
    }
}
