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

    public Text playStatus, playStatus2;
    private Motion statusScale;
    private Outline statusBoarder;

    private Text scorePoints;
    private Text timeCounter;

    [SerializeField]
    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}
    public int scoreValue;
    public int mode;

    public bool logging = false;

    public ChallengeManager mainChallenge;
    public List<ChallengeManager> subChallenges;

    void Start () 
    {
        countDownCounter = -1;
        mode = -1;
        Time.timeScale = 0f;

        #region General Interface Initialization
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>(true);
        playStatus2 = GameObject.Find("PlayStatus2").GetComponentInChildren<Text>(true);
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>(true);
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>(true);

        log = gameObject.AddComponent<Logger>();

        user.playerButton.onClick.AddListener(delegate { mainChallenge.Plan = user.Plan; });

        mainChallenge = new ChallengeManager(new float[] { 0.2f, 0.5f, 0.2f, 0.1f });

        subChallenges = new List<ChallengeManager>();
        for (int i = 0; i < mainChallenge.Size; i++)
        {
            subChallenges.Add(new ChallengeManager(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f }));
        }
        #endregion
    }

    void Update ()
    {
        if (user.locked)
        {
            if (control.gameStatus == Status.waitingMotion)
                control.startButton.interactable = false;
            else
                control.startButton.interactable = true;
        }
        else
            control.startButton.interactable = false;

        if ((control.connection == null) && (!ControlManager.Instance.joystick))
            Cursor.visible = false;
        else
            Cursor.visible = true;

        language = (int)chosenLanguage;
		
        #region Checks current game status
        switch (control.status)
        {
            case Status.begin:
                if (control.calibrating)
                    playStatus.text = moveCirclesText[language];
                else
                    playStatus.text = readyText[language];
                playStatus2.text = "";
   //             StartCountDown(CountDown);
                break;
            case Status.paused:
                if (control.calibrating)
                    playStatus.text = moveCirclesText[language];
                else
                    playStatus.text = pausedText[language];
                statusScale.MoveTo(Vector3.one);
                playStatus.color = YellowText;
                statusBoarder.effectColor = Color.black;
   //             StartCountDown(CountDown);
                break;
            case Status.playing:

                if (!logging)
                {
                    user.visibility.MoveTo(-10f);
                    user.visibility.locked = true;
                    StartLoggin();
                }
                else
                    log.Register(control.gameTime, gameMessageTexts[(int)control.status, language] + " - " + gameMessageTexts[(int)control.gameStatus, language]);
                
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
                    log.Register(control.gameTime, ControlManager.Instance.RawPosition);
                }
                playStatus.transform.localScale = statusScale;

                if (mode >= 0)
                {
                    playStatus.text = explainingModesText[mode, language];
                    playStatus2.text = ready2Text[language];
                }
                else
                {
                    playStatus.text = "";
                    playStatus2.text = "";
                }

                break;
            case Status.end:
                if (logging)
                {
                    user.visibility.locked = false;
                    StopLoggin();
                }
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

    public void StartLoggin()
    {
        log.StartFiles(user.managerDropdown.captionText.text, user.playerDropdown.captionText.text, user.memberDropdown.captionText.text, user.memberDropdown.value); 
        logging = true;
    }

    public void StopLoggin()
    {
        log.Register(user.memberDropdown.captionText.text, control.gameTime, mainChallenge);
        logging = false;
    }
}
