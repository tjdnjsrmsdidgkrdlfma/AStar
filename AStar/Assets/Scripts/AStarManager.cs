using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    #region 노드
    public enum NodeState
    {
        None,
        Wall,
        Start,
        End
    }

    [Header("노드")]
    [SerializeField] GameObject node;

    NodeState click_state = NodeState.None;
    #endregion

    #region 노드 정보
    float[] x_position = new float[2]; //-8.5 ~ 8.5
    float[] y_position = new float[2]; //-4.5 ~ 4.5

    Node[,] node_list = new Node[10, 18];
    Node start;
    Node end;

    Vector2 map_size = new Vector2(10, 18);
    #endregion

    #region 기타
    [Header("기타")]
    [SerializeField] GameObject mouse_position;
    #endregion

    void Awake()
    {
        x_position[0] = -8.5f;
        x_position[1] = 8.5f;

        y_position[0] = -4.5f;
        y_position[1] = 4.5f;

        start = null;
        end = null;
    }

    void Start()
    {
        SpawnNode();
    }

    void SpawnNode()
    {
        for (float y = y_position[0]; y <= y_position[1]; y++)
        {
            for (float x = x_position[0]; x <= x_position[1]; x++)
            {
                GameObject temp = Instantiate(node, new Vector2(x, y), Quaternion.identity);
                node_list[(int)(y + map_size.x / 2), (int)(x + map_size.y / 2)] = temp.GetComponent<Node>();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            FindingPath();

        if (Input.GetKeyDown(KeyCode.Alpha0))
            click_state = NodeState.None;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            click_state = NodeState.Wall;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            click_state = NodeState.Start;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            click_state = NodeState.End;

        if (Input.GetMouseButtonDown(0))
        {
            SetNodeState();
        }
    }

    void FindingPath()
    {

    }

    void SetNodeState()
    {
        Vector2 click_position = new Vector2();
        click_position.x = mouse_position.transform.position.x;
        click_position.y = mouse_position.transform.position.y;

        click_position.x = click_position.x + (map_size.y / 2);
        click_position.y = click_position.y + (map_size.x / 2);

        if (click_position.x < map_size.y && click_position.x > -map_size.y &&
            click_position.y < map_size.x && click_position.y > -map_size.x)
        {
            switch (click_state)
            {
                case NodeState.None:
                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.Start)
                        start = null;
                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.End)
                        end = null;

                    node_list[(int)click_position.y, (int)click_position.x].State = NodeState.None;

                    break;
                case NodeState.Wall:
                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.Start)
                        start = null;
                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.End)
                        end = null;

                    node_list[(int)click_position.y, (int)click_position.x].State = NodeState.Wall;

                    break;
                case NodeState.Start:
                    if (start != null)
                        start.State = NodeState.None;
                    start = node_list[(int)click_position.y, (int)click_position.x];

                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.End)
                        end = null;

                    node_list[(int)click_position.y, (int)click_position.x].State = NodeState.Start;

                    break;
                case NodeState.End:
                    if (end != null)
                        end.State = NodeState.None;
                    end = node_list[(int)click_position.y, (int)click_position.x];

                    if (node_list[(int)click_position.y, (int)click_position.x].State == NodeState.Start)
                        start = null;

                    node_list[(int)click_position.y, (int)click_position.x].State = NodeState.End;

                    break;
            }
        }
    }
}