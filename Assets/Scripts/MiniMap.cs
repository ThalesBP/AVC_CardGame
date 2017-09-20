using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {

    public Material mat;
    public Vector2 startVertex;
    public Vector2 mousePos;

    public RectTransform space;

    public Vector2 origin, size, point;
    public float scale = 0.75f;

    public List<Vector2> choices, challenges, ankleTrack;
//    private float recStep;
//    private float recTime;

    private float colorRate, colorAlpha, auxAlpha = 1f;

    void Start() 
    {
        ankleTrack = new List<Vector2> ();
        colorRate = 0.1f;
        colorAlpha = 0.3f;
//        recTime = 0f;
//        recStep = 0.25f;

        //        package = ControlManager.Instance.ankle;

        space = GameObject.Find("MiniMap").GetComponent<RectTransform>();

        choices = new List<Vector2>();
        challenges = new List<Vector2>();
    }

    void OnGUI() 
    {
        if (!mat) 
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }   
        // Start Graph plot
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);

        // Scale of space plot
        size = new Vector2 (
            space.rect.width * space.lossyScale.x,
            space.rect.height * space.lossyScale.y);

        // Center of space plot
        origin = space.position; // + size / 2;
        origin += Vector2.Scale(size, (Vector2.right + Vector2.down)) / 2;

        // Black Rectangle for space
        GL.Color(Color.black);
        RectForm (space.position, size);
        CrossForm (origin, size);

        // Blue Elipse for package
        GL.Color(Color.blue);
        point = origin + Vector2.Scale (ControlManager.Instance.ankle.origin * scale, size);
        ElipseForm (point, Vector2.Scale(ControlManager.Instance.ankle.bases * scale, size), 180);
        ElipseForm (point, Vector2.Scale(ControlManager.Instance.ankle.bases * scale, size)/ControlManager.Instance.ankle.elipseScale, 180);
        CrossForm (point, 0.1f*size);


        // Black Dot for position
        GL.Color(Color.black);
        point = origin + Vector2.Scale (ControlManager.Instance.RawPosition * scale, size);
        ElipseForm (point, 0.02f*size, 18);
        ElipseForm (point, 0.03f*size, 18);
        CrossForm (point, 0.02f*size);

        // Green Lines for helper
        GL.Color(new Color(0.0f, 0.4f, 0.0f, 1.0f));
        switch (ControlManager.Instance.helper)
        {
            case ControlManager.HelperMode.GoIn:
                auxAlpha = GameBase.MaxStiffness;
                GL.Color(new Color(0.0f, 0.4f, 0.0f, ControlManager.Instance.impedance.x / auxAlpha));
                point = origin + Vector2.Scale(ControlManager.Instance.centerSpring * scale, size);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.freeSpace * scale, size), 18);
                CrossForm(point, Vector2.Scale(ControlManager.Instance.freeSpace * scale, size));
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.outFreeSpace * scale, size), 18);
                break;
            case ControlManager.HelperMode.GoOut:
                auxAlpha = GameBase.MaxAntiFriction;
                GL.Color(new Color(0.0f, 0.4f, 0.0f, ControlManager.Instance.impedance.x / auxAlpha));
                point = origin + Vector2.Scale(ControlManager.Instance.ankle.CircleToElipse(Vector2.zero, Screen.height * 0.45f) * scale, size);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.centerSpring.x * ControlManager.Instance.ankle.bases * scale, size), 18);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.centerSpring.y * ControlManager.Instance.ankle.bases * scale, size), 18);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.freeSpace.x * ControlManager.Instance.ankle.bases * scale, size), 18);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.freeSpace.y * ControlManager.Instance.ankle.bases * scale, size), 18);
                CrossForm(point, Vector2.Scale(ControlManager.Instance.centerSpring.x * ControlManager.Instance.ankle.bases * scale, size));
                break;
            case ControlManager.HelperMode.None:
                GL.Color(new Color(0.0f, 0.4f, 0.0f, ControlManager.Instance.impedance.x / auxAlpha));
                point = origin + Vector2.Scale(ControlManager.Instance.centerSpring * scale, size);
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.freeSpace * scale, size), 18);
                CrossForm(point, Vector2.Scale(ControlManager.Instance.freeSpace * scale, size));
                ElipseForm(point, Vector2.Scale(ControlManager.Instance.outFreeSpace * scale, size), 18);
                break;
        }

        // Green Dot for choices
        for (int i = 0; i < choices.Count; i++)
        {
            GL.Color(Color.green);
            ElipseForm(origin + Vector2.Scale(choices[i] * scale, size) * scale, 0.01f * size, 12);
            CrossForm(origin + Vector2.Scale(choices[i] * scale, size) * scale, 0.01f * size);

            Line(origin + Vector2.Scale(choices[i] * scale, size) * scale,
                origin + Vector2.Scale(challenges[i] * scale, size) * scale);
        }

        // Red Dot for challenges
        for (int i = 0; i < challenges.Count; i++)
        {
            GL.Color(Color.red);
            ElipseForm(origin + Vector2.Scale(challenges[i] * scale, size), 0.02f * size, 12);
            CrossForm(origin + Vector2.Scale(challenges[i] * scale, size), 0.02f * size);
        }

        // Black changing alpha for track
        GL.Color (Color.black);
        if (ankleTrack.Count > 1)
        {
            float aux = 1f;
            for (int i = ankleTrack.Count - 1; i > 0 ; i--)
            {
                GL.Color(new Color (0f, 0f, 0f, aux));
                Line(origin + Vector2.Scale(ankleTrack[i - 1] * scale, size), origin + Vector2.Scale(ankleTrack[i] * scale, size));
                if (aux > colorAlpha)
                    aux -= colorRate;
            }
        }

        GL.End();
        GL.PopMatrix();

        //      if (package.enemy.GameStatus () == 4)
        //          ankleTrack.Clear ();
    }

    void Line(Vector2 start, Vector2 end)
    {
        Vector2 v3start = new Vector2 (
            start.x / Screen.width, 
            start.y / Screen.height);

        Vector3 v3end = new Vector2 (
            end.x / Screen.width, 
            end.y / Screen.height);

        GL.Vertex(v3start);
        GL.Vertex(v3end);


    }

    void RectForm(Vector2 startEdge, Vector2 sizes)
    {
        startEdge = new Vector2 (
            startEdge.x / Screen.width, 
            startEdge.y / Screen.height);

        sizes = new Vector2 (
            sizes.x / Screen.width,
            sizes.y / Screen.height);

        GL.Vertex(startEdge);
        GL.Vertex(startEdge + new Vector2(sizes.x, 0));
        GL.Vertex(startEdge + new Vector2(sizes.x, 0));
        GL.Vertex(startEdge + new Vector2(sizes.x, -sizes.y));
        GL.Vertex(startEdge + new Vector2(sizes.x, -sizes.y));
        GL.Vertex(startEdge + new Vector2(0, -sizes.y));
        GL.Vertex(startEdge + new Vector2(0, -sizes.y));
        GL.Vertex(startEdge);
    }

    void CrossForm (Vector2 center, Vector2 sizes)
    {
        center = new Vector2 (
            center.x / Screen.width, 
            center.y / Screen.height);
        sizes = new Vector2 (
            sizes.x /2 / Screen.width,
            sizes.y /2 / Screen.height);

        GL.Vertex(center + new Vector2(sizes.x, 0));    
        GL.Vertex(center + new Vector2(-sizes.x, 0));   
        GL.Vertex(center + new Vector2(0, sizes.y));    
        GL.Vertex(center + new Vector2(0, -sizes.y));   
    }

    void ElipseForm(Vector2 center, Vector2 sizes, int steps)
    {
        center = new Vector2 (
            center.x / Screen.width,
            center.y / Screen.height);
        sizes = new Vector2 (
            sizes.x / Screen.width,
            sizes.y / Screen.height);

        float angStep = 2f * Mathf.PI / steps;

        for (int i = 0; i < steps; i++)
        {
            GL.Vertex(center + new Vector2(
                Mathf.Sin(i * angStep) * (sizes.x), 
                Mathf.Cos(i * angStep) * (sizes.y)));
            GL.Vertex(center + new Vector2(
                Mathf.Sin((i + 1) * angStep) * (sizes.x), 
                Mathf.Cos((i + 1) * angStep) * (sizes.y)));
        }
    }

    public void Reset()
    {
        choices.Clear();
        challenges.Clear();
        ankleTrack.Clear();
    }
}
