﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserPanel : GameBase {

    [Space(5)]
    [Header("Language")]
    private int language;

    private enum UserStatus {unlocked, locked, newUser, creating, editing}
    private UserStatus managerStatus = UserStatus.unlocked;
    private UserStatus playerStatus = UserStatus.unlocked;

    [Space(5)]
    [Header("Texts")]
    private Text userTab;
    private Text managerLogin;
    private Text managerUser;
    private Text managerPassword;
    private Text playerUser;
    private Text login;
    private Text playerSelect;
    private Text choose;
    private Text plan;
    private Text left;
    private Text top;
    private Text botton;
    private Text right;
    private Text descriptionEdit;


    [Space(5)]
    [Header("Interface")]
    public Dropdown managerDropdown;
    public Dropdown playerDropdown, memberDropdown, languageDropdown;
    public InputField managerField, playerField, passwordField, playerInfoField, leftField, topField, bottonField, rightField;
    public Button managerButton, managerEditButton, playerButton, playerEditButton;

    [Space(5)]
    [Header("Design")]
    public RawImage managerEditImage;
    public RawImage playerEditImage;
    public Texture editTexture, deleteTexture, cancelTexture;
    public Text playerDescription;

    [Space(5)]
    [Header("Others")]
    public HideShow visibility;
    private UserManager users;
    /// <summary>
    /// Verifies if it's logged
    /// </summary>
    public bool locked = false;     // Verifies if it's logged
    private bool passwordWrong = false, userWrong = false;

    public float[] Plan
    {
        get 
        { 
            float leftValue, topValue, bottonValue, rightValue;
            leftValue = topValue = bottonValue = rightValue = 0f;;

            if (leftField.text != "")
                leftValue = float.Parse(leftField.text);
            if (rightField.text != "")
                rightValue = float.Parse(rightField.text);
            if (topField.text != "")
                topValue = float.Parse(topField.text);
            if (bottonField.text != "")
                bottonValue = float.Parse(bottonField.text);

            float total = leftValue + topValue + bottonValue + rightValue;
            if (total == 0f)
                total = 100f;
            else
                total = total / 100f;

            leftValue = leftValue / total;
            rightValue = rightValue / total;
            topValue = topValue / total;
            bottonValue = bottonValue / total;

            return new float [] { rightValue, topValue, leftValue, bottonValue };
        }
    }

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
        plan = GameObject.Find("PlanTitle").GetComponentInChildren<Text>(true);
        left = GameObject.Find("Left_factor").GetComponentInChildren<Text>(true);
        top = GameObject.Find("Top_factor").GetComponentInChildren<Text>(true);
        botton = GameObject.Find("Botton_factor").GetComponentInChildren<Text>(true);
        right = GameObject.Find("Right_factor").GetComponentInChildren<Text>(true);
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

        plan.text = sessionPlanText[language];

        /*
        float leftValue, topValue, bottonValue, rightValue;
        leftValue = topValue = bottonValue = rightValue = 0f;;

        if (leftField.text != "")
            leftValue = float.Parse(leftField.text);
        if (rightField.text != "")
            rightValue = float.Parse(rightField.text);
        if (topField.text != "")
            topValue = float.Parse(topField.text);
        if (bottonField.text != "")
            bottonValue = float.Parse(bottonField.text);

        float total = leftValue + topValue + bottonValue + rightValue;
        if (total == 0f)
            total = 1f;
        else
            total = total / 100f;

        left.text = (leftValue / total).ToString("F1") + "%";
        right.text = (rightValue / total).ToString("F1") + "%";
        top.text = (topValue / total).ToString("F1") + "%";
        botton.text = (bottonValue / total).ToString("F1") + "%";
        */

        float[] planFactors = Plan;

        right.text = planFactors[0].ToString("F1") + "%";
        top.text = planFactors[1].ToString("F1") + "%";
        left.text = planFactors[2].ToString("F1") + "%";
        botton.text = planFactors[3].ToString("F1") + "%";

        for (int option = 0; option < numOfMembers; option++)
        {
            memberDropdown.options[option].text = limbTexts[option, language];
        }
        memberDropdown.RefreshShownValue();

        locked = (managerStatus == UserStatus.locked) && (playerStatus == UserStatus.locked);
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
    void UpdatePlayerActivity(bool dropdown, bool button, bool edit, bool options, bool selectMode)
    {
        playerDropdown.interactable = dropdown;
        playerButton.interactable = button;
        playerEditButton.interactable = edit;
        memberDropdown.interactable = options;
        leftField.interactable = options;
        topField.interactable = options;
        bottonField.interactable = options;
        rightField.interactable = options;
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
