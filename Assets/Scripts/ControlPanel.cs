﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : GameBase {

    private int language;

    public Status status = Status.begin;

    private Text panel;

    private Text connect;
    private Text start;
    private Text stop;
    private Text playTime;
    private Text playTimeInput;
    private Text help;
    private Text sliderText;

    // Transform in list later
    private Text metric1;
    private Text metric2;
    private Text metric3;

    private Text gameMessages;

    [Range(0.1f, 3.0f)]
    public float gameSpeed = 1f;
    public float gameTime;
    public float totalGameTime;

    public float metric1Value;
    public float metric2Value;
    public float metric3Value;

    public HideShow visibility;
    public ResultsPanel results;
    public Button connectButton, startButton, stopButton;
    public InputField playTimeField;
    public Toggle helpToggle;
    public Slider slider;

    public Status gameStatus = Status.newTurn;

    public Connection connection;

    void Start () 
    {
        panel = GameObject.Find("ControlTabName").GetComponentInChildren<Text>(true);

        connect = GameObject.Find("Connect").GetComponentInChildren<Text>(true);
        start = GameObject.Find("Start").GetComponentInChildren<Text>(true);
        stop = GameObject.Find("Stop").GetComponentInChildren<Text>(true);
        playTime = GameObject.Find("PlayTimePlaceholder").GetComponentInChildren<Text>(true);
        playTimeInput = GameObject.Find("PlayTime").GetComponentInChildren<Text>(true);
        help = GameObject.Find("VisualHelp").GetComponentInChildren<Text>(true);
        sliderText = GameObject.Find("SliderText").GetComponentInChildren<Text>(true);

        metric1 = GameObject.Find("Metric1").GetComponentInChildren<Text>(true);
        metric2 = GameObject.Find("Metric2").GetComponentInChildren<Text>(true);
        metric3 = GameObject.Find("Metric3").GetComponentInChildren<Text>(true);

        gameMessages = GameObject.Find("GameMessage").GetComponentInChildren<Text>(true);
	
        startButton.onClick.AddListener(delegate { SwitchStartPause(); });
        stopButton.interactable = false;
        stopButton.onClick.AddListener(delegate { FinishGame(); });
        helpToggle.interactable = false;
        connectButton.onClick.AddListener(delegate { Connect(); });
    }
	
	void Update () 
    {
        language = (int)chosenLanguage;

        panel.text = controlPanelText[language];

        metric1.text = metric1Text[language] + "\n" + metric1Value.ToString("F0") + "%";
        metric2.text = metric2Text[language] + "\n" + metric2Value.ToString("F0") + "%";
        metric3.text = metric3Text[language] + "\n" + metric3Value.ToString("F0") + "%";

        if (connection == null)
            connect.text = connectText[language];
        else if (connection.connected)
            connect.text = disconnectText[language];
        else
            connect.text = connectingText[language];
        
        stop.text = stopText[language];
        help.text = helpText[language];
        sliderText.text = slider.value.ToString("F0") + " " + cardsText[language];

        gameMessages.text = gameMessageTexts[(int)status, language] + "\n" + gameMessageTexts[(int)gameStatus, language];

        switch (status)
        {
            case Status.begin:
                start.text = startText[language];
                playTime.text = playTimeText[language];
                gameTime = totalGameTime = 0f;
                break;
            case Status.paused:
                start.text = startText[language];
                playTime.text = infTimeText[language];
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
                    FinishGame();
                }
//              gameMessage = Messages.gameRunning;
                break;
            case Status.end:
                start.text = restartText[language];
                playTime.text = infTimeText[language];
                Time.timeScale = gameSpeed;
    //            gameMessage = Messages.showingResults;
    //            gameMessages.text = gameMessageTexts[(int)gameMessage, language];
                break;
        }
   //     gameMessages.text = gameMessageTexts[(int)gameMessage, language] + "\n" + gameMessageTexts[(int)dealerMessage, language];
	}

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGame()
    {
        status = Status.paused;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    private void StartGame()
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

        status = Status.end;
    }

    /// <summary>
    /// Switchs between start and pause status.
    /// </summary>
    private void SwitchStartPause()
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
                results.visibility.Hide();
                Choice.ResetChoice();
                break;
        }
    }

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
}