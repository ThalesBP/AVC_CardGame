using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager {

    public float[] rehabPlan = { 0.25f, 0.25f, 0.25f, 0.25f };
    public float[] rehabStat = { 0.0f, 0.0f, 0.0f, 0.0f };
    public List<int> rehabStory = new List<int>();

    public float[] Plan
    {
        get
        {
            return rehabPlan;
        }
        set
        {
            AdaptPlan(value);
        }
    }

    public int Size
    {
        get 
        { 
            return rehabPlan.Length;
        }
    }

    public int Challenge
    {
        get 
        {
            return GetChallenge();
        }
    }

    public ChallengeManager (float [] challenge)
    {
        AdaptPlan(challenge);
    }

    /// <summary>
    /// Adapts the plan.
    /// </summary>
    /// <param name="plan">The new plan.</param>
    private void AdaptPlan(float[] plan)
    {
        if (plan.Length == rehabPlan.Length)
        {
            float sum = 0f;
            foreach (float value in plan)
                sum += value;
            for (int i = 0; i < rehabPlan.Length; i++)
                rehabPlan[i] = plan[i] / sum;
        }
        else
        {
            rehabPlan = plan;
            rehabStat = new float[plan.Length];
            for (int i = 0; i < rehabStat.Length; i++)
                rehabStat[i] = 0f;
            rehabStory.Clear();
        }
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

        rehabStory.Add(Min(diff));

        for (int index = 0; index < rehabStat.Length; index++)
        {
            rehabStat[index] = 1f * CountIndex(rehabStory, index) / rehabStory.Count;
        }

        return rehabStory[rehabStory.Count - 1];
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
    public int Max(float[] list)
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
}