using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Choice represents a unique player's choice.
/// </summary>
public class Choice {

    public static int suitCounter, valueCounter, colorCounter = 0;
    public static int totalPoints = 0;
    public static int orderCounter = 0;
    public static float precision = 0f;

    public string objectiveCard = "";
    public string choiceCard = "";
    public bool suitMatch, valueMatch, colorMatch;
    public int pointMatch;
    public int order;
    public int numOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="Choice"/> class comparing the card chosen with objective card and the number of options.
    /// </summary>
    /// <param name="objective">Objective card.</param>
    /// <param name="choice">Chosen card.</param>
    /// <param name="nOptions">Number of card options.</param>
    public Choice(Card objective, Card choice, int nOptions)
    {
        pointMatch = 0;
        objectiveCard = objective.ToString(0);
        choiceCard = choice.ToString(0);

        suitMatch = objective.suit == choice.suit;
        valueMatch = objective.value == choice.value;
        colorMatch = objective.color == choice.color;

        if (suitMatch)
        {
            pointMatch += 10;
            suitCounter++;
        }
        if (valueMatch)
        {
            pointMatch += 15;
            valueCounter++;
        }
        if (colorMatch)
        {
            pointMatch += 5;
            colorCounter++;
        }

        order = orderCounter;
        orderCounter++;
        totalPoints += pointMatch;
    }

    public Choice () 
    {
        objectiveCard = choiceCard = "null";
        pointMatch = 0;

        suitMatch = false;
        valueMatch = false;
        colorMatch = false;
    }

    public static bool operator ==(Choice A, Choice B)
    {
        if ((A.suitMatch == B.suitMatch) && (A.valueMatch == B.valueMatch) && (A.colorMatch == B.colorMatch))
            return true;
        else
            return false;
    }

    public static bool operator !=(Choice A, Choice B)
    {
        if ((A.suitMatch != B.suitMatch) || (A.valueMatch != B.valueMatch) || (A.colorMatch != B.colorMatch))
            return true;
        else
            return false;
    }

    public static implicit operator int(Choice choice)
    {
        return choice.pointMatch;
    }

    public static implicit operator bool(Choice choice)
    {
        if (choice.pointMatch == 30)
            return true;
        else
            return false;
    }

    public override bool Equals(object choice)
    {
        try 
        {
            return (this == (Choice)choice);
        }
        catch
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return (pointMatch);
    }
}
