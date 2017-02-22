using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Works as common base for all classes in the game.
/// </summary>
/// <remarks>
/// Here contents all information that are going to be used along the development and can make the changes easier.
/// </remarks>
public class GameBase : MonoBehaviour {

    #region Suit of cards, cards and player's position

    protected static readonly string[] suitNames = {"Diamond", "Spades", "Heart", "Club"};
    protected static readonly string[] suitSymbols = {"♦", "♠", "♥", "♣"};
    protected static readonly string[] valueNames = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    protected static readonly string[] colorNames = { "Red", "Black"};

    // Check if this is useful
    public enum Suits {Diamond, Spades, Heart, Club};
    public enum Values {A = 0, J = 11, Q = 12, K = 13};
    public enum Colors {Red, Black};

    #endregion

    #region Card design informations
    protected static readonly float refSize = 3f;   // Ratio between charSize and fontSize to keep global size
    protected static int fontSize_M = 100;          // fontSize and charSize for Medium size
    protected static float charSize_M = refSize / fontSize_M;
    protected static int fontSize_S = 60;           // fontSize and charSize for Small size
    protected static float charSize_S =  refSize / fontSize_S;
    #endregion

    #region Delta time informations
    protected static readonly float[] DeltaTime = { 0.1f, 0.25f, 0.5f, 0.75f, 1f };
    protected static readonly int VeryShort = 0;
    protected static readonly int Short = 1;
    protected static readonly int Medium = 2;
    protected static readonly int Long = 3;
    protected static readonly int VeryLong = 4;
    #endregion

    #region Position, rotation and scale of cards on screen
    static protected float backgndDist = 0.4f;
    static protected float cardThick = 0.01f;       // Card's thickness
    static protected float spreadRadius = 3f;       // The radius that cards are spread
    #endregion

    #region Other informations
    #endregion

    #region Public Variables    // Variables to be changed realtime
    public int fontSizeAdjust_S = 60;
    public int fontSizeAdjust_M = 100;
    #endregion

    /// <summary>
    /// It is only used to update static variable from Unity's Inspector
    /// </summary>
    void Update()
    {
        fontSize_M = fontSizeAdjust_M;
        charSize_M = refSize / fontSize_M;
        fontSize_S = fontSizeAdjust_S;
        charSize_S = refSize / fontSize_S;
    }

    /// <summary>
    /// Sorts a deck.
    /// </summary>
    /// <returns>The deck sorted.</returns>
    /// <param name="cardlist">The deck to be sorted.</param>
    private List<GameObject> SortCards(List<GameObject> cardlist)
    {
        GameObject aux;
        List<GameObject> finaldeck = new List<GameObject>();

        for (int j = 0; j < suitNames.Length; j++)
        {
            for (int i = 0; i < valueNames.Length; i++)
            {
                if ((aux = FindCard(cardlist, "Card_" + valueNames[i] + "_" + suitNames[j])) != null)
                    finaldeck.Add(aux);
            }
        }
        return finaldeck;
    }

    /// <summary>
    /// Finds a card in a deck.
    /// </summary>
    /// <returns>The card.</returns>
    /// <param name="pack">The deck where to find.</param>
    /// <param name="name">The card to be find.</param>
    protected GameObject FindCard(GameObject[] pack, string name)
    {
        for (int i = 0; i < pack.Length; i++) 
        {
            if (string.Compare (pack [i].name, name) == 0)
                return pack [i];
        }
        return null;
    }

    /// <summary>
    /// Finds a card in a deck.
    /// </summary>
    /// <returns>The card.</returns>
    /// <param name="pack">The deck where to find.</param>
    /// <param name="name">The card to be find.</param>
    protected GameObject FindCard(List<GameObject> pack, string name)
    {
        for (int i = 0; i < pack.Count; i++) 
        {
            if (string.Compare (pack [i].name, name) == 0)
                return pack [i];
        }
        return null;
    }
}
