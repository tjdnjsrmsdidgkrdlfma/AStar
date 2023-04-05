using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    List<Node> opened_list = new List<Node>();
    List<Node> closed_list = new List<Node>();

    Vector2[] direction = new Vector2[4];
    Vector2 map_size = new Vector2(10, 18);
    #endregion

    #region 기타
    [Header("기타")]
    [SerializeField] GameObject mouse_position;
    [SerializeField] TextMeshProUGUI no_route_found;
    #endregion

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

        Node.up_arrow_image = Resources.Load<Sprite>("Up");
        Node.down_arrow_image= Resources.Load<Sprite>("Down");
        Node.left_arrow_image= Resources.Load<Sprite>("Left");
        Node.right_arrow_image = Resources.Load<Sprite>("Right");
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
        if (start == null || end == null)
            yield break;

        opened_list.Add(start);

        Vector2 node_index = GetIndex(start);

        for (int i = 0; i < direction.Length; i++)
        {
            //주변에 있는 노드가 존재하지 않는 노드인지 확인
            if (((int)node_index.y + (int)direction[i].y) < 0 || ((int)node_index.y + (int)direction[i].y) >= map_size.x
                || ((int)node_index.x + (int)direction[i].x) < 0 || ((int)node_index.x + (int)direction[i].x) >= map_size.y)
                continue;

            Node node = node_list[(int)node_index.y + (int)direction[i].y, (int)node_index.x + (int)direction[i].x];

            //주변에 있는 노드가 이동할 수 없는지 확인
            if (node.State == NodeState.Wall)
                continue;

            //시작 노드의 근처에 있는 노드들을 열린 목록에 넣어주고 부모 노드와 G, H, F값을 설정
            opened_list.Add(node);
            node.ParentNode = start;
            node.G = 1;
            node.H = (int)Mathf.Abs(node.transform.position.x - end.transform.position.x)
                     + (int)Mathf.Abs(node.transform.position.y - end.transform.position.y);
            node.F = node.G + node.H;
            node.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        opened_list.Remove(start);
        closed_list.Add(start);

        Node smallest_f_value_node;

        while (true)
        {
            //열린 목록에 끝 노드가 포함되어 있는 경우 == 시작 노드에서 끝 노드로 가는 길을 찾은 경우
            if (opened_list.Contains(end) == true)
            {
                StartCoroutine(RouteFound());
                yield break;
            }

            //열린 목록에 아무 것도 없는 경우 == 시작 노드에서 끝 노드로 가는 길이 없는 경우
            if (opened_list.Count == 0)
            {
                StartCoroutine(NoRouteFound());
                yield break;
            }
                
            smallest_f_value_node = opened_list[0];

            foreach (Node node in opened_list) //열린 목록에 있는 노드 중 F값이 가장 작은 노드를 검색
            {
                if (node.F < smallest_f_value_node.F)
                {
                    smallest_f_value_node = node;
                }
            }

            yield return new WaitForSeconds(0.5f);

            //열린 목록에서 가장 작은 F값을 가진 노드를 열린 목록에서 제거하고 닫힌 목록으로 이동
            smallest_f_value_node.GetComponent<SpriteRenderer>().color = new Color(1, 0.46f, 0.008f, 1);
            opened_list.Remove(smallest_f_value_node);
            closed_list.Add(smallest_f_value_node);

            node_index = GetIndex(smallest_f_value_node);

            for (int i = 0; i < direction.Length; i++)
            {
                //주변에 있는 노드가 존재하지 않는 노드인지 확인
                if (((int)node_index.y + (int)direction[i].y) < 0 || ((int)node_index.y + (int)direction[i].y) >= map_size.x
                    || ((int)node_index.x + (int)direction[i].x) < 0 || ((int)node_index.x + (int)direction[i].x) >= map_size.y)
                    continue;

                Node node = node_list[(int)node_index.y + (int)direction[i].y, (int)node_index.x + (int)direction[i].x];

                //주변에 있는 노드가 이동할 수 없거나 닫힌 목록에 포함되어 있는지 확인
                if (node.State == NodeState.Wall
                    || closed_list.Contains(node) == true)
                    continue;

                node.GetComponent<SpriteRenderer>().color = Color.yellow;
                int temp_g = smallest_f_value_node.G + 1;
                int temp_h = (int)Mathf.Abs(node.transform.position.x - end.transform.position.x)
                             + (int)Mathf.Abs(node.transform.position.y - end.transform.position.y);
                int temp_f = temp_g + temp_h;

                //열린 목록에 이미 존재하고 있었던 노드라면
                if (opened_list.Contains(node) == true)
                {
                    //원래의 G값과 새로 계산한 G값을 비교해서 새로 계산한 G값이 더 작으면 값을 변경
                    if (node.G > temp_g)
                    {
                        node.ParentNode = smallest_f_value_node;
                        node.G = temp_g;
                        node.H = temp_h;
                        node.F = temp_f;
                    }
                }
                //열린 목록에 존재하지 않는 노드라면
                else
                {
                    //열린 목록에 추가하고 값을 변경
                    opened_list.Add(node);
                    node.ParentNode = smallest_f_value_node;
                    node.G = temp_g;
                    node.H = temp_h;
                    node.F = temp_f;
                }
            }
        }

        
    }

    Vector2 GetIndex(Node node)
    {
        Vector2 position = node.transform.position;

        position.x = (int)(position.x + (map_size.y / 2));
        position.y = (int)(position.y + (map_size.x / 2));

        return position;
    }

    IEnumerator RouteFound()
    {
        //temp_node 노드의 ParentNode를 Push하고 temp_node의 ParentNode를 temp_node에 대입하고 ParentNode가 start가 아닌 동안 반복
        Stack<Node> node_stack = new Stack<Node>();
        Node temp_node = end;

        node_stack.Push(temp_node);
        while (true)
        {
            node_stack.Push(temp_node.ParentNode);
            temp_node = temp_node.ParentNode;
            if (temp_node == start)
                break;
        }

        while (node_stack.Count > 0)
        {
            temp_node = node_stack.Pop();
            temp_node.GetComponent<SpriteRenderer>().color = Color.gray;

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator NoRouteFound()
    {
        yield return new WaitForSeconds(0.5f);

        no_route_found.enabled = true;

        yield return new WaitForSeconds(3);

        no_route_found.enabled = false;
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