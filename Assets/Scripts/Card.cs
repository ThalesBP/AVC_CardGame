using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the card behaviour and information individually.
/// </summary>
public class Card : GameBase {

    enum status {idle, waiting, moving, updating};

    #region Card Infos
    public string suitName, suitSymbol, valueName, colorName;   // This may be useless
    public int suit, value, color;  // This card infos
    public Color colorRGB;
    #endregion

    public List<TextMesh> valueTexts, suitTexts;    // Text in 3D cards

    #region Motion's variables
    private Transform initial;                      // Transforme position before movement
    private Vector3 finalPos;                       // Final position desired
    private Quaternion finalRot;                    // Final rotation desired
    private float timeCounterPos, timeCounterRot;   // Counter for movement
    private float delayTimePos, delayTimeRot;       // Time before start moving
    private float deltaTimePos, deltaTimeRot;       // Total time moving
    private status statusPos, statusRot;            // Status of movement and rotation
    #endregion

    void Awake()
    {
        // Initialize variables
        delayTimePos = delayTimeRot = deltaTimePos = deltaTimeRot = 0f;
        timeCounterPos = timeCounterRot = 0f;
        initial = transform;
        statusPos = statusRot = status.idle;

        // Set visual text quality
        foreach (TextMesh value in valueTexts)
        {
            value.characterSize = charSize;
            value.fontSize = fontSize;
        }
        foreach (TextMesh value in suitTexts)
        {
            value.characterSize = charSize;
            value.fontSize = fontSize;
        }
    }

    void Update()
    {
        MoveCard();
    }

    #region Update Infos based on enums
    /// <summary>
    /// Sets card information. Color is based on suit.
    /// </summary>
    /// <param name="suitOfCard">Suit of card.</param>
    /// <param name="valueOfCard">Value of card.</param>
    public void UpdateInfos(suits suitOfCard, values valueOfCard)
    {
        suit = (int)suitOfCard;
        value = (int)valueOfCard;
        switch (suit)
        {
            case (int)suits.Diamond:
            case (int)suits.Heart:
                color = (int)colors.Red;
                break;
            case (int)suits.Spades:
            case (int)suits.Club:
                color = (int)colors.Black;
                break;
        }

        UpdateInfos();
    }


    /// <summary>
    /// Sets card information.
    /// </summary>
    /// <param name="suitOfCard">Suit of card.</param>
    /// <param name="valueOfCard">Value of card.</param>
    /// <param name="colorOfCard">Color of card.</param>
    public void UpdateInfos(suits suitOfCard, values valueOfCard, colors colorOfCard)
    {
        suit = (int)suitOfCard;
        value = (int)valueOfCard;
        color = (int)colorOfCard;
        UpdateInfos();
    }
    #endregion

    #region Update Infos functions
    /// <summary>
    /// Sets card information.
    /// </summary>
    /// <param name="suitOfCard">Suit of card.</param>
    /// <param name="valueOfCard">Value of card.</param>
    /// <param name="colorOfCard">Color of card.</param>
    public void UpdateInfos(int suitOfCard, int valueOfCard)
    {
        suit = suitOfCard;
        value = valueOfCard;
        switch (suit)
        {
            case (int)suits.Diamond:
            case (int)suits.Heart:
                color = (int)colors.Red;
                break;
            case (int)suits.Spades:
            case (int)suits.Club:
                color = (int)colors.Black;
                break;
        }
        UpdateInfos();
    }

    /// <summary>
    /// Sets card information.
    /// </summary>
    /// <param name="suitOfCard">Suit of card.</param>
    /// <param name="valueOfCard">Value of card.</param>
    /// <param name="colorOfCard">Color of card.</param>
    public void UpdateInfos(int suitOfCard, int valueOfCard, int colorOfCard)
    {
        suit = suitOfCard;
        value = valueOfCard;
        color = colorOfCard;
        UpdateInfos();
    }

