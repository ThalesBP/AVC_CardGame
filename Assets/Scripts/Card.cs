﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the card behaviour and information individually.
/// </summary>
public class Card : GameBase {

    /// <summary>
    /// Card status used to highlight or not, for example.
    /// </summary>
    public enum  Highlight {free, right, wrong, contrast};
    public enum SuitType {noSuit, miniSuit, singleSuit, multiSuit, complete};
    public enum ValueType {noValue, doubleValue, bigValue};

    [Space(5)]
    [Header("Setup")]
    public Highlight status;   // Highlight status
    public SuitType suitType;
    public ValueType valueType;

    [Space(5)]
    [Header("Infos")]
    #region Card Infos
    public string suitName;
    public string suitSymbol, valueName, colorName;   // This card textes and symbols
    public int suit, value, color;  // This card ìndexes
    public Color colorRGB;  // Color of this card
    public bool showed;
    #endregion

    [Space(5)]
    [Header("Design")]
    #region Design Infos
    public List<TextMesh> valueTexts;
    public List<TextMesh> suitTexts;    // Text in 3D cards
    [HideInInspector]
    public GameObject highlight;    // Object that highlights the card as right or wrong
    [HideInInspector]
    public Material wrongCardMat, rightCardMat, contrastMat; //  Materials for highlight object
    private MeshRenderer cardMeshRender;    // Mesh render of highlight object
    #endregion

    [Space(5)]
    [Header("Motion")]
    #region Motion's variables
    public Motion position;
    public Motion rotation, scale;
    public float timeToTwinkle;       // Timer counter before player plays
    #endregion

    void Awake()
    {
        // Initialize variables
        position = gameObject.AddComponent<Motion>();
        rotation = gameObject.AddComponent<Motion>();
        scale = gameObject.AddComponent<Motion>();
        suit = value = color = 0;
        showed = false;

        position.MoveTo(transform.position);
        rotation.MoveTo(transform.rotation.eulerAngles);
        scale.MoveTo(transform.localScale);

        status = Highlight.free;
        suitType = SuitType.singleSuit;
        valueType = ValueType.doubleValue;
        cardMeshRender = highlight.GetComponent<MeshRenderer>();
        highlight.SetActive(false);

        // Set visual text quality
        foreach (TextMesh text in valueTexts)
        {
            text.characterSize = charSize_S;
            text.fontSize = fontSize_S;
        }
        foreach (TextMesh text in suitTexts)
        {
            text.characterSize = charSize_S;
            text.fontSize = fontSize_S;
        }
        suitTexts[2].characterSize = charSize_M;    // Central suit has different size
        suitTexts[2].fontSize = fontSize_M;

        timeToTwinkle = 0f;
    }

    void Update()
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = scale;

