using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
//Node of graph that we make out of our level for pathfinding purposes
//Contains own row column coordinate and vector of nodes that AI can travel to from this node
public class Node
{
    public int row, column,ID;
    public List<Node> neighbors;
    public string type;
    public Node(int row, int column, string type)
    {
        this.row = row;
        this.column = column;
        this.type = type;
        neighbors = new List<Node>();
    }
}
//Node of A* algorithm
public class NodeA
{
    //from - distance from stars to current node
    //to - distance to destination from current node
    //total - sum of things above
    public double from,to,total;
    public bool closed = false;
    public bool opened = false;
    public int ID;
    public NodeA parent;
    public NodeA()
    {
        from = 0;
        to = 0;
        total = 0;
        ID = -1;
        parent = null;
    }
    public NodeA(Node startNode, Node endNode, int ID)
    {
        from = 0;
        to = Math.Abs(startNode.column-endNode.column) + Math.Abs(startNode.row - endNode.row);
        total = to;
        parent = null;
        this.ID = ID;
    }
    public NodeA(Node startNode, Node endNode, Node currentNode, NodeA parent,int ID)
    {
        from = Math.Abs(startNode.column - currentNode.column) + Math.Abs(startNode.row - currentNode.row);
        to = Math.Abs(currentNode.column - endNode.column) + Math.Abs(currentNode.row - endNode.row);
        total = to+from;
        this.parent = parent;
        this.ID = ID;
    }
    public void TryNewParent(Node startNode, Node endNode, Node currentNode, NodeA parent)
    {
        double fromTmp = Math.Abs(startNode.column - currentNode.column) + Math.Abs(startNode.row - currentNode.row);
        double toTmp = Math.Abs(currentNode.column - endNode.column) + Math.Abs(currentNode.row - endNode.row);
        double totalTmp = to + from;
        if(totalTmp<this.total)
        {
            this.to = toTmp;
            this.from = fromTmp;
            this.total = totalTmp;
            this.parent = parent;
        }
    }

}
//Struct for equation of line
public struct EOL
{
    public EOL(double a, double b)
    {
        this.a = a;
        this.b = b;
    }
    public double a, b;
}

public class PathFinding : MonoBehaviour
{
    //IN DEVELOPMENT
    public LevelGenerator levelGenerator;
    private int[,] tileMap;
    private List<Node>[,] nodeMap;
    private Vector2 tileSize;
    private Vector2 mapTranslation;
    public List<Node> graph;
    public GameObject spikes;
    public int jump_height, jump_lenght;


    // Use this for initialization
    void Start()
    {
        //Init graph
        graph = new List<Node>();
        //Load data from levelGenerator
        tileSize.y = levelGenerator.GetComponent<LevelGenerator>().height;
        tileSize.x = levelGenerator.GetComponent<LevelGenerator>().width;
        tileMap = levelGenerator.GetComponent<LevelGenerator>().tileMap;
        mapTranslation = levelGenerator.GetComponent<LevelGenerator>().mapTranslation;
        GenerateGraph();
        //PrintLinksToNeighbors("jump_start");
        //PrintNodes();
        //PrintGraph();
    }

    // Update is called once per frame
    void Update()
    {
        //Getting object position
        Vector3 position = GetComponent<Transform>().position;
        float x = position.x;
        float y = position.y;
        float z = position.z;
        //Transforming position to correspondent tileMap row and column
        int row = (int)((y + mapTranslation.y) / tileSize.y); //-1 * ((tileMap.GetLength(1) - randX) * width + mapTranslation.x);
        int column = (int)((x + mapTranslation.x) / tileSize.x);
        //Debug.Log(x + " " + y + " " + mapTranslation.x + " " + mapTranslation.y + " " + tileSize.x + " " + tileSize.y + " " + row + " " + column);
        //Instantiate(spikes, new Vector3(column  *tileSize.x + mapTranslation.x, row * tileSize.y + mapTranslation.y, 0), Quaternion.identity);
        //GoFromTo(5, 9,21, 11);

    }

