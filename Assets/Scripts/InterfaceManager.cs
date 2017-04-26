using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public int language;

    public ControlPanel control;

    #region Interface's texts
    public Text playStatus;
    private Motion statusScale;
    private Outline statusBoarder;

    private Text scorePoints;
    private Text timeCounter;
    #endregion

    #region GameVariables
    // Delete
 /*   [Range(0.1f, 3.0f)]
    public float gameSpeed = 1f;*/

    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}

  /*  private float gameTime;         // Current game time
    private float totalGameTime;    // Time to end the game
*/
    // Delete
 /*   public float metric1Value;
    public float metric2Value;
    public float metric3Value;*/

    public int scoreValue;

    // Delete
/*    public Messages gameMessage = Messages.newTurn;
    public Messages dealerMessage = Messages.newTurn; // rename to generic*/
    #endregion

    #region Interactive objects
    // Delete
 /*   public HideShow controlPanelVisibility;
    public Button connectButton, startButton, stopButton;
    public InputField playTimeField;
    public Toggle helpToggle;
    public Slider numOfCardsSlider;*/

    public Texture2D mouseDefault;
    #endregion
    public 

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

        // Delete
/*        #region Control Panel Initialization
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
        #endregion*/

        // Delete
    /*    #region Control Panel Interactive
        connectButton.interactable = false;
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        stopButton.onClick.AddListener(delegate { FinishGame(); });
        helpToggle.interactable = false;
        #endregion*/
        }

	void Update ()
    {
        language = (int)chosenLanguage;
		
        #region Checks current game status
        switch (control.status)
        {
            case Status.begin:
                playStatus.text = readyText[language];
                StartCountDown(CountDown);
                break;
            case Status.paused:
                playStatus.text = pausedText[language];
                statusScale.MoveTo(Vector3.one);
                playStatus.color = YellowText;
                statusBoarder.effectColor = Color.black;
 //               countDownCounter = -1;
                StartCountDown(CountDown);
                break;
  /*          case Status.counting:
                statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[VeryLong]);
                StartCountDown(CountDown);
                control.status = Status.playing;
                break;*/
            case Status.playing:
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
                else
                {
                    control.gameTime += Time.unscaledDeltaTime;
                }
                playStatus.transform.localScale = statusScale;
                break;
            case Status.end:
 //               countDownCounter = -1;

                playStatus.text = endOfGameText[language];
                statusScale.MoveTo(Vector3.one);
                playStatus.color = YellowText;
                statusBoarder.effectColor = Color.black;

                playStatus.text = endOfGameText[language];
                break;
        }
        #endregion

        scorePoints.text = scorePointsText[language] + "\n" + scoreValue.ToString("F0"); 

        #region Shows or hides the timer with panel
        if (control.visibility.showed)
        {
            timeCounter.color = SetAlpha(YellowText, control.visibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + control.gameTime.ToString("F1");
        }
        else
        {
            timeCounter.color = SetAlpha(YellowText, 1f - control.visibility.slideTimeLerp);
            timeCounter.text = timeText[language] + "\n" + control.gameTime.ToString("F1");
        }
        #endregion

        // Delete
    /*    #region Control Panel Text Update
        panel.text = controlPanelText[language];

        metric1.text = metric1Text[language] + "\n" + metric1Value.ToString("F0") + "%";
        metric2.text = metric2Text[language] + "\n" + metric2Value.ToString("F0") + "%";
        metric3.text = metric3Text[language] + "\n" + metric3Value.ToString("F0") + "%";

        connect.text = connectText[language];
        stop.text = stopText[language];
        help.text = helpText[language];
        numOfCards.text = numOfCardsSlider.value.ToString("F0") + " " + cardsText[language];
        #endregion*/
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

    // Delete
    /// <summary>
    /// Pauses the game.
    /// </summary>
/*    private void PauseGame()
    {
        currentStatus = Status.paused;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    private void StartGame()
    {
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
        results.visibility.Hide();

        currentStatus = Status.counting;
        Time.timeScale = gameSpeed;
    }

    /// <summary>
    /// Switchs between start and pause status.
    /// </summary>
    private void SwitchStartPause()
    {
        if (currentStatus != Status.end)
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
            currentStatus = Status.begin;
            results.visibility.Hide();
        }
    }

    /// <summary>
    /// Finishes the game.
    /// </summary>
    public void FinishGame()
    {
        results.visibility.Show();

        stopButton.interactable = false;

        currentStatus = Status.end;
    }*/
    #endregion
}
