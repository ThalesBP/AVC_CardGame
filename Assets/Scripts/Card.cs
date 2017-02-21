using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the card behaviour and information individually.
/// </summary>
public class Card : GameBase {

    public enum Status {free, right, wrong};

    #region Card Infos
    public string suitName, suitSymbol, valueName, colorName;   // This may be useless
    public int suit, value, color;  // This card infos
    public Color colorRGB;
    #endregion

    #region Design Infos
    [HideInInspector]
    public List<TextMesh> valueTexts, suitTexts;    // Text in 3D cards
    [HideInInspector]
    public GameObject highlight;
    [HideInInspector]
    public Material wrongCardMat, rightCardMat;
    public Status status;
    private MeshRenderer cardMeshRender;
    #endregion

    #region Motion's variables
    public Motion position, rotation, scale;
    #endregion

    void Awake()
    {
        // Initialize variables
        position = gameObject.AddComponent<Motion>();
        rotation = gameObject.AddComponent<Motion>();
        scale = gameObject.AddComponent<Motion>();
        suit = value = color = 0;

        position.MoveTo(transform.position);
        rotation.MoveTo(transform.rotation.eulerAngles);
        scale.MoveTo(transform.localScale);

        status = Status.free;
        cardMeshRender = highlight.GetComponent<MeshRenderer>();
        highlight.SetActive(false);

        // Set visual text quality
        foreach (TextMesh value in valueTexts)
        {
            value.characterSize = charSize_S;
            value.fontSize = fontSize_S;
        }
        foreach (TextMesh value in suitTexts)
        {
            value.characterSize = charSize_S;
            value.fontSize = fontSize_S;
        }
        suitTexts[2].characterSize = charSize_M;    // Central suit has different size
        suitTexts[2].fontSize = fontSize_M;
    }

    void Update()
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = scale;

        switch (status)
        {
            case Status.free:
                highlight.SetActive(false);
                break;
            case Status.right:
                highlight.SetActive(true);
                cardMeshRender.material = rightCardMat;
                break;
            case Status.wrong:
                highlight.SetActive(true);
                cardMeshRender.material = wrongCardMat;
                break;
        }
    }

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

    public void UpdateInfos(Card card)
    {
        UpdateInfos(card.suit, card.value, card.color);
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

        if ((cardA.suit != cardB.suit) || (cardA.value != cardB.value) || (cardA.color == cardB.color))
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
    /// Serves as a hash function for a Card object ordering based first in suit then in value.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
    public override int GetHashCode()
    {
        return (suit * valueName.Length + value);
    }

    #endregion

}