    void GenerateGraph()
    {
        //**********************************************************************************************************************************************************************************************//
        //********************************************************        NODE GENERATION         ******************************************************************************************************//
        //**********************************************************************************************************************************************************************************************//
        //Generate nodes that will use to make graph that will use A* on to find path
        //Node types: "edge": is located above right and left tiles of plaform, "fall": located one to left or right from edge (from this node AI can fall down on lower platform)
        //            "landing": located below "fall" on platform that AI lands when it falls, "jump_start": node from where AI can jump to platform above it 
        for (int x = 0; x < tileMap.GetLength(1); x++)
        {
            for (int y = 0; y < tileMap.GetLength(0); y++)
            {
                //Check if current row and column correspond to platform edge (last right or left tile)
                if (tileMap[ClampRow(y), ClampColumn(x)] == 1 && tileMap[ClampRow(y - 1), ClampColumn(x)] != 1 &&
                    (tileMap[ClampRow(y), ClampColumn(x + 1)] != 1 || tileMap[ClampRow(y), ClampColumn(x - 1)] != 1))
                {
                    //Add node coorespondent to this edge (node above it)
                    Node currentEdgeNode = new Node(ClampRow(y - 1), ClampColumn(x), "edge");
                    graph.Add(currentEdgeNode);
                    //If we stay on edge in order to fall down we need to move 1 tile forward (Falling is the way how enemy goes to lower platform
                    //So we check if there is no obstacles on left or right (depends if it is left or right edge of platform) to add node where unit starts to fall
                    //Check right (we have right and left edges and need to treat them separatly)
                    if (tileMap[ClampRow(y), ClampColumn(x + 1)] != 1)
                    {
                        Node currentFallNode = new Node(ClampRow(y - 1), ClampColumn(x + 1), "fall");
                        graph.Add(currentFallNode);
                        //Look for place where unit lands (one tile above place where we encountered 1 first time) and add correponding node
                        for (int k = 0; k < tileMap.GetLength(0); k++)
                        {
                            if (tileMap[ClampRow(y + k), ClampColumn(x + 1)] == 1)
                            {
                                graph.Add(new Node(ClampRow(y + k - 1), ClampColumn(x + 1), "landing"));
                                //Draw fall paths
                                //Debug.DrawLine(new Vector3((x + 1) * tileSize.x, (tileMap.GetLength(0) - (y - 1)) * tileSize.y, 0), new Vector3((x + 1) * tileSize.x, (tileMap.GetLength(0) - (y + k - 1)) * tileSize.y, 0), Color.green, 9999, false);
                                break;
                            }
                        }
                        //Look for position from where unit can jump on this edge (jump from below)
                        List<Node> jump_start_node = new List<Node>();
                        for (int k = 1; k < jump_lenght; k++)
                        {
                            for (int l = 0; l < jump_height; l++)
                            {
                                if (tileMap[ClampRow(y + l), ClampColumn(x + k)] == 1)
                                {
                                    if (tileMap[ClampRow(y + l - 1), ClampColumn(x + k)] != 1 &&
                                        PlatformDoesntHaveJumpStartNode(jump_start_node, new Node(ClampRow(y + l - 1), ClampColumn(x + k), "jump_start")) &&
                                        PathDoesntCrossesTerrain(new Node(ClampRow(y + l - 1), ClampColumn(x + k), "jump_start"),
                                                                 new Node(ClampRow(y - 1), ClampColumn(x), "jump_start")))
                                    {
                                        Node currentJumpStartNode = new Node(ClampRow(y + l - 1), ClampColumn(x + k), "jump_start");
                                        currentJumpStartNode.neighbors.Add(currentFallNode);
                                        currentFallNode.neighbors.Add(currentJumpStartNode); 
                                        graph.Add(currentJumpStartNode);
                                        //jump_start_node.Add(currentJumpStartNode);
                                        //Draw jump paths
                                        //Debug.DrawLine(new Vector3(x * tileSize.x, (tileMap.GetLength(0) - (y - 1)) * tileSize.y, 0), new Vector3((x + k) * tileSize.x, (tileMap.GetLength(0) - (y + l - 1)) * tileSize.y, 0), Color.red, 9999, false);

                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //Check left
                    if (tileMap[ClampRow(y), ClampColumn(x - 1)]!= 1)
                    {
                        Node currentFallNode = new Node(ClampRow(y - 1), ClampColumn(x - 1), "fall");
                        graph.Add(currentFallNode);
                        for (int k = 0; k < tileMap.GetLength(0); k++)
                        {
                            if (tileMap[ClampRow(y + k), ClampColumn(x - 1)] == 1)
                            {
                                graph.Add(new Node(ClampRow(y + k - 1), ClampColumn(x - 1), "landing"));
                                //Draw fall paths
                                //Debug.DrawLine(new Vector3((x - 1) * tileSize.x, (tileMap.GetLength(0) - (y - 1)) * tileSize.y, 0), new Vector3((x - 1) * tileSize.x, (tileMap.GetLength(0) - (y + k - 1)) * tileSize.y, 0), Color.green, 9999, false);
                                break;
                            }
                        }
                        //Look for position from where unit can jump on this edge(jump from below)
                        List<Node> jump_start_node = new List<Node>();
                        for (int k = 1; k < jump_lenght; k++)
                        {
                            for (int l = 0; l < jump_height; l++)
                            {
                                if (tileMap[ClampRow(y + l), ClampColumn(x - k)] == 1)
                                {
                                    if (tileMap[ClampRow(y + l - 1), ClampColumn(x - k)] != 1 &&
                                        PlatformDoesntHaveJumpStartNode(jump_start_node, new Node(ClampRow(y + l - 1), ClampColumn(x - k), "jump_start")) &&
                                        PathDoesntCrossesTerrain(new Node(ClampRow(y + l - 1), ClampColumn(x - k), "jump_start"),
                                                                 new Node(ClampRow(y - 1), ClampColumn(x), "jump_start")))
                                    {
                                        Node currentJumpStartNode = new Node(ClampRow(y + l - 1), ClampColumn(x - k), "jump_start");
                                        currentJumpStartNode.neighbors.Add(currentFallNode);
                                        currentFallNode.neighbors.Add(currentJumpStartNode);
                                        graph.Add(currentJumpStartNode);
                                        //jump_start_node.Add(currentJumpStartNode);
                                        //Debug.DrawLine(new Vector3(x * tileSize.x, (tileMap.GetLength(0) - (y - 1)) * tileSize.y, 0), new Vector3((x - k ) * tileSize.x, (tileMap.GetLength(0) - (y + l - 1)) * tileSize.y, 0), Color.red, 9999, false);


                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        //PrintNodes();
        //PrintLinksToNeighbors("jump_start");
//**********************************************************************************************************************************************************************************************//
//********************************************************        REMOVAL OF DUPLICATED NODES         ******************************************************************************************//
//**********************************************************************************************************************************************************************************************//
        List<Node> graphTmp = new List<Node>();
        for (int i = 0; i < graph.Count - 1; i++)
        {
            bool foundDuplicate = false;
            for (int j = i + 1; j < graph.Count; j++)
            {
                if (graph[i].column == graph[j].column && graph[i].row == graph[j].row && graph[i].type.Equals(graph[j].type))
                {
                    CopyNeighbors(graph[i], graph[j]);
                    foundDuplicate = true;
                    break;
                }
            }
            if (!foundDuplicate)
            {
                graphTmp.Add(graph[i]);
            }
        }
        graphTmp.Add(graph[graph.Count - 1]);
        graph.Clear();
        graph = graphTmp;
        //PrintNodes();
        //PrintLinksToNeighbors("fall");

//**********************************************************************************************************************************************************************************************//
//********************************************************        GENERATION OF NODEMAP               ******************************************************************************************//
//**********************************************************************************************************************************************************************************************//

        //nodeMap has same size as tileMap and allows to access graph nodes by row and column
        //Init
        nodeMap = new List<Node>[tileMap.GetLength(0), tileMap.GetLength(1)];
        for (int row=0;row<tileMap.GetLength(0);row++)
        {
            for(int column=0;column<tileMap.GetLength(1);column++)
            {
                nodeMap[row, column] = new List<Node>();
            }
        }
        //Fill nodeMap with node data
        for(int i=0;i<graph.Count;i++)
        {
            nodeMap[graph[i].row, graph[i].column].Add(graph[i]);
        }


        //**********************************************************************************************************************************************************************************************//
        //********************************************************        NODE CONNECTION INTO GRAPH          ******************************************************************************************//
        //**********************************************************************************************************************************************************************************************//

        //Assign IDs
        for (int i = 0; i < graph.Count; i++)
        {
            graph[i].ID = i;
        }
        //Connect "fall" nodes with "landing" nodes
        for (int i=0;i<graph.Count;i++)
        {
            if(graph[i].type.Equals("fall"))
            {
                //Search for landing coordinates
                for (int j = 0;j< tileMap.GetLength(0);j++)
                {
                    if(tileMap[ClampRow(graph[i].row+j+1), graph[i].column] == 1)
                    {
                        //Find "landing" node with that coordinates
                        for(int k=0;k<graph.Count;k++)
                        {
                            if(graph[k].column == graph[i].column &&
                               graph[k].row == graph[i].row + j &&
                               graph[k].type.Equals("landing"))
                            {
                                graph[i].neighbors.Add(graph[k]);
                                goto EndFall;
                            }
                        }
                    }
                }
            }
            EndFall:;          
        }
        //PrintLinksuToNeighbors("fall");
        //Connect nodes on same platform
        for(int i=0;i<graph.Count;i++)
        {
            if (graph[i].row == 22 && graph[i].type.Equals("landing"))
            {
                int r = 0;
            }
            //Find closest left node on this platform
            for (int j=1;j<tileMap.GetLength(1);j++)
            {
                //If there is tile right and below and there is no tile on current coordinate

                if(tileMap[ClampRow(graph[i].row+1), ClampColumn(graph[i].column-j+1)]==1 &&
                   tileMap[ClampRow(graph[i].row), ClampColumn(graph[i].column - j)] != 1)
                {
                    if (nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column - j)].Count != 0)
                    {
                        graph[i].neighbors.Add(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column - j)][0]);
                        break;
                    }
                }
                else if (graph[i].type.Equals("fall"))
                {
                    if (nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column - j)].Count != 0)
                    {
                        graph[i].neighbors.Add(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column - j)][0]);
                    }
                    break;
                }
                else
                {
                    break;
                }

            }

            //Find closest right node on this platform
            for (int j = 1; j < tileMap.GetLength(1); j++)
            {
                //If there is tile right and below and there is no tile on current coordinates
                if (tileMap[ClampRow(graph[i].row + 1), ClampColumn(graph[i].column + j-1)] == 1 &&
                   tileMap[ClampRow(graph[i].row), ClampColumn(graph[i].column + j)] != 1)
                {
                    if(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column + j)].Count!=0)
                    {
                        //graph[i].neighbors.Add(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column + j)][0]);
                        break;
                    }                    
                }
                else if(graph[i].type.Equals("fall"))
                {
                    if(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column + j)].Count!=0)
                    {
                        graph[i].neighbors.Add(nodeMap[ClampRow(graph[i].row), ClampColumn(graph[i].column + j)][0]);
                    }
                    break;
                }
                else
                {
                    break;
                }

            }
            
        }
        //for (int i = 0; i < graph.Count; i++)
        //{
        //    if (graph[i].type.Equals("edge"))
        //    {
        //        PrintLinksToNeighborsRecursivly(graph[i], 3);
        //    }
        //}
        //PrintNodes();        
        //PrintLinksuToNeighbors("fall");
        //Connect node on same tile
        for (int row = 0; row < tileMap.GetLength(0); row++)
        {
            for (int column = 0; column < tileMap.GetLength(1); column++)
            {
                for (int k = 0; k < nodeMap[row, column].Count; k++)
                {
                    for (int l = 0; l < nodeMap[row, column].Count; l++)
                    {
                        AddNeighbor(nodeMap[row,column][k], nodeMap[row, column][l]);                       
                    }
                }
            }
        }
        //for (int i = 0; i < graph.Count; i++)
        //{
        //    if (graph[i].type.Equals("edge"))
        //    {
        //        PrintLinksToNeighborsRecursivly(graph[i], 2);
        //    }
        //}



        //PrintGraph();
        //PrintLinksToNeighbors("jump_start");
    }

    //Given coordinates of two points function will draw path between them
    public List<Vector2> GoFromTo(double xStart, double yStart, double xEnd, double yEnd)
    {
        int rowStart = tileMap.GetLength(0) - (int)((yStart + mapTranslation.y) / tileSize.y); //-1 * ((tileMap.GetLength(1) - randX) * width + mapTranslation.x);
        int columnStart = (int)((xStart + mapTranslation.x) / tileSize.x);
        int rowEnd = tileMap.GetLength(0) - (int)((yEnd + mapTranslation.y) / tileSize.y); //-1 * ((tileMap.GetLength(1) - randX) * width + mapTranslation.x);
        int columnEnd = (int)((xEnd + mapTranslation.x) / tileSize.x);
        int rowStartLanding=0;
        int rowEndLanding=0;
        Node startNode = null;
        Node endNode = null;
        //Return list with points that AI should go through
        List<Vector2> path = new List<Vector2>();

        //Instantiate(spikes, new Vector3(columnStart * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - rowStart) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);
        //Instantiate(spikes, new Vector3(columnEnd * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - rowEnd) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);

        //**********************************************************************************************************************************************************************************************//
        //********************************************************       FIND START AND END NODES ******************************************************************************************************//
        //**********************************************************************************************************************************************************************************************//

        //Check if start is on platform
        //If not it means that unit is in air and we draw vertical path to land it
        if (tileMap[ClampRow(rowStart + 1),ClampColumn(columnStart)]!=1)
        {
            for(int i=0;i<tileMap.GetLength(0);i++)
            {
                if(tileMap[ClampRow(rowStart + 1 + i) , ClampColumn(columnStart)] == 1)
                {
                    //Debug.DrawLine(new Vector3((columnStart) * tileSize.x, (tileMap.GetLength(0) - (rowStart + i)) * tileSize.y, 0),
                    //               new Vector3((columnStart) * tileSize.x, (tileMap.GetLength(0) - rowStart) * tileSize.y, 0),
                    //               Color.white, 1, false);
                    rowStartLanding = rowStart + i;
                    break;
                }
            }
         }
        else
        {
            rowStartLanding = rowStart;
        }

        //Check if end is on platform
        //If not it means that unit is in air and we draw vertical path to land it
        if (tileMap[ClampRow(rowEnd + 1), ClampColumn(columnEnd)] != 1)
        {
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                if (tileMap[ClampRow(rowEnd + 1 + i), ClampColumn(columnEnd)] == 1)
                {
                    //Debug.DrawLine(new Vector3((columnEnd) * tileSize.x, (tileMap.GetLength(0) - (rowEnd + i)) * tileSize.y, 0),
                    //               new Vector3((columnEnd) * tileSize.x, (tileMap.GetLength(0) - rowEnd) * tileSize.y, 0),
                    //               Color.white, 1, false);
                    rowEndLanding = rowEnd + i;
                    break;
                }
            }
        }
        else
        {
            rowEndLanding = rowEnd;
        }
        //Now we look for closets node on platform where unit is or just landed
        //Looking for it we move 1 tile right and left each iteration untill we uncounter node
        bool leftIsOk = true;
        bool rightIsOk = true;

        //Search for start node 
        for(int i=0;i< tileMap.GetLength(1);i++)
        {
            int tmp = ClampColumn(columnStart + i);
            if (tileMap[ClampRow(rowStartLanding), ClampColumn(columnStart + i)]==1)
            {
                rightIsOk = false;
            }
            if (tileMap[ClampRow(rowStartLanding), ClampColumn(columnStart - i)] == 1)
            {
                leftIsOk = false;
            }
            if (nodeMap[ClampRow(rowStartLanding),ClampColumn(columnStart+i)].Count!=0 && rightIsOk)
            {
                startNode = nodeMap[ClampRow(rowStartLanding), ClampColumn(columnStart + i)][0];
                break;
            }
            if (nodeMap[ClampRow(rowStartLanding), ClampColumn(columnStart - i)].Count != 0 && leftIsOk)
            {
                startNode = nodeMap[ClampRow(rowStartLanding), ClampColumn(columnStart - i)][0];
                break;
            }
        }

        leftIsOk = true;
        rightIsOk = true;

        //Search for end node
        for (int i = 0; i < tileMap.GetLength(1); i++)
        {
            if (tileMap[ClampRow(rowEndLanding), ClampColumn(columnEnd + i)] == 1)
            {
                rightIsOk = false;
            }
            if (tileMap[ClampRow(rowEndLanding), ClampColumn(columnEnd - i)] == 1)
            {
                leftIsOk = false;
            }
            if (nodeMap[ClampRow(rowEndLanding), ClampColumn(columnEnd + i)].Count != 0 && rightIsOk)
            {
                endNode = nodeMap[ClampRow(rowEndLanding), ClampColumn(columnEnd + i)][0];
                break;
            }
            if (nodeMap[ClampRow(rowEndLanding), ClampColumn(columnEnd - i)].Count != 0 && leftIsOk)
            {
                endNode = nodeMap[ClampRow(rowEndLanding), ClampColumn(columnEnd - i)][0];
                break;
            }
        }
        //Instantiate(spikes, new Vector3(columnStart * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - rowStartLanding) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);
        //Instantiate(spikes, new Vector3(columnEnd * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - rowEndLanding) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);


        //**********************************************************************************************************************************************************************************************//
        //***********************************r*********************       FIND PATH FROM START TO END NODE        ***************************************************************************************//
        //**********************************************************************************************************************************************************************************************//

        List<NodeA> open = new List<NodeA>();
        List<NodeA> closed = new List<NodeA>();
        List<NodeA> nodesA = new List<NodeA>();
        NodeA currentNodeA;
        Node currentNode;
        //Init nodesA
        for(int i=0;i<graph.Count;i++)
        {
            nodesA.Add(null);
        }
        //Create first nodeA
        nodesA[startNode.ID] = new NodeA(startNode, endNode, startNode.ID);
        nodesA[startNode.ID].opened = true;
        currentNodeA = nodesA[startNode.ID];
        currentNode = startNode;
        for (int i = 0; i < graph.Count; i++)
        {
            //If currnet node ID equal to end node ID then we found path
            if(currentNodeA.ID == endNode.ID)
            {
                break;
            }
            //Go through all neighbors
            for(int j=0;j<currentNode.neighbors.Count;j++)
            {
                //If neighbors is not in open list we create it and fill with data
                if(nodesA[currentNode.neighbors[j].ID] == null)
                {
                    nodesA[currentNode.neighbors[j].ID] = new NodeA(startNode, endNode, currentNode, currentNodeA, currentNode.neighbors[j].ID);
                    nodesA[currentNode.neighbors[j].ID].opened = true;
                    open.Add(nodesA[currentNode.neighbors[j].ID]);
                }
                //Else we check if my path to this neighbor has lower total cast
                else if(!nodesA[currentNode.neighbors[j].ID].closed)
                {
                    nodesA[currentNode.neighbors[j].ID].TryNewParent(startNode, endNode, currentNode, currentNodeA);
                }
            }
            //Closing current node and adding it to closed list
            currentNodeA.closed = true;
            closed.Add(currentNodeA);
            //Find new current node by choosing first nodeA with smallest "total" value from open list
            double minTotal = Double.MaxValue;
            int minID = -1;
            for(int j=0;j<open.Count;j++)
            {
                if(minTotal>open[j].total)
                {
                    minID = j;
                    minTotal = open[j].total;
                }
            }
            //Code below can render all steps that A*took
            if (i != 0)
            {
                //Debug.DrawLine(new Vector3((currentNode.column) * tileSize.x, (tileMap.GetLength(0) - (currentNode.row)) * tileSize.y, 0),
                //               new Vector3((graph[currentNodeA.parent.ID].column) * tileSize.x, (tileMap.GetLength(0) - graph[currentNodeA.parent.ID].row) * tileSize.y, 0),
                //               Color.black, 1, false);
            }

            if(minID == -1 || open.Count==0)
            {
                return path;
            }
            currentNodeA = open[minID];
            currentNode = graph[currentNodeA.ID];
            open.RemoveAt(minID);
        }
        List<Vector2> pathTmp = new List<Vector2>();
        while (currentNodeA.ID != startNode.ID)
        {
            NodeA nextNodeA = currentNodeA.parent;
            //Debug.DrawLine(new Vector3((graph[currentNodeA.ID].column) * tileSize.x, (tileMap.GetLength(0) - (graph[currentNodeA.ID].row)) * tileSize.y, 0),
            //               new Vector3((graph[nextNodeA.ID].column) * tileSize.x, (tileMap.GetLength(0) - (graph[nextNodeA.ID].row)) * tileSize.y, 0),
            //                       Color.white, 1, false);
            currentNodeA = currentNodeA.parent;
            pathTmp.Add(new Vector2(graph[currentNodeA.ID].column * tileSize.x, 
                                 (tileMap.GetLength(0) - graph[currentNodeA.ID].row) * tileSize.y));
        }

        //PrintGraph();
        //Reverce path
        for (int i = 0; i < pathTmp.Count; i++)
        {
            path.Add(pathTmp[pathTmp.Count - 1 - i]);
        }
        return path;
    }

