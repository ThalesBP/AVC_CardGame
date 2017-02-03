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
    
    public enum suits {Diamond, Spades, Heart, Club};
    public enum values {A = 0, J = 11, Q = 12, K = 13};
    public enum colors {Red, Black};

    #endregion

    #region Card design informations
    static protected float charSize = 0.03f;
    static protected int fontSize = 100;
    static protected float refSize = charSize * fontSize;
    #endregion

    #region Position, rotation and scale of cards on screen
    #endregion

    #region Other informations
    #endregion

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