        switch (status)
        {
            case Highlight.free:
                highlight.SetActive(false);
                break;
            case Highlight.right:
                highlight.SetActive(true);
                cardMeshRender.material = rightCardMat;
                break;
            case Highlight.wrong:
                highlight.SetActive(true);
                cardMeshRender.material = wrongCardMat;
                break;
            case Highlight.contrast:
                highlight.SetActive(true);
                cardMeshRender.material = contrastMat;
                break;
        }

    }

    #region Time functions
    /// <summary>
    /// Highlights the card time based.
    /// </summary>
    /// <param name="delay">Delay before starts to twinkle.</param>
    /// <param name="deltaTime">Delta time between status change.</param>
    public void HighlightTimer(float delay, float deltaTime)
    {
        timeToTwinkle += Time.unscaledDeltaTime;

        if (timeToTwinkle > delay)
        if (timeToTwinkle > delay + deltaTime)
        if (timeToTwinkle > delay + 2f * deltaTime)
            timeToTwinkle = delay;
        else
            status = Card.Highlight.free;
        else
            status = Card.Highlight.contrast;
        else
            status = Card.Highlight.free;
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
            case (int)Suits.Diamond:
            case (int)Suits.Heart:
                color = (int)Colors.Red;
                break;
            case (int)Suits.Spades:
            case (int)Suits.Club:
                color = (int)Colors.Black;
                break;
            default:
                color = colorName.Length;
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
    /// Updates the infos based on a card.
    /// </summary>
    /// <param name="card">Card to be based on.</param>
    public void UpdateInfos(Card card)
    {
        UpdateInfos(card.suit, card.value, card.color);
    }

    /// <summary>
    /// Updates the suit types of the card.
    /// </summary>
    /// <param name="type">Suit type.</param>
    public void UpdateInfos(SuitType type)
    {
        suitType = type;
        switch (type)
        {
            case SuitType.noSuit:
                foreach (TextMesh text in suitTexts)
                {
                    text.gameObject.SetActive(false);
                }
                break;
            case SuitType.miniSuit:
                foreach (TextMesh text in suitTexts)
                {
                    if ((text.gameObject.name == "SuitUp") || (text.gameObject.name == "SuitDown"))
                        text.gameObject.SetActive(true);
                    else
                        text.gameObject.SetActive(false);
                }
                break;
            case SuitType.singleSuit:
                foreach (TextMesh text in suitTexts)
                {
                    if (suitTexts.IndexOf(text) > 2)
                        text.gameObject.SetActive(false);
                    else 
                        text.gameObject.SetActive(true);
                }
                while (suitTexts.Count > 3)
                {
                    suitTexts.RemoveAt(suitTexts.Count - 1);
                }
                suitTexts[2].transform.localPosition = Vector3.zero;
                suitTexts[2].transform.localScale = centralScale;
                break;
            case SuitType.multiSuit:
                foreach (TextMesh text in suitTexts)
                {
                    if ((text.gameObject.name == "SuitUp") || (text.gameObject.name == "SuitDown"))
                        text.gameObject.SetActive(false);
                    else
                        text.gameObject.SetActive(true);
                }
                goto default;
            case SuitType.complete:
                foreach (TextMesh text in suitTexts)
                    text.gameObject.SetActive(true);
                goto default;
            default:
                while (suitTexts.Count < 3 + value)
                {
                    GameObject aux = suitTexts[suitTexts.Count - 1].gameObject;
                    aux = Instantiate<GameObject>(aux, aux.transform);
                    aux.transform.parent = suitTexts[suitTexts.Count - 1].transform.parent;
                    aux.transform.name = "SuitCenter" + (suitTexts.Count - 2).ToString();

                    suitTexts.Add(aux.GetComponent<TextMesh>());
//                    suitTexts[suitTexts.Count - 1].transform.localPosition = Vector3.zero;
  //                  suitTexts[suitTexts.Count - 1].transform.localScale = new Vector3(0.04f, 0.03f, 0.4f);
                }

                List<Vector3> suitPos = new List<Vector3>();
                switch (value)
                {
                    case 0:
                        suitPos = SplitVertical(yPos, 0f, 1);
                        break;
                    case 1:
                        suitPos = SplitVertical(yPos, 0f, 2);
                        break;
                    case 2:
                        suitPos = SplitVertical(yPos, 0f, 3);
                        break;
                    case 3:
                        suitPos = SplitVertical(yPos, -xPos, 2);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 2));
                        break;
                    case 4:
                        suitPos = SplitVertical(yPos, -xPos, 2);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 2));
                        suitPos.AddRange(SplitVertical(yPos, 0f, 1));
                        break;
                    case 5:
                        suitPos = SplitVertical(yPos, -xPos, 3);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 3));
                        break;
                    case 6:
                        suitPos = SplitVertical(yPos, -xPos, 3);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 3));
                        suitPos.AddRange(SplitVertical(yPos / 2f, 0f, 2));
                        suitPos.RemoveAt(suitPos.Count - 1);
                        break;
                    case 7:
                        suitPos = SplitVertical(yPos, -xPos, 3);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 3));
                        suitPos.AddRange(SplitVertical(yPos / 2f, 0f, 2));
                        break;
                    case 8:
                        suitPos = SplitVertical(yPos, -xPos, 4);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 4));
                        suitPos.AddRange(SplitVertical(yPos, 0f, 1));
                        break;
                    case 9:
                        suitPos = SplitVertical(yPos, -xPos, 4);
                        suitPos.AddRange(SplitVertical(yPos, xPos, 4));
                        suitPos.AddRange(SplitVertical(yPos * 3f / 4f, 0f, 2));
                        break;
                }

                if (value < 10)
                    for (int iSuit = 0; iSuit <= value; iSuit++)
                    {
                        suitTexts[iSuit + 2].transform.localScale = scaleSuit * centralScale;
                        suitTexts[iSuit + 2].transform.localPosition = suitPos[iSuit];
                    }
                break;
        }
    }

    /// <summary>
    /// Updates the value types of the card.
    /// </summary>
    /// <param name="type">Value type.</param>
    public void UpdateInfos(ValueType type)
    {
        switch (type)
        {
            case ValueType.noValue:
                valueTexts[0].gameObject.SetActive(false);
                valueTexts[1].gameObject.SetActive(false);
                break;
            case ValueType.doubleValue:
                valueTexts[0].gameObject.SetActive(true);
                valueTexts[0].transform.localPosition = valuePos;
                valueTexts[0].transform.localScale = valueScale;

                valueTexts[1].gameObject.SetActive(true);
                valueTexts[1].transform.localPosition = -valuePos;
                valueTexts[1].transform.localScale = valueScale;
                break;
            case ValueType.bigValue:
                valueTexts[0].gameObject.SetActive(true);
                valueTexts[0].transform.localPosition = Vector3.zero;
                valueTexts[0].transform.localScale = centralScale;

                valueTexts[1].gameObject.SetActive(false);
                break;
        }
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
        switch ((Colors)color)
        {
            case Colors.Black:
                colorRGB = Color.black;
                break;
            case Colors.Red:
                colorRGB = Color.red;
                break;
            default:
                colorRGB = Color.blue;
                break;    
        }

        foreach (TextMesh text in valueTexts)
        {
            text.text = valueName;
            text.color = colorRGB;
        }


        foreach (TextMesh text in suitTexts)
        {
            text.text = suitSymbol;
            text.color = colorRGB;
        }

    }
    #endregion

    #region Operator definitions
    public static bool operator ==(Card cardA, Card cardB)
    {
        if ((((object)cardA) == null) || (((object)cardB) == null))
        {
            if ((((object)cardA) == null) && (((object)cardB) == null))
                return true;
            else
                return false;
        }

        if ((cardA.suit == cardB.suit) && (cardA.value == cardB.value) && (cardA.color == cardB.color))
            return true;
        else
            return false;
    }

    public static bool operator !=(Card cardA, Card cardB)
    {
        if ((((object)cardA) == null) || (((object)cardB) == null))
        {
            if ((((object)cardA) == null) && (((object)cardB) == null))
                return false;
            else
                return true;
        }

        if ((cardA.suit != cardB.suit) || (cardA.value != cardB.value) || (cardA.color != cardB.color))
            return true;
        else
            return false;
    }


    public override bool Equals(object cardO)
    {
        try 
        {
            return (this == (Card)cardO);
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        if (this == null)
            return "Empty Card";
        else
            return "Card " + valueNames[value] + suitNames[suit] + " " + colorNames[color];
    }

    /// <param name="mode">0 = suit as names / 1 = suit as symbols</param>
    public string ToString(int mode)
    {
        switch (mode)
        {
            case 0:
            default:
                return ToString();
            case 1:
                return "Card " + valueNames[value] + suitSymbols[suit] + " " + colorNames[color];
        }
    }

    /// <summary>
    /// Serves as a hash function for a Card object ordering based first in value then in suit.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
    public override int GetHashCode()
    {
        return (suit + value * suitName.Length);
    }

    #endregion

}