    public void PrintGraph()
    {
        for (int i = 0; i < graph.Count; i++)
        {
            if (graph[i].type.Equals("fall"))
            {
                for (int j = 0; j < graph[i].neighbors.Count; j++)
                {
                    Debug.DrawLine(new Vector3(graph[i].column * tileSize.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y, 0), new Vector3(graph[i].neighbors[j].column * tileSize.x, (tileMap.GetLength(0) - graph[i].neighbors[j].row) * tileSize.y, 0), Color.yellow, 9999, false);

                }
            }
            else if (graph[i].type.Equals("jump_start"))
            {
                for (int j = 0; j < graph[i].neighbors.Count; j++)
                {
                    Debug.DrawLine(new Vector3(graph[i].column * tileSize.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y, 0), new Vector3(graph[i].neighbors[j].column * tileSize.x, (tileMap.GetLength(0) - graph[i].neighbors[j].row) * tileSize.y, 0), Color.red, 9999, false);

                }
            }
            else if (graph[i].type.Equals("landing"))
            {
                for (int j = 0; j < graph[i].neighbors.Count; j++)
                {
                    Debug.DrawLine(new Vector3(graph[i].column * tileSize.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y, 0), new Vector3(graph[i].neighbors[j].column * tileSize.x, (tileMap.GetLength(0) - graph[i].neighbors[j].row) * tileSize.y, 0), Color.green, 9999, false);

                }
            }
            else if (graph[i].type.Equals("edge"))
            {
                for (int j = 0; j < graph[i].neighbors.Count; j++)
                {
                    Debug.DrawLine(new Vector3(graph[i].column * tileSize.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y, 0), new Vector3(graph[i].neighbors[j].column * tileSize.x, (tileMap.GetLength(0) - graph[i].neighbors[j].row) * tileSize.y, 0), Color.blue, 9999, false);

                }
            }
        }
    }

