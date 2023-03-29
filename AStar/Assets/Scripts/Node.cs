using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float g;
    public float h;
    public float f;

    public Node parent_node;

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
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
}
