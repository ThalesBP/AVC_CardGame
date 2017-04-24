using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public enum GameStatus {begin, playing, paused, end};
    public enum UserStatus {unlocked, locked, newUser, creating, editing}
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
        public HideShow controlPanelVisibility, userPanelVisibility, managerPanelVisibility, resultsVisibility;
        public Button connectButton, startButton, stopButton;
        public InputField playTimeField;
        public Toggle helpToggle;
        public Slider numOfCardsSlider;
        public Texture2D mouseDefault;
        #endregion

        #region User interface
        private UserManager users;
        public Dropdown managerDropdown, playerDropdown, memberDropdown;
        public InputField managerField, playerField, passwordField, playerInfoField;
        public Button managerButton, managerEditButton, playerButton, playerEditButton;
        public RawImage managerEditImage, playerEditImage;
        public Texture editTexture, deleteTexture, cancelTexture;
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
        users = gameObject.AddComponent<UserManager>();
        UpdateUsers(managerDropdown, users.Managers);
        UpdateUsers(playerDropdown, users.players);

        managerButton.onClick.AddListener(delegate { ManagerButton1(); });
        managerEditButton.onClick.AddListener(delegate { ManagerButton2(); });
        playerButton.onClick.AddListener(delegate { PlayerButton1(); });
        playerEditButton.onClick.AddListener(delegate { PlayerButton2(); });
        playerDropdown.onValueChanged.AddListener( delegate { UpdateDescription();});
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
                //Time.timeScale = 0f;
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

        #region User Panel Text Update
        if (userPanelVisibility.showed && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            if (managerStatus != UserStatus.locked)
                ManagerButton1();
        }

        switch (managerStatus)
        {
            case UserStatus.unlocked:
                UpdateManagerActivity(true, true, true, true);
                managerEditImage.texture = editTexture;
                login.text = loginText[language];
                if (managerDropdown.captionText.text == newUserText[language])
                    managerStatus = UserStatus.newUser;
                break;
            case UserStatus.newUser:
                UpdateManagerActivity(true, false, false, true);
                login.text = addText[language];
                managerEditImage.texture = editTexture;
                if (managerDropdown.captionText.text != newUserText[language])
                    managerStatus = UserStatus.unlocked;
                break;
            case UserStatus.creating:
                UpdateManagerActivity(true, true, true, false);
                login.text = doneText[language];
                managerEditImage.texture = cancelTexture;
                if (userPanelVisibility.showed && Input.GetKey(KeyCode.Escape))
                    ManagerButton2();
                break;
            case UserStatus.editing:
                UpdateManagerActivity(true, true, true, false);
                login.text = doneText[language];
                managerEditImage.texture = deleteTexture;
                break;
            case UserStatus.locked:
                UpdateManagerActivity(false, false, false, true);
                managerEditImage.texture = editTexture;
                playerDropdown.options[users.AmountPlayers].text = newUserText[language];
                login.text = logoutText[language];
                break;
        }
        switch (playerStatus)
        {
            case UserStatus.unlocked:
                if (managerStatus == UserStatus.locked)
                    UpdatePlayerActivity(true, true, true, true, true);
                else 
                    UpdatePlayerActivity(false, false, false, false, true);
                choose.text = chooseText[language];
                playerEditImage.texture = editTexture;
                if (playerDropdown.captionText.text == newUserText[language])
                    playerStatus = UserStatus.newUser;
                break;
            case UserStatus.newUser:
                UpdatePlayerActivity(true, true, false, false, true);
                playerEditImage.texture = editTexture;
                choose.text = addText[language];
                if (playerDropdown.captionText.text != newUserText[language])
                    playerStatus = UserStatus.unlocked;
                break;
            case UserStatus.creating:
                UpdatePlayerActivity(true, true, true, false, false);
                playerEditImage.texture = cancelTexture;
                choose.text = doneText[language];
                if (userPanelVisibility.showed && Input.GetKey(KeyCode.Escape))
                    PlayerButton2();
                break;
            case UserStatus.editing:
                UpdatePlayerActivity(true, true, true, false, false);
                playerEditImage.texture = deleteTexture;
                choose.text = doneText[language];
                break;
            case UserStatus.locked:
                UpdatePlayerActivity(false, true, false, false, true);
                playerEditImage.texture = editTexture;
                choose.text = changeText[language];
                break;
        }

        userTab.text = userPanelText[language];

        managerLogin.text = managerLoginText[language];
        managerPassword.text = enterPasswordText[language];
        managerDropdown.options[users.AmountManagers].text = newUserText[language];

        playerSelect.text = playerSelectText[language];
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
    void UpdatePlayerActivity(bool dropdown, bool button, bool edit, bool memeber, bool selectMode)
    {
        playerDropdown.interactable = dropdown;
        playerButton.interactable = button;
        playerEditButton.interactable = edit;
        memberDropdown.interactable = memeber;
        playerDropdown.gameObject.SetActive(selectMode);
        playerField.gameObject.SetActive(!selectMode);
        playerInfoField.gameObject.SetActive(!selectMode);
    }
    #endregion

    #region User interface functions
    /// <summary>
    /// Executes the manager button action.
    /// </summary>
    void ManagerButton1()
    {
        switch (managerStatus)
        {
            case UserStatus.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = UserStatus.locked;
                    playerStatus = UserStatus.unlocked;
                    UpdateUsers(playerDropdown, users.players);
                    UpdateDescription();
                }
                else
                {
                    Debug.Log("Password Error");
                }
                break;
            case UserStatus.newUser:
                managerStatus = UserStatus.creating;
                managerField.text = "";
                passwordField.text = "";
                Debug.Log("Create new Manager?");
                break;
            case UserStatus.creating:
                managerStatus = UserStatus.unlocked;
                passwordField.text = "";

                Debug.Log("Creating Manager");
                break;
            case UserStatus.editing:
                managerStatus = UserStatus.unlocked;
                Debug.Log("Editing Manager");
                break;
            case UserStatus.locked:
                managerStatus = UserStatus.unlocked;
                break;
        }
    }

    /// <summary>
    /// Execute the players button action.
    /// </summary>
    void PlayerButton1()
    {
        switch (playerStatus)
        {
            case UserStatus.unlocked:
                playerStatus = UserStatus.locked;
                break;
            case UserStatus.newUser:
                playerField.text = "";
                playerInfoField.text = "";
                playerStatus = UserStatus.creating;
                break;
            case UserStatus.creating:
                playerStatus = UserStatus.unlocked;
                Debug.Log("Creating Player");
                break;
            case UserStatus.editing:
                playerStatus = UserStatus.unlocked;
                Debug.Log("Editing Player");
                break;
            case UserStatus.locked:
                playerStatus = UserStatus.unlocked;
                break;
        }
    }

    /// <summary>
    /// Creates a new manager.
    /// </summary>
    void ManagerButton2()
    {
        switch (managerStatus)
        {
            case UserStatus.creating:
                managerStatus = UserStatus.unlocked;
                Debug.Log("Cancel Creating");
                break;
            case UserStatus.editing:
                managerStatus = UserStatus.unlocked;
                Debug.Log("Delete Manager");
                break;
            case UserStatus.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = UserStatus.editing;
                    managerField.text = managerDropdown.captionText.text;
                    Debug.Log("Edit Manager");
                }
                else
                {
                    Debug.Log("Password Error");
                }
                break;
        }
    }

    /// <summary>
    /// Creates a new player.
    /// </summary>
    void PlayerButton2()
    {
        switch (playerStatus)
        {
            case UserStatus.creating:
                playerStatus = UserStatus.unlocked;
                Debug.Log("Cancel Creating");
                break;
            case UserStatus.editing:
                playerStatus = UserStatus.unlocked;
                Debug.Log("Delete Player");
                break;
            case UserStatus.unlocked:
                playerStatus = UserStatus.editing;
                playerField.text = playerDropdown.captionText.text;
                playerInfoField.text = playerDescription.text;
                Debug.Log("Edit Player");
                break;
        }
    }
    #endregion
}
