using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    Node parent_node;
    public Node ParentNode
    {
        get { return parent_node; }
        set
        {
            parent_node = value;

            if(Mathf.Approximately(transform.position.y + 1, ParentNode.transform.position.y))
            {
                sprite_renderer.sprite = up_arrow_image;
            }
            else if (Mathf.Approximately(transform.position.y - 1, ParentNode.transform.position.y))
            {
                sprite_renderer.sprite = down_arrow_image;
            }
            else if (Mathf.Approximately(transform.position.x - 1, ParentNode.transform.position.x))
            {
                sprite_renderer.sprite = left_arrow_image;
            }
            else if (Mathf.Approximately(transform.position.x + 1, ParentNode.transform.position.x))
            {
                sprite_renderer.sprite = right_arrow_image;
            }
        }
    }

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
                    sprite_renderer.color = Color.green;
                    break;
            }
        }
    }

    SpriteRenderer sprite_renderer;
    public static Sprite up_arrow_image;
    public static Sprite down_arrow_image;
    public static Sprite left_arrow_image;
    public static Sprite right_arrow_image;

    void Awake()
    {
        g = 0;
        h = 0;
        f = 0;

        sprite_renderer = GetComponent<SpriteRenderer>();
    }
}
