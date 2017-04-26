using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public enum GameStatus {begin, playing, paused, end};
    public GameStatus currentStatus = GameStatus.begin;
    public int language;

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

    private Text panel;

    private Text connect;
    private Text start;
    private Text stop;
    private Text playTime;
    private Text playTimeInput;
    private Text help;
    private Text numOfCards;
    #endregion

    #region GameVariables
    [Range(0.1f, 3.0f)]
    public float gameSpeed = 1f;

    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}

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
    public Messages gameMessage = Messages.newTurn;
    public Messages dealerMessage = Messages.newTurn;
    #endregion

    #region Interactive objects
        #region Control interface
        public HideShow controlPanelVisibility, resultsVisibility;
        public Button connectButton, startButton, stopButton;
        public InputField playTimeField;
        public Toggle helpToggle;
        public Slider numOfCardsSlider;
        public Texture2D mouseDefault;
        #endregion
    #endregion
    public 
	// Use this for initialization
	void Start () 
    {
        Cursor.SetCursor(mouseDefault, Vector2.zero, CursorMode.ForceSoftware);
        countDownCounter = -1;
        Time.timeScale = 0f;

        #region General Interface Initialization
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>(true);
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>(true);
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>(true);
        #endregion

        #region Results Panel Initialization
        hitRate = GameObject.Find("HitRate").GetComponentsInChildren<Text>();
        timeRate = GameObject.Find("TimeRate").GetComponentsInChildren<Text>();
        info1 = GameObject.Find("Info1").GetComponentsInChildren<Text>();
        info2 = GameObject.Find("Info2").GetComponentsInChildren<Text>();
        #endregion

        #region Control Panel Initialization
        panel = GameObject.Find("ControlTabName").GetComponentInChildren<Text>(true);

        connect = GameObject.Find("Connect").GetComponentInChildren<Text>(true);
        start = GameObject.Find("Start").GetComponentInChildren<Text>(true);
        stop = GameObject.Find("Stop").GetComponentInChildren<Text>(true);
        playTime = GameObject.Find("PlayTimePlaceholder").GetComponentInChildren<Text>(true);
        playTimeInput = GameObject.Find("PlayTime").GetComponentInChildren<Text>(true);
        help = GameObject.Find("VisualHelp").GetComponentInChildren<Text>(true);
        numOfCards = GameObject.Find("NumOfCards").GetComponentInChildren<Text>(true);

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>(true);
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>(true);
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>(true);

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>(true);
        #endregion

        #region Control Panel Interactive
        connectButton.interactable = false;
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        stopButton.onClick.AddListener(delegate { FinishGame(); });
        helpToggle.interactable = false;
        #endregion
        }
	// Update is called once per frame
	void Update ()
    {
        language = (int)chosenLanguage;
		
        #region Counts the time down
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
        #endregion

        #region Checks current game status
        switch (currentStatus)
        {
            case GameStatus.begin:
                gameTime = totalGameTime = 0f;
                gameMessage = Messages.waitingStart;
                playStatus.text = readyText[language];
                start.text = startText[language];
                break;
            case GameStatus.paused:
                gameMessage = Messages.gamePaused;
                start.text = startText[language];
                break;
            case GameStatus.playing:
                gameMessage = Messages.gameRunning;

                if (countDownCounter < 0)
                    gameTime += Time.unscaledDeltaTime;

                if (Choice.orderCounter > 0)
                {
                    if (Time.timeScale != 0f)
                        Time.timeScale = gameSpeed;
                }

                if (totalGameTime > 0f)
                    if (gameTime > 60f * totalGameTime)
                    {
                        FinishGame();
                    }
                start.text = pauseText[language];
                break;
            case GameStatus.end:
                gameMessage = Messages.showingResults;
                gameMessages.text = gameMessageTexts[(int)gameMessage, language];
                playStatus.text = endOfGameText[language];
                start.text = restartText[language];
                break;
        }

        gameMessages.text = gameMessageTexts[(int)gameMessage, language] + "\n" + gameMessageTexts[(int)dealerMessage, language];
        #endregion

        #region Shows or hides the timer with panel
        if (controlPanelVisibility.showed)
        {
            timeCounter.color = SetAlpha(YellowText, controlPanelVisibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + gameTime.ToString("F1");
        }
        else
        {
            timeCounter.color = SetAlpha(YellowText, 1f - controlPanelVisibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + gameTime.ToString("F1");
        }

        if (resultsVisibility.showed)
        {
            hitRate[0].text = hitRateText[language];
            timeRate[0].text = timeRateText[language];
            info1[0].text = info1Text[language];
            info2[0].text = info2Text[language];

            hitRate[1].text = (100f * Choice.totalMatches / Choice.orderCounter).ToString("F0") + "%";
            timeRate[1].text = Choice.averageTimeToChoose.ToString("F1");
            info1[1].text = "0";
            info2[1].text = "0";

            hitRate[2].text = Choice.totalMatches.ToString("F0") + ofText[language] + Choice.orderCounter.ToString("F0");
            timeRate[2].text = fromText[language] + Choice.rangeOfTime[0].ToString("F1") + toText[language] + Choice.rangeOfTime[1].ToString("F1");
            info1[2].text = fromText[language] + "0 to 0";
            info2[2].text = fromText[language] +"0 to 0";
        }
        #endregion

        #region Control Panel Text Update
        panel.text = controlPanelText[language];
        scorePoints.text = scorePointsText[language] + "\n" + scoreValue.ToString("F0"); 

        metric1.text = metric1Text[language] + "\n" + metric1Value.ToString("F0") + "%";
        metric2.text = metric2Text[language] + "\n" + metric2Value.ToString("F0") + "%";
        metric3.text = metric3Text[language] + "\n" + metric3Value.ToString("F0") + "%";

        connect.text = connectText[language];
        stop.text = stopText[language];
        help.text = helpText[language];
        numOfCards.text = numOfCardsSlider.value.ToString("F0") + " " + cardsText[language];
        #endregion
	}

    #region Game status manager functions
    /// <summary>
    /// Starts count down with 'time' seconds.
    /// </summary>
    /// <param name="time">Time in seconds.</param>
    public void StartCountDown(int time)
    {
        countDownCounter = time + 1;
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGame()
    {
        playStatus.text = pausedText[language];

        statusScale.MoveTo(Vector3.one);
        playStatus.color = YellowText;
        statusBoarder.effectColor = Color.black;

        currentStatus = GameStatus.paused;
        Time.timeScale = 0f;
        countDownCounter = -1;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    private void StartGame()
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

        stopButton.interactable = true;
        resultsVisibility.Hide();

        currentStatus = GameStatus.playing;
        Time.timeScale = gameSpeed;
    }

    /// <summary>
    /// Switchs between start and pause status.
    /// </summary>
    private void SwitchStartPause()
    {
        if (currentStatus != GameStatus.end)
        {
            if (Time.timeScale == gameSpeed)
            {
                PauseGame();
            }
            else
            {
                StartGame();
            }
        }
        else
        {
            currentStatus = GameStatus.begin;
            resultsVisibility.Hide();
        }
    }

    /// <summary>
    /// Finishes the game.
    /// </summary>
    public void FinishGame()
    {
        resultsVisibility.Show();
        countDownCounter = -1;

        playStatus.text = endOfGameText[language];
        statusScale.MoveTo(Vector3.one);
        playStatus.color = YellowText;
        statusBoarder.effectColor = Color.black;

        stopButton.interactable = false;

        currentStatus = GameStatus.end;
    }
    #endregion
}
