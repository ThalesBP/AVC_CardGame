using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : GameBase {

    [Space(5)]
    [Header("Language")]
    private int language;

    [Space(5)]
    [Header("Status")]
    public Status status = Status.begin;
    public Status gameStatus = Status.newTurn;

    [Space(5)]
    [Header("Texts")]
    private Text panel;

    private Text connect;
    private Text calibrate;
    private Text start;
    private Text stop;
    private Text playTime;
    private Text playTimeInput;
    private Text obs;
    private Text obsInput;
    private Text modeText;
    private Text nCardsText;
    private Text kStiffText;
    private Text cFrictionText;
    private Text help;
    private Text force;

    private Text gameMessages;

    [Space(5)]
    [Header("Map")]
    public MiniMap map;
    //[HideInInspector]
    public Material miniMaptMat; //  Materials for minimap

    [Space(5)]
    [Header("Game Info")]
    [Range(0.1f, 3.0f)]
    public float gameSpeed = 1f;
    public float gameTime;
    public float totalGameTime;

    [Space(5)]
    [Header("Robot Manager")]
    public Button connectButton;
    public Button calibrateButton, startButton, stopButton;

    [Space(5)]
    [Header("Game Manager")]
    public InputField playTimeField;
    public InputField obsField;
    public Dropdown gameMode;
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider nCardsSlider;
    public Slider kStiffSlider;
    public Slider cFrictionSlider;
    public Toggle helpToggle;
    public Toggle forceToggle;

    [Space(5)]
    [Header("Connection")]
    public Connection connection;
    public bool calibrating;
    private bool connected;

    [Space(5)]
    [Header("Others")]
    public HideShow visibility;
    public ResultsPanel results;


    void Start () 
    {
        visibility = GetComponent<HideShow>();

        panel = GameObject.Find("ControlTabName").GetComponentInChildren<Text>(true);

        connect = GameObject.Find("Connect").GetComponentInChildren<Text>(true);
        calibrate = GameObject.Find("Calibrate").GetComponentInChildren<Text>(true);
        start = GameObject.Find("Start").GetComponentInChildren<Text>(true);
        stop = GameObject.Find("Stop").GetComponentInChildren<Text>(true);
        playTime = GameObject.Find("PlayTimePlaceholder").GetComponentInChildren<Text>(true);
        playTimeInput = GameObject.Find("PlayTime").GetComponentInChildren<Text>(true);
        obs = GameObject.Find("ObsPlaceholder").GetComponentInChildren<Text>(true);
        obsInput = GameObject.Find("Obs").GetComponentInChildren<Text>(true);
        modeText = GameObject.Find("GameModeText").GetComponentInChildren<Text>(true);
        nCardsText = GameObject.Find("NCardsText").GetComponentInChildren<Text>(true);
        kStiffText = GameObject.Find("StiffnessText").GetComponentInChildren<Text>(true);
        cFrictionText = GameObject.Find("FrictionText").GetComponentInChildren<Text>(true);
        help = GameObject.Find("VisualHelp").GetComponentInChildren<Text>(true);
        help = GameObject.Find("ForceHelp").GetComponentInChildren<Text>(true);

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>(true);

        map = gameObject.AddComponent<MiniMap>();
        map.mat = miniMaptMat;
        map.space = GameObject.Find("MiniMap").GetComponentInChildren<RectTransform>(true);

        calibrateButton.onClick.AddListener(delegate { Calibrate(); });
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        stopButton.onClick.AddListener(delegate { FinishGame(); });
        connectButton.onClick.AddListener(delegate { Connect(); });

        connected = calibrating = false;
    }
	
	void Update () 
    {
        map.enabled = visibility.showed;

        language = (int)chosenLanguage;

        panel.text = controlPanelText[language];

        if (connection == null)
        {
            connected = true;
            connect.text = connectText[language];
        }
        else if (connection.connected)
        {
            connected = true;
            connect.text = disconnectText[language];
        }
        else
        {
            connected = false;
            connect.text = connectingText[language];
        }

        if (calibrating)
            calibrate.text = calibratingText[language];
        else
            calibrate.text = calibrateText[language];

        calibrateButton.interactable = false;

        stop.text = stopText[language];
        help.text = helpText[language];
        nCardsText.text = nCardsSlider.value.ToString("F0") + " " + cardsText[language];
        kStiffText.text = stiffnessText[language] + ": " + (MaxStiffness * kStiffSlider.value / kStiffSlider.maxValue).ToString("F1");
        cFrictionText.text = frictionText[language] + ": " + (MaxAntiFriction * cFrictionSlider.value / cFrictionSlider.maxValue).ToString("F1");
        ControlManager.Instance.stiffness = MaxStiffness * kStiffSlider.value / kStiffSlider.maxValue;
        ControlManager.Instance.antiFriction = MaxAntiFriction * cFrictionSlider.value / cFrictionSlider.maxValue;

        gameMessages.text = gameMessageTexts[(int)status, language] + "\n" + gameMessageTexts[(int)gameStatus, language];

        switch (status)
        {
            case Status.begin:
                start.text = startText[language];
                playTime.text = playTimeText[language];
                gameTime = totalGameTime = 0f;

                calibrateButton.interactable = connected;

                nCardsSlider.interactable = true;

                map.Reset();
                break;
            case Status.paused:
                start.text = startText[language];
                playTime.text = infTimeText[language];

                calibrateButton.interactable = connected;
                break;
            case Status.playing:
                start.text = pauseText[language];
                playTime.text = infTimeText[language];
                // To check
                if (Choice.orderCounter > 0)
                {
                    if (Time.timeScale != 0f)
                        Time.timeScale = gameSpeed;
                }

                // To check
                if (gameTime > 60f * totalGameTime)
                {
                    if (gameStatus != Status.playerChoice)
                        FinishGame();
                }
                break;
            case Status.end:
                start.text = restartText[language];
                playTime.text = infTimeText[language];
                nCardsSlider.interactable = false;
                break;
        }

        modeText.text = gameSetupText[language];
        for (int option = 0; option < numOfGameModes; option++)
        {
            gameMode.options[option].text = gameModeTexts[option, language];
        }
        gameMode.RefreshShownValue();

        if (gameMode.value == numOfGameModes - 1)
        {
    //        slider.value = 3f;
            nCardsSlider.interactable = false;
        }

        obs.text = obsText[language];
   //     gameMessages.text = gameMessageTexts[(int)gameMessage, language] + "\n" + gameMessageTexts[(int)dealerMessage, language];
	}

    #region Game Control
    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        status = Status.paused;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void StartGame()
    {
        if (playTimeInput.text == "")
        {
            totalGameTime = float.PositiveInfinity;;
        }
        else
        {
            totalGameTime = (float)int.Parse(playTimeInput.text);
        }

        playTimeField.interactable = false;
        stopButton.interactable = true;
        results.visibility.Hide();

        status = Status.playing;
        Time.timeScale = gameSpeed;
    }

    /// <summary>
    /// Finishes the game.
    /// </summary>
    public void FinishGame()
    {
        stopButton.interactable = false;
        results.visibility.Show();

        Time.timeScale = gameSpeed;
        status = Status.end;
    }

    /// <summary>
    /// Switchs between start and pause status.
    /// </summary>
    public void SwitchStartPause()
    {
        switch (status)
        {
            case Status.begin:
                StartGame();
                break;
            case Status.paused:
                StartGame();
                break;
            case Status.playing:
                PauseGame();
                break;
            case Status.end:
                status = Status.begin;
                playTimeField.interactable = true;
                results.visibility.Hide();
                Choice.ResetChoice();
                break;
        }
    }
    #endregion

    #region Robot Control
    private void Connect()
    {
        connection = gameObject.AddComponent<Connection> ();
        ControlManager.Instance.connection = connection;
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(delegate { Disconnect(); });
    }

    private void Disconnect()
    {
        if (connection != null) Destroy (connection);
        connectButton.onClick.RemoveAllListeners();
        connectButton.onClick.AddListener(delegate { Connect(); });
    }

    private void Calibrate()
    {
        if (calibrating)
        {
            calibrating = false;
            startButton.interactable = true;
        }
        else
        {
            calibrating = true;
            startButton.interactable = false;

            ControlManager.Instance.ankle.Reset();
        }
    }
    #endregion
}