    //Renders lines to each neighbor of particular type of node 
    void PrintLinksToNeighbors(string type)
    {
        for(int i=0;i<graph.Count;i++)
        {
            if(graph[i].type.Equals(type))
            {
                for (int j = 0; j < graph[i].neighbors.Count; j++)
                {
                    Debug.DrawLine(new Vector3(graph[i].column * tileSize.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y, 0), new Vector3(graph[i].neighbors[j].column * tileSize.x, (tileMap.GetLength(0) - graph[i].neighbors[j].row) * tileSize.y, 0), Color.yellow, 9999, false);

                }
            }            
        }
    }

    void PrintLinksToNeighborsRecursivly(Node node,int count)
    {
        if(count==0)
        {
            return;
        }
        else
        {
            count--;
        }
        for (int i = 0; i < node.neighbors.Count; i++)
        {
                Debug.DrawLine(new Vector3(node.column * tileSize.x, (tileMap.GetLength(0) - node.row) * tileSize.y, 0), 
                               new Vector3(node.neighbors[i].column * tileSize.x, (tileMap.GetLength(0) - node.neighbors[i].row) * tileSize.y, 0), 
                               Color.yellow, 9999, false);
            PrintLinksToNeighborsRecursivly(node.neighbors[i], count);
        }
    }

