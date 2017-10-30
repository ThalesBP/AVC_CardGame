using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    [Space(5)]
    [Header("Language")]
    public int language;

    [Space(5)]
    [Header("Panels")]
    public ControlPanel control;
    public UserPanel user;

    [Space(5)]
    [Header("Log")]
    public Logger log;
  //  public Texture2D mouseDefault;
    public bool logging = false;
    private float readTime = 0f;

    [Space(5)]
    [Header("Texts")]
    public Text playStatus;         // Central Text Above
    public Text playStatus2;        // Central Text Below
    public GameObject mainButton;   // Central Button
    private Motion statusScale;     // Central Text Scale
    private Outline statusBoarder;  // Central Text's outlind

    private Text mainButtonText;
    private Text scorePoints;
    public Text timeCounter;
    // Transform in list later
    private Text metric1;
    private Text metric2;
    private Text metric3;

    [Space(5)]
    [Header("Counters")]
    [SerializeField]
    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}
    public int scoreValue;
    public int mode;

    public float metric1Value;
    public float metric2Value;
    public float metric3Value;

    [Space(5)]
    [Header("Challenges")]
    public ChallengeManager mainChallenge;
    public List<ChallengeManager> subChallenges;

    private AudioSource music;
    private Motion musicFade;

    void Start () 
    {
        countDownCounter = -1;
        mode = -1;
    //    Time.timeScale = 0f;

        #region General Interface Initialization
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>(true);
        playStatus2 = GameObject.Find("PlayStatus2").GetComponentInChildren<Text>(true);
        mainButtonText = GameObject.Find("MainButtonText").GetComponentInChildren<Text>(true);
        mainButton.SetActive(false);

        music = GetComponent<AudioSource>();
        musicFade = gameObject.AddComponent<Motion>();

        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>(true);
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>(true);

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>(true);
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>(true);
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>(true);

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

        language = (int)chosenLanguage;

        metric1.text = metric1Text[language] + "\n" + metric1Value.ToString("F0") + "%";
        metric2.text = metric2Text[language] + "\n" + metric2Value.ToString("F0") + "%";
        metric3.text = metric3Text[language] + "\n" + metric3Value.ToString("F0") + "%";
		
        #region Checks current game status
        switch (control.status)
        {
            case Status.begin:
                music.Play();
                music.Pause();
                if (control.calibrating)
                    playStatus.text = moveCirclesText[language];
                else
                {
                    playStatus.text = readyText[language];
                    mainChallenge.Reset();
                    foreach (ChallengeManager challenge in subChallenges)
                        challenge.Reset();
                }
                playStatus2.text = "";
   //             StartCountDown(CountDown);
                break;
            case Status.paused:
                music.Pause();

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
                music.UnPause();
                music.volume = control.musicSlider.value;

                if (!logging)
                {
                    user.visibility.MoveTo(-10f);
                    user.visibility.locked = true;
                    StartLoggin();
                }
                else
                {
                    log.Register(control.gameTime, gameMessageTexts[(int)control.status, language] + " - " + gameMessageTexts[(int)control.gameStatus, language]);
                }
                
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
                    if (control.connection != null)
                    {
                        if (control.connection.ReadTime != readTime)
                        {
                            log.Register(control.gameTime, ControlManager.Instance.RawPosition, control.connection.Force);
                            readTime = control.connection.ReadTime;
                        }
                    }
                    else
                        log.Register(control.gameTime, ControlManager.Instance.RawPosition, Vector2.zero);

                    if (control.connection != null)
                    {
                        if (control.connection.DeltaTimeRead > Time.unscaledDeltaTime)
                        {
                            Debug.Log("Big Time Step Reading: " + control.connection.DeltaTimeRead.ToString());
                            log.Register(control.gameTime, "Big Time Step Reading" + control.connection.DeltaTimeRead.ToString());
                        }

                        if (control.connection.DeltaTimeSend > Time.unscaledDeltaTime)
                        {
                            Debug.Log("Big Time Step Sending: " + control.connection.DeltaTimeSend.ToString());
                            log.Register(control.gameTime, "Big Time Step Sending" + control.connection.DeltaTimeSend.ToString());
                        }
                    }

                }
                playStatus.transform.localScale = statusScale;

                if (mode >= 0)
                {
                    if (!mainButton.gameObject.activeSelf)
                    {
                        statusScale.MoveTo(highlightScale * Vector3.one);
                        statusScale.MoveTo(Vector3.one, DeltaTime[Short]);
                    
                        if (ControlManager.Instance.Position.y < Screen.height * 0.40f)
                        if (LimbSide((Limbs)InterfaceManager.Instance.user.memberDropdown.value) == Side.right)
                            mainButton.transform.localPosition = Vector3.left * Screen.height * 0.20f;
                        else
                            mainButton.transform.localPosition = Vector3.right * Screen.height * 0.20f;
                        else
                            mainButton.transform.localPosition = Vector3.down * Screen.height * 0.20f;
                    }
                    playStatus.color = SetAlpha(YellowText, statusScale.LerpScale);
                    statusBoarder.effectColor = Color.Lerp(Color.black, playStatus.color, (1 - statusScale.LerpScale + 0.2f));

                    playStatus.text = explainingModesText[mode, language];
                    playStatus2.text = "";// ready2Text[language];

                    mainButton.SetActive(true);
                }
                else
                {
                    if (mainButton.gameObject.activeSelf)
                    {
                        statusScale.MoveTo(Vector3.one);
                        statusScale.MoveTo(highlightScale * Vector3.one, DeltaTime[Short]);
                    }
                    playStatus.color = SetAlpha(YellowText, 1f - statusScale.LerpScale);
                    statusBoarder.effectColor = Color.Lerp(Color.black, playStatus.color, statusScale.LerpScale + 0.2f);
//                    playStatus.text = "";
  //                  playStatus2.text = "";
                    mainButton.SetActive(false);
                }

                break;
            case Status.end:
                if (logging)
                {
                    musicFade.timeScaled = false;
                    musicFade.Fade(DeltaTime[MuchLonger]);
                    StopLoggin();
                }
                user.visibility.locked = false;
                music.volume = (music.volume = control.musicSlider.value - musicFade.LerpScale);
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
            metric1.color = SetAlpha(YellowText, control.visibility.slideTimeLerp);
            metric2.color = SetAlpha(YellowText, control.visibility.slideTimeLerp);
            metric3.color = SetAlpha(YellowText, control.visibility.slideTimeLerp);

        }
        else
        {
            timeCounter.color = SetAlpha(YellowText, 1f - control.visibility.slideTimeLerp);
            metric1.color = SetAlpha(YellowText, 1f - control.visibility.slideTimeLerp);
            metric2.color = SetAlpha(YellowText, 1f - control.visibility.slideTimeLerp);
            metric3.color = SetAlpha(YellowText, 1f - control.visibility.slideTimeLerp);
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
        log.StartFiles(user.managerDropdown.captionText.text, user.playerDropdown.captionText.text, user.memberDropdown.captionText.text, user.memberDropdown.value, control.obsField.text); 
        logging = true;
    }

    public void StopLoggin()
    {
        log.Register(user.memberDropdown.captionText.text, control.gameTime, mainChallenge);
        logging = false;
    }
}
