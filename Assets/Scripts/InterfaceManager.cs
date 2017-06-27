using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public int language;

    public ControlPanel control;
    public UserPanel user;
    public Logger log;
    public Texture2D mouseDefault;

    public Text playStatus;
    private Motion statusScale;
    private Outline statusBoarder;

    private Text scorePoints;
    private Text timeCounter;

    [SerializeField]
    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}
    public int scoreValue;

    UnityEngine.Events.UnityAction StartLogAction;
    UnityEngine.Events.UnityAction RestartAction;

    void Start () 
    {
        countDownCounter = CountDown;
        Time.timeScale = 0f;

        #region General Interface Initialization
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>(true);
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>(true);
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>(true);

        log = gameObject.AddComponent<Logger>();

        StartLogAction = delegate {StartLog();};
        RestartAction = delegate {Restart();};

        control.startButton.onClick.AddListener(StartLogAction);
        control.stopButton.onClick.AddListener(delegate { StopLog(); });
        #endregion
    }

    void Update ()
    {
        if (user.locked)
            control.startButton.interactable = true;
        else
            control.startButton.interactable = false;

        if ((control.connection == null) && (!ControlManager.Instance.joystick))
            Cursor.visible = false;
        else
            Cursor.visible = true;
//        else
//            Cursor.SetCursor(mouseDefault, Vector2.zero, CursorMode.ForceSoftware);

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
                StartCountDown(CountDown);
                break;
            case Status.playing:
                if (countDownCounter >= 0)
                {
                    if (statusScale.Idle)
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
                    log.Register(control.gameTime, ControlManager.Instance.Position);
                    log.Register(control.gameTime, gameMessageTexts[(int)control.status, language] + " - " + gameMessageTexts[(int)control.gameStatus, language]);
                }
                playStatus.transform.localScale = statusScale;

                break;
            case Status.end:
                playStatus.text = endOfGameText[language];
                statusScale.MoveTo(Vector3.one);
                playStatus.color = YellowText;
                statusBoarder.effectColor = Color.black;
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
	}

    /// <summary>
    /// Starts count down with 'time' seconds.
    /// </summary>
    /// <param name="time">Time in seconds.</param>
    public void StartCountDown(int time)
    {
        countDownCounter = time + 1;
    }

    private void StartLog()
    {
        log.StartFiles(user.managerDropdown.captionText.text, user.playerDropdown.captionText.text, user.memberDropdown.captionText.text); 
        control.startButton.onClick.RemoveListener(StartLogAction);
        user.visibility.MoveTo(-10f);
        user.visibility.locked = true;
    }

    private void StopLog()
    {
        user.visibility.locked = false;
        control.startButton.onClick.AddListener(RestartAction);
    }

    private void Restart()
    {
        user.visibility.locked = false;
        control.startButton.onClick.RemoveListener(RestartAction);
        control.startButton.onClick.AddListener(StartLogAction);
    }
}
