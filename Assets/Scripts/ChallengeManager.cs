using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager {

    public float[] rehabPlan = { 0.25f, 0.25f, 0.25f, 0.25f };
    public float[] rehabStat = { 0.0f, 0.0f, 0.0f, 0.0f };
    public List<int> rehabStory = new List<int>();

    /// <summary>
    /// Challenge plan
    /// </summary>
    public float[] Plan
    {
        get
        {
            return rehabPlan;
        }
        set
        {
            NewPlan(value);
        }
    }

    /// <summary>
    /// Challenge statistics
    /// </summary>
    public float[] Done
    {
        get
        {
            return rehabStat;
        }
    }

    /// <summary>
    /// The plna size
    /// </summary>
    public int Size
    {
        get 
        { 
            return rehabPlan.Length;
        }
        set
        {
            NewPlan(value);
        }
    }

    /// <summary>
    /// Gets a challenge.
    /// </summary>
    public int Challenge
    {
        get 
        {
            return GetChallenge();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChallengeManager"/> class.
    /// </summary>
    /// <param name="challenge">Challenge plan.</param>
    public ChallengeManager (float [] challenge)
    {
        NewPlan(challenge);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChallengeManager"/> class.
    /// </summary>
    /// <param name="number">Number of challenges.</param>
    public ChallengeManager (int number)
    {
        NewPlan(number);
    }

    /// <summary>
    /// Adapts the plan.
    /// </summary>
    /// <param name="plan">The new plan.</param>
    private void NewPlan(float[] plan)
    {
        rehabPlan = new float[plan.Length];
        float sum = 0f;

        foreach (float value in plan)
            sum += value;

        for (int i = 0; i < rehabPlan.Length; i++)
            if (sum == 0)
                rehabPlan[i] = 1f / rehabPlan.Length;
            else 
                rehabPlan[i] = plan[i] / sum;
    }

    /// <summary>
    /// Adapts the plan.
    /// </summary>
    /// <param name="number">Number of options.</param>
    private void NewPlan(int number)
    {
        rehabPlan = new float[number];
        rehabStat = new float[number];
        for (int i = 0; i < number; i++)
        {
            rehabPlan[i] = 1f / number;
            rehabStat[i] = 0f;
        }
        rehabStory.Clear();
    }

    /// <summary>
    /// Gets the next challenge index.
    /// </summary>
    private int GetChallenge()
    {
        float[] diff = new float[rehabStat.Length];

        for (int i = 0; i < diff.Length; i++)
        {
            diff[i] = rehabStat[i] - rehabPlan[i];
        }

        return Min(diff);
    }

    /// <summary>
    /// Adds a choice.
    /// </summary>
    public void AddChoice(int choice)
    {
        rehabStory.Add(choice);

        for (int index = 0; index < rehabStat.Length; index++)
        {
            rehabStat[index] = 1f * CountIndex(rehabStory, index) / rehabStory.Count;
        }
    }

    /// <summary>
    /// Counts the index frequence.
    /// </summary>
    /// <returns>How many times the index appears in the list.</returns>
    /// <param name="list">List to be counted.</param>
    /// <param name="index">Index wished.</param>
    private int CountIndex(List<int> list, int index)
    {
        int count = 0;
        foreach (int value in list)
        {
            if (value == index)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Retunr the min element's index.
    /// </summary>
    private int Min(float[] list)
    {
        float extreme = 0f;
        List<int> index = new List<int>();

        extreme = float.MaxValue;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] < extreme)
            {
                extreme = list[i];
                index.Clear();
                index.Add(i);
            }
            else
            {
                if (list[i] == extreme)
                    index.Add(i);
            }
        }
        return index[Random.Range(0, index.Count)];
    }

    /// <summary>
    /// Retunr the max element's index.
    /// </summary>
    private int Max(float[] list)
    {
        float extreme = 0f;
        List<int> index = new List<int>();

        extreme = float.MinValue;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] > extreme)
                {
                    extreme = list[i];
                    index.Clear();
                    index.Add(i);
                }
                else
                {
                    if (list[i] == extreme)
                        index.Add(i);
                }
            }
        return index[Random.Range(0, index.Count)];
    }

    public void Reset()
    {
        for (int index = 0; index < rehabStat.Length; index++)
        {
            rehabStat[index] = 0f;
        }
        rehabStory.Clear();
    }
}