    /// <summary>
    /// Updates the names of information.
    /// </summary>
    private void UpdateInfos()
    {
        suitName = suitNames[suit];
        suitSymbol = suitSymbols[suit];
        valueName = valueNames[value];
        colorName = colorNames[color];
        switch ((colors)color)
        {
            case colors.Black:
                colorRGB = Color.black;
                break;
            case colors.Red:
                colorRGB = Color.red;
                break;
            default:
                colorRGB = Color.blue;
                break;    
        }

        foreach (TextMesh value in valueTexts)
        {
            value.text = valueName;
            value.color = colorRGB;
        }


        foreach (TextMesh suit in suitTexts)
        {
            suit.text = suitSymbol;
            suit.color = colorRGB;
        }

    }
    #endregion

    #region Move card functions
    /// <summary>
    /// Counts and check the timers.
    /// </summary>
    public void Counter()
    {
        // Postion counters
        if (statusPos != status.idle)
        {
            if (timeCounterPos < delayTimePos)
            {
                statusPos = status.waiting;
                timeCounterPos += Time.deltaTime;
            }
            else if (timeCounterPos < delayTimePos + deltaTimePos)
            {
                statusPos = status.moving;
                timeCounterPos += Time.deltaTime;
            }
            else
                statusPos = status.updating;
        }
        // Rotation counters
        if (statusRot != status.idle)
        {
            if (timeCounterRot < delayTimeRot)
            {
                statusRot = status.waiting;
                timeCounterRot += Time.deltaTime;
            }
            else if (timeCounterRot < delayTimeRot + deltaTimeRot)
            {
                statusRot = status.moving;
                timeCounterRot += Time.deltaTime;
            }
            else
                statusRot = status.updating;
        }
    }

    /// <summary>
    /// Moves the card based on time counters.
    /// </summary>
    private void MoveCard()
    {
        Counter();
        // Update position
        switch (statusPos)
        {
            case status.idle:
                break;
            case status.moving:
                transform.position = Vector3.Lerp(initial.position, finalPos, Mathf.SmoothStep(0f, 1f, (timeCounterPos - delayTimePos) / deltaTimePos));
                break;
            case status.waiting:
                break;
            case status.updating:
                transform.position = initial.position = finalPos;
                statusPos = status.idle;
                break;
        }
        // Update rotation
        switch (statusRot)
        {
            case status.idle:
                break;
            case status.moving:
                transform.rotation = Quaternion.Lerp(initial.rotation, finalRot, Mathf.SmoothStep(0f, 1f, (timeCounterRot - delayTimeRot) / deltaTimeRot));
                break;
            case status.waiting:
                break;
            case status.updating:
                transform.rotation = initial.rotation = finalRot;
                statusRot = status.idle;
                break;
        }
    }

    /// <summary>
    /// Moves the card immediately to a position.
    /// </summary>
    /// <param name="destiny">Destiny place to move.</param>
    public void MoveCard(Vector3 destiny)
    {
        transform.position = initial.position = destiny;
    }

    /// <summary>
    /// Moves the card immediately to a rotation.
    /// </summary>
    /// <param name="destiny">Destiny rotation to move.</param>
    public void MoveCard(Quaternion destiny)
    {
        transform.rotation = initial.rotation = destiny;
    }

    /// <summary>
    /// Moves the card from where it is to destiny place.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    public void MoveCard(Vector3 destiny, float deltaTime)
    {
        timeCounterPos = 0f;
        deltaTimePos = deltaTime;
        finalPos = destiny;
        statusPos = status.moving;
    }

    /// <summary>
    /// Moves the card from where it is to destiny place after some time.
    /// </summary>
    /// <param name="destiny">Destiny place to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    /// <param name="delayTime">Time it waits to start moving.</param>
    public void MoveCard(Vector3 destiny, float deltaTime, float delayTime)
    {
        delayTimePos = delayTime;
        MoveCard(destiny, deltaTime);
    }


