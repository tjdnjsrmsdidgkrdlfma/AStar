using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{   
    int g;
    public int G
    {
        get { return g; }
        set
        {
            g = value;
            g_text.text = g.ToString();
        }
    }
    int h;
    public int H
    {
        get { return h; }
        set
        {
            h = value;
            h_text.text = h.ToString();
        }
    }
    int f;
    public int F
    {
        get { return f; }
        set
        {
            f = value;
            f_text.text = f.ToString();
        }
    }

    public Node parent_node;

    [SerializeField] TextMeshPro g_text;
    [SerializeField] TextMeshPro h_text;
    [SerializeField] TextMeshPro f_text;

    AStarManager.NodeState state = AStarManager.NodeState.None;
    public AStarManager.NodeState State
    {
        get { return state; }
        set
        {
            state = value;

            switch (state)
            {
                case AStarManager.NodeState.None:
                    sprite_renderer.color = Color.white;
                    break;
                case AStarManager.NodeState.Wall:
                    sprite_renderer.color = Color.black;
                    break;
                case AStarManager.NodeState.Start:
                    sprite_renderer.color = Color.red;
                    break;
                case AStarManager.NodeState.End:
                    sprite_renderer.color = Color.blue;
                    break;
            }
        }
    }

    SpriteRenderer sprite_renderer;

    void Awake()
    {
        g = 0;
        h = 0;
        f = 0;

        sprite_renderer = GetComponent<SpriteRenderer>();
    }
}
