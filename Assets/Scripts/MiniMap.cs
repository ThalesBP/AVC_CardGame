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

    //public AnkleMovement package;

    private List<Vector2> ankleTrack;
    private float recStep;
    private float recTime;

    private float colorRate, colorAlpha;

    public List<Vector2> choices, challenges;

    void Start() 
    {
        ankleTrack = new List<Vector2> ();
        colorRate = 0.1f;
        colorAlpha = 0.3f;
        recTime = 0f;
        recStep = 0.5f;

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
        point = origin + Vector2.Scale (ControlManager.Instance.ankle.origin, size);
        ElipseForm (point, Vector2.Scale(ControlManager.Instance.ankle.bases * scale, size));
        ElipseForm (point, Vector2.Scale(ControlManager.Instance.ankle.bases * scale, size)/ControlManager.Instance.ankle.elipseScale);
        CrossForm (point, 0.1f*size);


        // Black Dot for position
        GL.Color(Color.black);
        point = origin + Vector2.Scale (ControlManager.Instance.RawPosition * scale, size);
        ElipseForm (point, 0.02f*size);
        ElipseForm (point, 0.03f*size);
        CrossForm (point, 0.02f*size);

        // Green Lines for helper
        /*      GL.Color(new Color(0.0f, 0.4f, 0.0f, 1.0f));
        if (package.elipseSpace)
            ElipseForm (origin + Vector2.Scale (package.centerSpring, size), Vector2.Scale (package.freeSpace, size));
        else
            RectForm (origin + Vector2.Scale (package.centerSpring - new Vector2(package.freeSpace.x, -package.freeSpace.y), size), Vector2.Scale (package.freeSpace, size) * 2);
        CrossForm (origin + Vector2.Scale (package.centerSpring, size), 0.02f*size);
*/
        // Red Dot for choices

        for (int i = 0; i < choices.Count; i++)
        {
            GL.Color(Color.red);
            ElipseForm(origin + Vector2.Scale(choices[i] * scale, size), 0.02f * size);
            CrossForm(origin + Vector2.Scale(choices[i] * scale, size), 0.02f * size);
        }
        for (int i = 0; i < challenges.Count; i++)
        {
            GL.Color(Color.green);
            ElipseForm(origin + Vector2.Scale(challenges[i] * scale, size), 0.01f * size);
            CrossForm(origin + Vector2.Scale(challenges[i] * scale, size), 0.01f * size);

            Line(origin + Vector2.Scale(choices[i] * scale, size),
                origin + Vector2.Scale(challenges[i] * scale, size));
        }
        // Record the track
        if (recTime <= 0f)
        {
            ankleTrack.Add (point - origin);
            recTime = recStep;
        } else
            recTime -= Time.deltaTime;

        // Black changing alpha for track
        GL.Color (Color.black);
        if (ankleTrack.Count > 1)
        {
            float aux = 1f;
            for (int i = ankleTrack.Count - 1; i > 0 ; i--)
            {
                GL.Color(new Color (0f, 0f, 0f, aux));
                Line(ankleTrack[i - 1] + origin, ankleTrack[i] + origin);
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

    void ElipseForm(Vector2 center, Vector2 sizes)
    {
        center = new Vector2 (
            center.x / Screen.width,
            center.y / Screen.height);
        sizes = new Vector2 (
            sizes.x / Screen.width,
            sizes.y / Screen.height);

        for (int i = 0; i < 360; i++)
        {
            GL.Vertex(center + new Vector2(
                Mathf.Sin(i * Mathf.Deg2Rad) * (sizes.x), 
                Mathf.Cos(i * Mathf.Deg2Rad) * (sizes.y)));
            GL.Vertex(center + new Vector2(
                Mathf.Sin((i + 1) * Mathf.Deg2Rad) * (sizes.x), 
                Mathf.Cos((i + 1) * Mathf.Deg2Rad) * (sizes.y)));
        }
    }

}
