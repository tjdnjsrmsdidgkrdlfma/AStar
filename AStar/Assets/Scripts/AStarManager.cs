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

    Vector2[] direction = new Vector2[4];
    Vector2 map_size = new Vector2(10, 18);
    #endregion

    #region 기타
    [Header("기타")]
    [SerializeField] GameObject mouse_position;
    #endregion

    List<Node> opened_list = new List<Node>();
    List<Node> closed_list = new List<Node>();

    void Awake()
    {
        x_position[0] = -8.5f;
        x_position[1] = 8.5f;

        y_position[0] = -4.5f;
        y_position[1] = 4.5f;

        start = null;
        end = null;

        direction[0] = new Vector2(0, 1);
        direction[1] = new Vector2(1, 0);
        direction[2] = new Vector2(0, -1);
        direction[3] = new Vector2(-1, 0);
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
            StartCoroutine(FindingPath());

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

    IEnumerator FindingPath()
    {
        opened_list.Add(start);

        Vector2 node_index = GetIndex(start);

        for (int i = 0; i < direction.Length; i++)
        {
            Node node = node_list[(int)node_index.y + (int)direction[i].y, (int)node_index.x + (int)direction[i].x];

            if (node.State != NodeState.Wall)
            {
                opened_list.Add(node);
                node.parent_node = start;
                node.G = 1;
                node.H = (int)Mathf.Abs(node.transform.position.x - end.transform.position.x)
                         + (int)Mathf.Abs(node.transform.position.y - end.transform.position.y);
                node.F = node.G + node.H;
                node.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }

        opened_list.Remove(start);
        closed_list.Add(start);

        Node smallest_f_value_node;

        while (true)
        {
            if (opened_list.Count == 0 || opened_list.Contains(end) == true)
                break;

            smallest_f_value_node = opened_list[0];

            foreach (Node node in opened_list)
            {
                if (node.F < smallest_f_value_node.F)
                {
                    smallest_f_value_node = node;
                }
            }

            yield return new WaitForSeconds(0.5f);

            smallest_f_value_node.GetComponent<SpriteRenderer>().color = Color.green;
            opened_list.Remove(smallest_f_value_node);
            closed_list.Add(smallest_f_value_node);

            node_index = GetIndex(smallest_f_value_node);

            for (int i = 0; i < direction.Length; i++)
            {
                Node node = node_list[(int)node_index.y + (int)direction[i].y, (int)node_index.x + (int)direction[i].x];

                if (node.State == NodeState.Wall
                        || closed_list.Contains(node) == true)
                    continue;

                node.GetComponent<SpriteRenderer>().color = Color.yellow;
                int temp_g = smallest_f_value_node.G + 1;
                int temp_h = (int)Mathf.Abs(node.transform.position.x - end.transform.position.x)
                             + (int)Mathf.Abs(node.transform.position.y - end.transform.position.y);
                int temp_f = temp_g + temp_h;

                if (opened_list.Contains(node) == true)
                {
                    if (node.G > temp_g)
                    {
                        node.parent_node = smallest_f_value_node;
                        node.G = temp_g;
                        node.H = temp_h;
                        node.F = temp_f;
                    }
                }
                else
                {
                    opened_list.Add(node);
                    node.parent_node = smallest_f_value_node;
                    node.G = temp_g;
                    node.H = temp_h;
                    node.F = temp_f;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        //Stack<Node> node_stack = new Stack<Node>();
        //Node temp_node = end;

        //while (true)
        //{
        //    node_stack.Push(temp_node.parent_node);
        //    temp_node = temp_node.parent_node;
        //    if (temp_node == null)
        //        break;
        //}

        //while (node_stack.Count > 0)
        //{
        //    temp_node = node_stack.Pop();
        //    temp_node.GetComponent<SpriteRenderer>().color = Color.magenta;
        //}

        yield return null;
    }

    Vector2 GetIndex(Node node)
    {
        Vector2 position = node.transform.position;

        position.x = (int)(position.x + (map_size.y / 2));
        position.y = (int)(position.y + (map_size.x / 2));

        return position;
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