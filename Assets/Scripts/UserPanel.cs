using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserPanel : GameBase {

    private int language;

    private enum UserStatus {unlocked, locked, newUser, creating, editing}
    private UserStatus managerStatus = UserStatus.unlocked;
    private UserStatus playerStatus = UserStatus.unlocked;

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
    public Dropdown managerDropdown, playerDropdown, memberDropdown, languageDropdown;
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
        chosenLanguage = (Languages)languageDropdown.value;
        language = (int)chosenLanguage;

        if (visibility.showed && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            if (managerStatus != UserStatus.locked)
                ManagerButton1();
            else if (playerStatus != UserStatus.locked)
                PlayerButton1();
        }

        // When TAB is pressed, we should select the next selectable UI element
        if (visibility.showed && Input.GetKeyDown(KeyCode.Tab)) 
        {
            Selectable next = null;
            Selectable current = null;

            // Figure out if we have a valid current selected gameobject
            if (EventSystem.current.currentSelectedGameObject != null) 
            {
                // Unity doesn't seem to "deselect" an object that is made inactive
                if (EventSystem.current.currentSelectedGameObject.activeInHierarchy) 
                {
                    current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

                }
            }

            if (current != null) 
            {
                // When SHIFT is held along with tab, go backwards instead of forwards
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    next = current.FindSelectableOnLeft();
                    if (next == null) 
                    {
                        next = current.FindSelectableOnUp();
                    }
                } 
                else 
                {
                    next = current.FindSelectableOnRight();
                    if (next == null) 
                    {
                        next = current.FindSelectableOnDown();
                    }
                }
            } else 
            {
                // If there is no current selected gameobject, select the first one
                if (Selectable.allSelectables.Count > 0) 
                {
                    next = Selectable.allSelectables[0];
                }
            }

            if (next != null)  
            {
                next.Select();
            }
        }


        switch (managerStatus)
        {
            case UserStatus.unlocked:
                UpdateManagerActivity(true, true, true, true);
                managerEditImage.texture = editTexture;
                login.text = loginText[language];
                userWrong = false;
                if (managerDropdown.value == users.AmountManagers)
                    managerStatus = UserStatus.newUser;
                break;
            case UserStatus.newUser:
                UpdateManagerActivity(true, false, false, true);
                login.text = addText[language];
                managerEditImage.texture = editTexture;
                if (managerDropdown.value != users.AmountManagers)
                    managerStatus = UserStatus.unlocked;
                break;
            case UserStatus.creating:
                UpdateManagerActivity(true, true, true, false);
                login.text = doneText[language];
                managerEditImage.texture = cancelTexture;
                if (visibility.showed && Input.GetKey(KeyCode.Escape))
                    ManagerButton2();
                break;
            case UserStatus.editing:
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
            case UserStatus.locked:
                UpdateManagerActivity(false, false, false, true);
                managerEditImage.texture = editTexture;
                playerDropdown.options[users.AmountPlayers].text = insertUserText[language];
                login.text = logoutText[language];
                break;
        }
        switch (playerStatus)
        {
            case UserStatus.unlocked:
                if (managerStatus == UserStatus.locked)
                {
                    UpdatePlayerActivity(true, true, true, true, true);
                    userWrong = false;
                }
                else 
                    UpdatePlayerActivity(false, false, false, false, true);
                choose.text = chooseText[language];
                playerEditImage.texture = editTexture;
                if (playerDropdown.value == users.AmountPlayers)
                    playerStatus = UserStatus.newUser;
                break;
            case UserStatus.newUser:
                UpdatePlayerActivity(true, true, false, false, true);
                playerEditImage.texture = editTexture;
                choose.text = addText[language];
                if (playerDropdown.value != users.AmountPlayers)
                    playerStatus = UserStatus.unlocked;
                break;
            case UserStatus.creating:
                UpdatePlayerActivity(true, true, true, false, false);
                playerEditImage.texture = cancelTexture;
                choose.text = doneText[language];
                if (visibility.showed && Input.GetKey(KeyCode.Escape))
                    PlayerButton2();
                break;
            case UserStatus.editing:
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
            case UserStatus.locked:
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
            case UserStatus.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = UserStatus.locked;
                    playerStatus = UserStatus.unlocked;

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
            case UserStatus.newUser:
                managerStatus = UserStatus.creating;
                managerField.text = "";
                passwordField.text = "";
                passwordWrong = false;
                managerUser.color = BlackMatteColor;
                managerPassword.color = BlackMatteColor;
                break;
            case UserStatus.creating:
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
                        managerStatus = UserStatus.unlocked;
                    }
                }
                break;
            case UserStatus.editing:
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
                        managerStatus = UserStatus.unlocked;
                    }
                }
                break;
            case UserStatus.locked:
                managerStatus = UserStatus.unlocked;
                playerStatus = UserStatus.unlocked;
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
            case UserStatus.unlocked:
                playerUser.color = BlackMatteColor;
                playerStatus = UserStatus.locked;
                break;
            case UserStatus.newUser:
                playerField.text = "";
                playerInfoField.text = "";
                playerUser.color = BlackMatteColor;
                playerStatus = UserStatus.creating;
                break;
            case UserStatus.creating:
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
                        playerStatus = UserStatus.unlocked;
                    }
                }
                break;
            case UserStatus.editing:
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
                        playerStatus = UserStatus.unlocked;
                    }
                }
                break;
            case UserStatus.locked:
                playerUser.color = BlackMatteColor;
                playerStatus = UserStatus.unlocked;
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
            case UserStatus.creating:
                managerStatus = UserStatus.unlocked;
                managerUser.color = BlackMatteColor;
                managerPassword.color = BlackMatteColor;
                break;
            case UserStatus.editing:
                if ((managerField.text == managerDropdown.options[managerDropdown.value].text) && (users.CheckPassword(managerDropdown.value, passwordField.text)))
                {
                    if (managerEditButton.image.color == GreenColor)
                        managerEditButton.image.color = Color.red;
                    else
                    {
                        managerStatus = UserStatus.unlocked;
                        managerUser.color = BlackMatteColor;
                        managerPassword.color = BlackMatteColor;
                        managerEditButton.image.color = GreenColor;
                        users.RemoveManager(managerDropdown.value, passwordField.text);
                    }
                }
                else
                {
                    managerStatus = UserStatus.unlocked;
                    managerUser.color = BlackMatteColor;
                    managerPassword.color = BlackMatteColor;
                }
                break;
            case UserStatus.unlocked:
                if (users.CheckPassword(managerDropdown.value, passwordField.text))
                {
                    managerStatus = UserStatus.editing;
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
            case UserStatus.creating:
                playerUser.color = BlackMatteColor;
                playerStatus = UserStatus.unlocked;
                Debug.Log("Cancel Creating");
                break;
            case UserStatus.editing:
                if ((playerField.text == playerDropdown.options[playerDropdown.value].text) && (playerInfoField.text == playerDescription.text))
                {
                    if (playerEditButton.image.color == GreenColor)
                        playerEditButton.image.color = Color.red;
                    else
                    {
                        playerStatus = UserStatus.unlocked;
                        playerUser.color = BlackMatteColor;
                        playerEditButton.image.color = GreenColor;
                        users.RemovePlayer(playerDropdown.value);
                    }
                }
                else
                {
                    playerStatus = UserStatus.unlocked;
                    playerUser.color = BlackMatteColor;
                }
                break;
            case UserStatus.unlocked:
                playerUser.color = BlackMatteColor;
                playerStatus = UserStatus.editing;
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
