using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : GameBase {

    private int language;

    private enum Status {unlocked, locked, newUser, creating, editing}
    private Status managerStatus = Status.unlocked;
    private Status playerStatus = Status.unlocked;

    private Text userTab;
    private Text managerLogin;
    private Text managerUser;
    private Text managerPassword;
    private Text playerUser;
    private Text login;
    private Text playerSelect;
    private Text choose;
    private Text descriptionEdit;

    private UserManager users;
    private HideShow visibility;
    public Dropdown managerDropdown, playerDropdown, memberDropdown;
    public InputField managerField, playerField, passwordField, playerInfoField;
    public Button managerButton, managerEditButton, playerButton, playerEditButton;
    public RawImage managerEditImage, playerEditImage;
    public Texture editTexture, deleteTexture, cancelTexture;
    public Text playerDescription;
    private bool passwordWrong = false, userWrong = false;

	void Start () 
    {
        userTab = GameObject.Find("UserTabName").GetComponentInChildren<Text>(true);
        managerLogin = GameObject.Find("ManagerDescription").GetComponentInChildren<Text>(true);
        managerUser = GameObject.Find("NewManagerPlaceholder").GetComponentInChildren<Text>(true);
        managerPassword = GameObject.Find("ManagerPasswordPlaceholder").GetComponentInChildren<Text>(true);
        playerUser = GameObject.Find("NewPlayerPlaceholder").GetComponentInChildren<Text>(true);
        login = GameObject.Find("Login").GetComponentInChildren<Text>(true);
        playerSelect = GameObject.Find("PlayerDescription").GetComponentInChildren<Text>(true);
        choose = GameObject.Find("Choose").GetComponentInChildren<Text>(true);
        descriptionEdit = GameObject.Find("PlayerInfoPlaceholder").GetComponentInChildren<Text>(true);

        users = gameObject.AddComponent<UserManager>();
        UpdateDropdown(managerDropdown, users.Managers);
        UpdateDropdown(playerDropdown, users.Players);
        UpdateDescription();

        managerButton.onClick.AddListener(delegate { ManagerButton1(); });
        managerEditButton.onClick.AddListener(delegate { ManagerButton2(); });
        playerButton.onClick.AddListener(delegate { PlayerButton1(); });
        playerEditButton.onClick.AddListener(delegate { PlayerButton2(); });
        playerDropdown.onValueChanged.AddListener( delegate { UpdateDescription();});

        visibility = GetComponent<HideShow>();
    }
	
	void Update () 
    {
        language = (int)chosenLanguage;

        if (visibility.showed && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            if (managerStatus != Status.locked)
                ManagerButton1();
            else if (playerStatus != Status.locked)
                PlayerButton1();
        }

        switch (managerStatus)
        {
            case Status.unlocked:
                UpdateManagerActivity(true, true, true, true);
                managerEditImage.texture = editTexture;
                login.text = loginText[language];
                userWrong = false;
                if (managerDropdown.value == users.AmountManagers)
                    managerStatus = Status.newUser;
                break;
            case Status.newUser:
                UpdateManagerActivity(true, false, false, true);
                login.text = addText[language];
                managerEditImage.texture = editTexture;
                if (managerDropdown.value != users.AmountManagers)
                    managerStatus = Status.unlocked;
                break;
            case Status.creating:
                UpdateManagerActivity(true, true, true, false);
                login.text = doneText[language];
                managerEditImage.texture = cancelTexture;
                if (visibility.showed && Input.GetKey(KeyCode.Escape))
                    ManagerButton2();
                break;
            case Status.editing:
                UpdateManagerActivity(true, true, true, false);
                login.text = doneText[language];
                if ((managerField.text == managerDropdown.options[managerDropdown.value].text) && (users.CheckPassword(managerDropdown.value, passwordField.text)))
                    managerEditImage.texture = deleteTexture;
                else 
                {
                    managerEditImage.texture = cancelTexture;
                    managerEditButton.image.color = GreenColor;
                }
                break;
            case Status.locked:
                UpdateManagerActivity(false, false, false, true);
                managerEditImage.texture = editTexture;
                playerDropdown.options[users.AmountPlayers].text = insertUserText[language];
                login.text = logoutText[language];
                break;
        }
        switch (playerStatus)
        {
            case Status.unlocked:
                if (managerStatus == Status.locked)
                {
                    UpdatePlayerActivity(true, true, true, true, true);
                    userWrong = false;
                }
                else 
                    UpdatePlayerActivity(false, false, false, false, true);
                choose.text = chooseText[language];
                playerEditImage.texture = editTexture;
                if (playerDropdown.value == users.AmountPlayers)
                    playerStatus = Status.newUser;
                break;
            case Status.newUser:
                UpdatePlayerActivity(true, true, false, false, true);
                playerEditImage.texture = editTexture;
                choose.text = addText[language];
                if (playerDropdown.value != users.AmountPlayers)
                    playerStatus = Status.unlocked;
                break;
            case Status.creating:
                UpdatePlayerActivity(true, true, true, false, false);
                playerEditImage.texture = cancelTexture;
                choose.text = doneText[language];
                if (visibility.showed && Input.GetKey(KeyCode.Escape))
                    PlayerButton2();
                break;
            case Status.editing:
                UpdatePlayerActivity(true, true, true, false, false);
                if ((playerField.text == playerDropdown.options[playerDropdown.value].text) && (playerInfoField.text == playerDescription.text))
                    playerEditImage.texture = deleteTexture;
                else
                {
                    playerEditImage.texture = cancelTexture;
                    playerEditButton.image.color = GreenColor;
                }
                choose.text = doneText[language];
                break;
            case Status.locked:
                UpdatePlayerActivity(false, true, false, false, true);
                playerEditImage.texture = editTexture;
                choose.text = changeText[language];
                break;
        }

        userTab.text = userPanelText[language];

        managerLogin.text = managerLoginText[language];
        managerDropdown.options[users.AmountManagers].text = insertUserText[language];

        if (userWrong)
        {
            managerUser.text = userWrongText[language];
            playerUser.text = userWrongText[language];
        }
        else 
        {
            managerUser.text = insertUserText[language];
            playerUser.text = insertUserText[language];
        }

        if (passwordWrong)
            managerPassword.text = passwordWrongText[language];
        else
            managerPassword.text = enterPasswordText[language];

        playerSelect.text = playerSelectText[language];
        descriptionEdit.text = insertInfoText[language];

        for (int option = 0; option < memberDropdown.options.Count; option++)
        {
            memberDropdown.options[option].text = limbTexts[option + 2, language];
        }
        memberDropdown.RefreshShownValue();
	}

    #region Interface functions
    /// <summary>
    /// Updates a specified dropdown.
    /// </summary>
    /// <param name="dropdown">Dropdown to be updated.</param>
    /// <param name="users">Users to be uploaded.</param>
    void UpdateDropdown(Dropdown dropdown, List<string> users)
    {
        List<string> users_aux = new List<string>();
        if (users.Count > 0)
        {
            foreach (string user in users)
                users_aux.Add(user);
        }
        users_aux.Add(insertUserText[language]);

        dropdown.ClearOptions();
        dropdown.AddOptions(users_aux);
        users_aux.Clear();
    }

    /// <summary>
    /// Updates the description space based on the player.
    /// </summary>
    void UpdateDescription()
    {
        if (playerDropdown.value == users.AmountPlayers)
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

    /// <summary>
    /// Verifies the repentance of a name.
    /// </summary>
    /// <returns><c>true</c>, if repentance was verifyed, <c>false</c> otherwise.</returns>
    /// <param name="options">Options to be verifyed.</param>
    /// <param name="name">Name to find.</param>
    bool VerifyRepentance(Dropdown options, string name)
    {
        foreach (Dropdown.OptionData option in options.options)
        {
            if (option.text == name)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Verifies the repentance excluding a option.
    /// </summary>
    /// <returns><c>true</c>, if repentance was verifyed, <c>false</c> otherwise.</returns>
    /// <param name="options">Options to be verifyed..</param>
    /// <param name="name">Name to find.</param>
    /// <param name="value">Value of element to be excluded.</param>
    bool VerifyRepentance(Dropdown options, string name, int value)
    {
        for (int i = 0; i < options.options.Count; i++)
        {
            if (i != value)
            if (options.options[i].text == name)
                return true;
        }
        return false;
    }
    #endregion

    #region Buttons functions
    /// <summary>
    /// Executes the manager button action.
    /// </summary>
    void ManagerButton1()
    {
        switch (managerStatus)
        {
            case Status.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = Status.locked;
                    playerStatus = Status.unlocked;

                    passwordWrong = false;
                    managerUser.color = BlackMatteColor;
                    managerPassword.color = BlackMatteColor;
                }
                else
                {
                    passwordWrong = true;
                    managerPassword.color = RedColor;
                    passwordField.text = "";
                }
                break;
            case Status.newUser:
                managerStatus = Status.creating;
                managerField.text = "";
                passwordField.text = "";
                passwordWrong = false;
                managerUser.color = BlackMatteColor;
                managerPassword.color = BlackMatteColor;
                break;
            case Status.creating:
                if ((managerField.text == "") || (passwordField.text == ""))
                {
                    managerUser.color = RedColor;
                    managerPassword.color = RedColor;
                }
                else
                {
                    userWrong = VerifyRepentance(managerDropdown, managerField.text);

                    if (userWrong)
                    {
                        managerUser.color = RedColor;
                        managerField.text = "";
                    }
                    else
                    {   
                        users.AddManager(managerField.text, passwordField.text);

                        managerUser.color = BlackMatteColor;
                        managerPassword.color = BlackMatteColor;
                        managerStatus = Status.unlocked;
                    }
                }
                break;
            case Status.editing:
                managerEditButton.image.color = GreenColor;
                if ((managerField.text == "") || (passwordField.text == ""))
                {
                    managerUser.color = RedColor;
                    managerPassword.color = RedColor;
                }
                else
                {
                    userWrong = VerifyRepentance(managerDropdown, managerField.text, managerDropdown.value);

                    if (userWrong)
                    {
                        managerUser.color = RedColor;
                        managerField.text = "";
                    }
                    else
                    {   
                        users.ChangeManager(managerDropdown.value, managerField.text, passwordField.text);

                        managerUser.color = BlackMatteColor;
                        managerPassword.color = BlackMatteColor;
                        managerStatus = Status.unlocked;
                    }
                }
                break;
            case Status.locked:
                managerStatus = Status.unlocked;
                playerStatus = Status.unlocked;
                managerUser.color = BlackMatteColor;
                managerPassword.color = BlackMatteColor;
                playerDropdown.value = 0;
                break;
        }
        UpdateDropdown(managerDropdown, users.Managers);
        UpdateDropdown(playerDropdown, users.Players);
        UpdateDescription();
    }

    /// <summary>
    /// Execute the players button action.
    /// </summary>
    void PlayerButton1()
    {
        switch (playerStatus)
        {
            case Status.unlocked:
                playerUser.color = BlackMatteColor;
                playerStatus = Status.locked;
                break;
            case Status.newUser:
                playerField.text = "";
                playerInfoField.text = "";
                playerUser.color = BlackMatteColor;
                playerStatus = Status.creating;
                break;
            case Status.creating:
                if (playerField.text == "")
                {
                    playerUser.color = RedColor;
                }
                else
                {
                    userWrong = VerifyRepentance(playerDropdown, playerField.text);

                    if (userWrong)
                    {
                        playerUser.color = RedColor;
                        playerField.text = "";
                    }
                    else
                    {   
                        users.AddPlayer(playerField.text, playerInfoField.text);

                        playerUser.color = BlackMatteColor;
                        playerStatus = Status.unlocked;
                    }
                }
                break;
            case Status.editing:
                playerEditButton.image.color = GreenColor;
                if (playerField.text == "")
                {
                    playerUser.color = RedColor;
                }
                else
                {
                    userWrong = VerifyRepentance(playerDropdown, playerField.text, playerDropdown.value);

                    if (userWrong)
                    {
                        playerUser.color = RedColor;
                        playerField.text = "";
                    }
                    else
                    {   
                        users.ChangePlayer(playerDropdown.value, playerField.text, playerInfoField.text);

                        playerUser.color = BlackMatteColor;
                        playerStatus = Status.unlocked;
                    }
                }
                break;
            case Status.locked:
                playerUser.color = BlackMatteColor;
                playerStatus = Status.unlocked;
                break;
        }
        UpdateDropdown(managerDropdown, users.Managers);
        UpdateDropdown(playerDropdown, users.Players);
        UpdateDescription();
    }

    /// <summary>
    /// Creates a new manager.
    /// </summary>
    void ManagerButton2()
    {
        switch (managerStatus)
        {
            case Status.creating:
                managerStatus = Status.unlocked;
                managerUser.color = BlackMatteColor;
                managerPassword.color = BlackMatteColor;
                break;
            case Status.editing:
                if ((managerField.text == managerDropdown.options[managerDropdown.value].text) && (users.CheckPassword(managerDropdown.value, passwordField.text)))
                {
                    if (managerEditButton.image.color == GreenColor)
                        managerEditButton.image.color = Color.red;
                    else
                    {
                        managerStatus = Status.unlocked;
                        managerUser.color = BlackMatteColor;
                        managerPassword.color = BlackMatteColor;
                        managerEditButton.image.color = GreenColor;
                        users.RemoveManager(managerDropdown.value, passwordField.text);
                    }
                }
                else
                {
                    managerStatus = Status.unlocked;
                    managerUser.color = BlackMatteColor;
                    managerPassword.color = BlackMatteColor;
                }
                break;
            case Status.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = Status.editing;
                    managerField.text = managerDropdown.captionText.text;
                    passwordWrong = false;
                    managerUser.color = BlackMatteColor;
                    managerPassword.color = BlackMatteColor;
                }
                else
                {
                    passwordWrong = true;
                    managerPassword.color = RedColor;
                    passwordField.text = "";
                }
                break;
        }
        UpdateDropdown(managerDropdown, users.Managers);
        UpdateDropdown(playerDropdown, users.Players);
        UpdateDescription();
    }

    /// <summary>
    /// Creates a new player.
    /// </summary>
    void PlayerButton2()
    {
        switch (playerStatus)
        {
            case Status.creating:
                playerUser.color = BlackMatteColor;
                playerStatus = Status.unlocked;
                Debug.Log("Cancel Creating");
                break;
            case Status.editing:
                if ((playerField.text == playerDropdown.options[playerDropdown.value].text) && (playerInfoField.text == playerDescription.text))
                {
                    if (playerEditButton.image.color == GreenColor)
                        playerEditButton.image.color = Color.red;
                    else
                    {
                        playerStatus = Status.unlocked;
                        playerUser.color = BlackMatteColor;
                        playerEditButton.image.color = GreenColor;
                        users.RemovePlayer(playerDropdown.value);
                    }
                }
                else
                {
                    playerStatus = Status.unlocked;
                    playerUser.color = BlackMatteColor;
                }
                break;
            case Status.unlocked:
                playerUser.color = BlackMatteColor;
                playerStatus = Status.editing;
                playerField.text = playerDropdown.captionText.text;
                playerInfoField.text = playerDescription.text;
                Debug.Log("Edit Player");
                break;
        }
        UpdateDropdown(managerDropdown, users.Managers);
        UpdateDropdown(playerDropdown, users.Players);
        UpdateDescription();
    }
    #endregion
}