    void PrintNodes()
    {
        for (int i = 0; i < graph.Count; i++)
        {
            if (!graph[i].type.Equals("jump_starts"))
            {
                Instantiate(spikes, new Vector3(graph[i].column * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - graph[i].row) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);
            }
        }

    }

    //Checks if nodes from list belong to platform that currnet node belongs to
    bool PlatformDoesntHaveJumpStartNode(List<Node> existingJumpStartNodes, Node currentNode)
    {
        for (int i = 0; i < existingJumpStartNodes.Count; i++)
        {
            //If nodes have same row they can be on same platform
            if (existingJumpStartNodes[i].row == currentNode.row)
            {
                //if nodes have same row and columns platform obvoiusly already has node
                if (existingJumpStartNodes[i].column == currentNode.column)
                {
                    return false;
                }
                //else we need to check if nodes bolong to same platform checking if there is row of 1's (single platform) that connects these columns
                else if (existingJumpStartNodes[i].column > currentNode.column)
                {
                    for (int j = 0; j < tileMap.GetLength(1); j++)
                    {
                        //Platform ended and we didn't rech node                
                        if (tileMap[(existingJumpStartNodes[i].row + 1 + tileMap.GetLength(0)) % tileMap.GetLength(0), (existingJumpStartNodes[i].column - j + tileMap.GetLength(1)) % tileMap.GetLength(1)] != 1)
                        {
                            break;
                        }
                        //We reached current node column from one of nodes from list that means that plaftform already has node on it
                        if (existingJumpStartNodes[i].column - j == currentNode.column)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < tileMap.GetLength(1); j++)
                    {
                        //Platform ended and we didn't rech node
                        if (tileMap[(existingJumpStartNodes[i].row + 1 + tileMap.GetLength(0)) % tileMap.GetLength(0), (existingJumpStartNodes[i].column + j + tileMap.GetLength(1)) % tileMap.GetLength(1)] != 1)
                        {
                            break;
                        }
                        //We reached current node column from one of nodes from list that means that plaftform already has node on it
                        if (existingJumpStartNodes[i].column + j == currentNode.column)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //Checks if straight line from node1 to node2 crosses any platform
    bool PathDoesntCrossesTerrain(Node node1, Node node2)
    {
        int columnStart, columnEnd, rowStart, rowEnd;

        //Find max and min row and column    
        if (node1.column > node2.column)
        {
            columnStart = node2.column;
            columnEnd = node1.column;
            rowStart = node2.row;
            rowEnd = node1.row;
        }
        else if (node1.column < node2.column)
        {
            columnStart = node1.column;
            columnEnd = node2.column;
            rowStart = node1.row;
            rowEnd = node2.row;
        }
        else
        {
            return true;
        }

        if (node1.row == node2.row)
        {
            for (int column = columnStart; column < columnEnd; column++)
            {
                if (tileMap[node2.row, column] == 1)
                {
                    return false;
                }
            }
            return true;
        }
        EOL eol = GetEquationOfLine(columnStart, rowStart, columnEnd, rowEnd);
        double a = eol.a;
        double b = eol.b;
        if (a < 0)
        {
            int y = 0;
        }
        //Find through which tiles line passes and if it passes trough tile with 1 (terrain tile) return flase
        for (int column = columnStart + 1; column < columnEnd; column++)
        {
            int rowUpper = (int)(a * column + b);
            int rowLower = (int)(a * (column + 1) + b);
            if (rowLower < rowUpper)
            {
                for (int row = rowLower; row <= rowUpper; row++)
                {
                    //Instantiate(spikes, new Vector3(column * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - row) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);
                    if (tileMap[row, column] == 1)
                    {
                        return false;
                    }
                }
            }

            else
            {
                for (int row = rowUpper; row <= rowLower; row++)
                {
                    //Instantiate(spikes, new Vector3(column * tileSize.x + mapTranslation.x, (tileMap.GetLength(0) - row) * tileSize.y + mapTranslation.y, 0), Quaternion.identity);

                    if (tileMap[row, column] == 1)
                    {
                        return false;
                    }
                }
            }

        }
        return true;
    }

    EOL GetEquationOfLine(int lineStartX, int lineStartY, int lineEndX, int lineEndY)
    {
        double xDelta = lineEndX - lineStartX;
        double yDelta = lineEndY - lineStartY;
        double slope = yDelta / xDelta;
        double a = slope;
        double b = (double)lineStartY - slope * (double)lineStartX;//y=ax+b >> b=y-ax (x and y can be use from  lineStart or from lineEnd)
        return new global::EOL(a, b);
    }

    //Takes tow nodes and write neigbors of first one to second one. If second already has node it wont be copied
    void CopyNeighbors(Node copyFromNode, Node copyToNode)
    {
        for(int i=0;i<copyFromNode.neighbors.Count;i++)
        {
            bool hasCopy = false;
            for(int j=0;j<copyToNode.neighbors.Count;j++)
            {
                if(copyFromNode.neighbors[i].column == copyToNode.neighbors[j].column &&
                   copyFromNode.neighbors[i].row == copyToNode.neighbors[j].row &&
                   copyFromNode.neighbors[i].type.Equals(copyToNode.neighbors[j].type))
                {
                    hasCopy = true;
                    break;
                }
            }
            if(!hasCopy)
            {
                copyToNode.neighbors.Add(copyFromNode.neighbors[i]);
            }
        }
    }

    void AddNeighbor(Node node, Node neighbor)
    {
        //Same node -> return
        if(node.ID==neighbor.ID)
        {
            return;
        }
        else
        {
            for(int i=0;i<node.neighbors.Count;i++)
            {
                //Check if neighbor is already in node's neighbors list 
                if(node.neighbors[i].ID == neighbor.ID)
                {
                    return;
                }
            }
            node.neighbors.Add(neighbor);
        }
        return;
    }

    int ClampRow(int row)
    {
        if(row<0)
        {
            return tileMap.GetLength(0) - Math.Abs(row) % tileMap.GetLength(0);
        }
        else
        {
            return row % tileMap.GetLength(0);
        }
    }

    int ClampColumn(int column)
    {
        if (column < 0)
        {
            return tileMap.GetLength(1) - Math.Abs(column) % tileMap.GetLength(1);
        }
        else
        {
            return column % tileMap.GetLength(1);
        }
    }


}
