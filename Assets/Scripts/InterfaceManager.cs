using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public enum GameStatus {begin, playing, paused, end};
    public enum UserStatus {unlocked, locked, creating, editing}
    public GameStatus currentStatus = GameStatus.begin;
    public UserStatus managerStatus = UserStatus.unlocked;
    public UserStatus playerStatus = UserStatus.unlocked;
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

    private Text userTab;
    private Text managerLogin;
    private Text managerPassword;
    private Text login;
    private Text playerSelect;
    private Text choose;
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
        public HideShow controlPanelVisibility, managerPanelVisibility, resultsVisibility;
        public Button connectButton, startButton, stopButton;
        public InputField playTimeField;
        public Toggle helpToggle;
        public Slider numOfCardsSlider;
        public Texture2D mouseDefault;
        #endregion

        #region User interface
        private bool logged, newManager, editManager, playerChosen, newPlayer, editPlayer;
        private UserManager users;
        public Dropdown managerDropdown, playerDropdown, memberDropdown;
        public InputField managerField, playerField, passwordField;
        public Button managerButton, managerEditButton, playerButton, playerEditButton;
        public RawImage managerEditImage, playerEditImage;
        public Texture editTexture, deleteTexture;
        public Text playerDescription;
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
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>();
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>();
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>();
        #endregion

        #region Results Panel Initialization
        hitRate = GameObject.Find("HitRate").GetComponentsInChildren<Text>();
        timeRate = GameObject.Find("TimeRate").GetComponentsInChildren<Text>();
        info1 = GameObject.Find("Info1").GetComponentsInChildren<Text>();
        info2 = GameObject.Find("Info2").GetComponentsInChildren<Text>();
        #endregion

        #region Control Panel Initialization
        panel = GameObject.Find("ControlTabName").GetComponentInChildren<Text>();

        connect = GameObject.Find("Connect").GetComponentInChildren<Text>();
        start = GameObject.Find("Start").GetComponentInChildren<Text>();
        stop = GameObject.Find("Stop").GetComponentInChildren<Text>();
        playTime = GameObject.Find("PlayTimePlaceholder").GetComponentInChildren<Text>();
        playTimeInput = GameObject.Find("PlayTime").GetComponentInChildren<Text>();
        help = GameObject.Find("VisualHelp").GetComponentInChildren<Text>();
        numOfCards = GameObject.Find("NumOfCards").GetComponentInChildren<Text>();

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>();
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>();
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>();

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>();
        #endregion

        #region User Panel Initialization
        userTab = GameObject.Find("UserTabName").GetComponentInChildren<Text>();
        managerLogin = GameObject.Find("ManagerDescription").GetComponentInChildren<Text>();
        managerPassword = GameObject.Find("ManagerPasswordPlaceholder").GetComponentInChildren<Text>();
        login = GameObject.Find("Login").GetComponentInChildren<Text>();
        playerSelect = GameObject.Find("PlayerDescription").GetComponentInChildren<Text>();
        choose = GameObject.Find("Choose").GetComponentInChildren<Text>();
        #endregion

        #region Control Panel Interactive
        connectButton.interactable = false;
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        stopButton.onClick.AddListener(delegate { FinishGame(); });
        helpToggle.interactable = false;
        #endregion

        #region User Panel Interactive
        logged = newManager = editManager = playerChosen = newPlayer = editPlayer = false;
        users = gameObject.AddComponent<UserManager>();
        UpdateUsers(managerDropdown, users.Managers);
        UpdateUsers(playerDropdown, users.players);

        managerButton.onClick.AddListener(delegate { ManagerAction(); });
        managerEditButton.onClick.AddListener(delegate { EditManager(); });
        playerButton.onClick.AddListener(delegate { PlayerAction(); });
        playerEditButton.onClick.AddListener(delegate { EditPlayer(); });
        playerDropdown.onValueChanged.AddListener( delegate { UpdateDescription();});
        #endregion
        }
	// Update is called once per frame
	void Update ()
    {
        language = (int)chosenLanguage;
		
        // Counts the time down
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

        // Checks current game status
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
                //Time.timeScale = 0f;
                break;
        }
        gameMessages.text = gameMessageTexts[(int)gameMessage, language] + "\n" + gameMessageTexts[(int)dealerMessage, language];

        // Shows or hides the timer with panel
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

        #region User Panel Text Update
        switch (managerStatus)
        {
            case UserStatus.unlocked:
                UpdateManagerActivity(true, newManager, newManager, true);
                UpdatePlayerActivity(false, false, false, true);
                if (newManager)
                    login.text = addText[language];
                else
                    login.text = loginText[language];
                break;
            case UserStatus.creating:
                UpdateManagerActivity(true, true, true, false);
                break;
            case UserStatus.editing:
                UpdateManagerActivity(true, true, true, false);
                break;
            case UserStatus.locked:
                UpdateManagerActivity(false, false, false, true);
                playerStatus = UserStatus.unlocked;
                playerDropdown.options[users.AmountPlayers].text = newUserText[language];
                login.text = logoutText[language];
                break;
        }

        switch (playerStatus)
        {
            case UserStatus.unlocked:
                UpdatePlayerActivity(true, true, newPlayer, true);

                break;
            case UserStatus.creating:
                UpdatePlayerActivity(true, true, true, false);
                break;
            case UserStatus.editing:
                UpdatePlayerActivity(true, true, true, false);
                break;
            case UserStatus.locked:
                UpdatePlayerActivity(false, true, false, true);
                break;
        }

        newManager = (managerDropdown.captionText.text == newUserText[language]);
        newPlayer = (playerDropdown.captionText.text == newUserText[language]);

        userTab.text = userPanelText[language];
        managerLogin.text = managerLoginText[language];
        managerPassword.text = enterPasswordText[language];

        managerDropdown.options[users.AmountManagers].text = newUserText[language];
   /*     if (logged)
        {
            playerDropdown.options[users.AmountPlayers].text = newUserText[language];
            login.text = logoutText[language];
        }
        else
        {
            if (newManager)
                login.text = addText[language];
            else
                login.text = loginText[language];
        }*/

        playerSelect.text = playerSelectText[language];
        if (playerChosen)
        {
            choose.text = changeText[language];
        }
        else
        {
            if (newPlayer)
                choose.text = addText[language];
            else
                choose.text = chooseText[language];
        }

        for (int option = 0; option < memberDropdown.options.Count; option++)
        {
            memberDropdown.options[option].text = limbTexts[option + 2, language];
        }
        memberDropdown.RefreshShownValue();
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

    #region Status update functions
    /// <summary>
    /// Updates a specified dropdown.
    /// </summary>
    /// <param name="dropdown">Dropdown to be updated.</param>
    /// <param name="users">Users to be uploaded.</param>
    void UpdateUsers(Dropdown dropdown, List<string> users)
    {
        List<string> users_aux = new List<string>();
        foreach (string user in users)
            users_aux.Add(user);
        users_aux.Add(newUserText[language]);

        dropdown.ClearOptions();
        dropdown.AddOptions(users_aux);
        users_aux.Clear();
    }

    /// <summary>
    /// Updates the description space based on the player.
    /// </summary>
    void UpdateDescription()
    {
        if (playerDropdown.value == users.AmountManagers)
            playerDescription.text = "";
        else
            playerDescription.text = users.Description[playerDropdown.value]; 
    }

    /// <summary>
    /// Updates the manager activity.
    /// </summary>
    void UpdateManagerActivity(bool dropdown, bool edit, bool password, bool selectMode)
    {
        managerDropdown.interactable = dropdown;
        managerEditButton.interactable = edit;
        passwordField.interactable = password;
        managerDropdown.gameObject.SetActive(selectMode);
        managerField.gameObject.SetActive(!selectMode);
    }

    /// <summary>
    /// Updates the player activity.
    /// </summary>
    void UpdatePlayerActivity(bool dropdown, bool button, bool edit, bool selectMode)
    {
        playerDropdown.interactable = dropdown;
        playerButton.interactable = button;
        playerEditButton.interactable = edit;
        memberDropdown.interactable = button;
        playerDropdown.gameObject.SetActive(selectMode);
        playerField.gameObject.SetActive(!selectMode);
    }

    /// <summary>
    /// Executes the manager button action.
    /// </summary>
    void ManagerAction()
    {
        if (newManager)
        {
            managerDropdown.gameObject.SetActive(false);
            managerField.gameObject.SetActive(true);
        }
        else
        {
            if (logged)
            {
                passwordField.text = "";
                logged = false;
            }
            else
            {
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    UpdateUsers(playerDropdown, users.players);
                    UpdateDescription();
                    logged = true;
                }
                else
                {
                    Debug.Log("Password Error");
                }
            }
        }
    }

    /// <summary>
    /// Execute the players button action.
    /// </summary>
    void PlayerAction()
    {
        if (!newPlayer)
        {
            if (!playerChosen)
            {
                playerChosen = true;
            }
            else
            {
                playerChosen = false;
            }
        }
        else
        {
            Debug.Log("Create Player");
        }
    }

    /// <summary>
    /// Creates a new manager.
    /// </summary>
    void EditManager()
    {
        Debug.Log("Edit manager");
    }

    /// <summary>
    /// Creates a new player.
    /// </summary>
    void EditPlayer()
    {
        Debug.Log("Edit player");
    }
    #endregion
}
