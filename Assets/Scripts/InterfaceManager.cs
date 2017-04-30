using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : Singleton<InterfaceManager> {

    public int language;

    public ControlPanel control;
    public Texture2D mouseDefault;

    public Text playStatus;
    private Motion statusScale;
    private Outline statusBoarder;

    private Text scorePoints;
    private Text timeCounter;

    private int countDownCounter;   // Counter for count down
    public int CountDownCounter {get {return countDownCounter;}}
    public int scoreValue;

	void Start () 
    {
        countDownCounter = -1;
        Time.timeScale = 0f;

        #region General Interface Initialization
        playStatus = GameObject.Find("PlayStatus").GetComponentInChildren<Text>(true);
        statusScale = playStatus.gameObject.AddComponent<Motion>();
        statusScale.MoveTo(Vector3.one);
        statusScale.timeScaled = false;
        statusBoarder = playStatus.GetComponent<Outline>();

        scorePoints = GameObject.Find("ScorePoints").GetComponentInChildren<Text>(true);
        timeCounter = GameObject.Find("TimeCounter").GetComponentInChildren<Text>(true);
        #endregion
    }

    void Update ()
    {

        if (control.connection != null)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        else
            Cursor.SetCursor(mouseDefault, Vector2.zero, CursorMode.ForceSoftware);
        
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
}