    /// <summary>
    /// Moves the card from its rotation to destiny rotation.
    /// </summary>
    /// <param name="destiny">Destiny rotation to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    public void MoveCard(Quaternion destiny, float deltaTime)
    {
        timeCounterRot = 0f;
        deltaTimeRot = deltaTime;
        finalRot = destiny;
        statusRot = status.moving;
    }

    /// <summary>
    /// Moves the card from its rotation to destiny rotation after some time.
    /// </summary>
    /// <param name="destiny">Destiny rotation to reach.</param>
    /// <param name="deltaTime">Time it takes to reach it.</param>
    /// <param name="delayTime">Time it waits to start rotatin.</param>
    public void MoveCard(Quaternion destiny, float deltaTime, float delayTime)
    {
        delayTimeRot = delayTime;
        MoveCard(destiny, deltaTime);
    }

    /// <summary>
    /// Moves the card from its initial position and rotation to destiny ones.
    /// </summary>
    /// <param name="destinyPos">Destiny position to reach.</param>
    /// <param name="destinyRot">Destiny rotation to reach.</param>
    /// <param name="deltaTime">Time it takes to reach.</param>
    public void MoveCard(Vector3 destinyPos, Quaternion destinyRot, float deltaTime)
    {
        MoveCard(destinyPos, deltaTime);
        MoveCard(destinyRot, deltaTime);
    }

    /// <summary>
    /// Moves the card from its initial position and rotation to destiny ones after some time.
    /// </summary>
    /// <param name="destinyPos">Destiny position to reach.</param>
    /// <param name="destinyRot">Destiny rotation to reach.</param>
    /// <param name="deltaTime">Time it takes to reach.</param>
    /// <param name="delayTime">Time it takes to reach.</param>
    public void MoveCard(Vector3 destinyPos, Quaternion destinyRot, float deltaTime, float delayTime)
    {
        delayTimePos = deltaTime;
        delayTimeRot = deltaTime;
        MoveCard(destinyPos, destinyRot, deltaTime);
    }

    /*
    public void MoveCard(Vector3 final, float deltaTime)
    {
        if (deltaTime > 0f)
        {
            if (transform.position != final)
            {
                timeCounterPos += Time.deltaTime;
                transform.position = Vector3.Lerp(initial.position, final, Mathf.SmoothStep(0f, 1f, timeCounterPos / deltaTime));
            }
            else
            {
                delayCounterPos = 0f;
                timeCounterPos = 0f;
                initial.position = transform.position;
            }
        }
        else
        {
            delayCounterPos = 0f;
            timeCounterPos = 0f;
            transform.position = initial.position = final;
        }
    }

    public void MoveCard(Vector3 final, float deltaTime, float delayTime)
    {
        if (transform.position != final)
        {
            if (delayCounterPos > delayTime)
                MoveCard(final, deltaTime);
            else 
                delayCounterPos += Time.deltaTime;
        }
    }

    public void MoveCard(Quaternion final, float deltaTime)
    {
        if (deltaTime > 0f)
        {
            if (transform.rotation != final)
            {
                timeCounterRot += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(initial.rotation, final, Mathf.SmoothStep(0f, 1f, timeCounterRot / deltaTime));
            }
            else
            {
                delayCounterRot = 0f;
                timeCounterRot = 0f;
                initial.rotation = transform.rotation;
            }
        }
        else
        {
            delayCounterRot = 0f;
            timeCounterRot = 0f;
            transform.rotation = initial.rotation = final;
        }
    }

    public void MoveCard(Quaternion final, float deltaTime, float delayTime)
    {
        if (transform.rotation != final)
        {
            if (timeCounterRot > delayTime)
                MoveCard(final, deltaTime);
            else
                delayCounterRot += Time.deltaTime;
        }
    }*/
    #endregion
}
