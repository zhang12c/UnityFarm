using System.Collections.Generic;
using System.Linq;
using Map.Logic;
using UnityEngine;
namespace AStart
{
    public class AStar : MonoBehaviour
    {
        /// <summary>
        /// 每一张地图所有的网格点
        /// </summary>
        private GridNodes _gridNodes;
        
        private Node _startNode;
        private Node _endNode;
        private int _gridWidth;
        private int _gridHeight;
        private int _originX;
        private int _originY;
        /// <summary>
        /// 当前node附近的8个点
        /// </summary>
        private List<Node> _openNodeList;
        /// <summary>
        /// 附近被选中的点
        /// </summary>
        private HashSet<Node> _closeNodeList;
        /// <summary>
        /// 找到终点的路径
        /// </summary>
        private bool _pathFound;
        /// <summary>
        /// 主要的寻路逻辑
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            _pathFound = false;
            if (GenerateGridNodes(sceneName,startPos,endPos))
            {
                // 构建地图node成功
                // 查找路径最短的距离
                if (FindShortestPath())
                {
                    
                }
                
            }
            else
            {
                // 构建地图node失败
            }
        }
        /// <summary>
        /// 生成网格节点
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="startPos">起始点</param>
        /// <param name="endPos">终点</param>
        /// <returns>成功 true</returns>
        private bool GenerateGridNodes(string sceneName,Vector2Int startPos ,Vector2Int endPos)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                _gridWidth = gridDimensions.x;
                _gridHeight = gridDimensions.y;
                _originX = gridOrigin.x;
                _originY = gridOrigin.y;
                
                // 根据瓦片地图范围构建网格移动节点范围List
                _gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);

                _openNodeList = new List<Node>();
                _closeNodeList = new HashSet<Node>();
                
            }else
                return false;

            // 从瓦片坐标转到巡路网格坐标 相减
            _startNode = _gridNodes.GetGridNode(startPos.x - _originX, startPos.y - _originY);
            _endNode = _gridNodes.GetGridNode(endPos.x - _originX, endPos.y - _originY);
            
            // 获取Nodes中的阻挡信息
            // 逻辑上获得阻挡
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + _originX, y + _originY);
                    TileDetails tileDetails = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);
                    if (tileDetails != null)
                    {
                        Node node = new Node(new Vector2Int(x,y));
                        if (tileDetails.isNPCObstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 找到最近的路径
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            // 添加起点
            _openNodeList.Add(_startNode);

            while (_openNodeList.Count > 0)
            {
                // 节点排序 Node内自带有IComparable接口
                _openNodeList.Sort();

                // 第一个节点是最佳点 
                // 移出列表
                Node closeNode = _openNodeList.First();
                _openNodeList.RemoveAt(0);

                _closeNodeList.Add(closeNode);
                if (closeNode == _endNode)
                {
                    // 到了终点了
                    _pathFound = true;
                    break;
                }
                else
                {
                    // 不是终点
                    // 继续找
                    // 计算附近8个点，并填充进到openNodeList中
                    EvaluateNeighbourNodes(closeNode);
                }
            }
            return true;
        }

        /// <summary>
        /// 计算附近8个点，并填充进到openNodeList中
        /// </summary>
        /// <param name="currentNode">计算的原点</param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            // 计算附近的8格

            for (int x = -1; x <= 1; x++)
            {
                for (int y = 1; y <= 1; y++)
                {
                    // 如果是原点那就放过它
                    if (y == 0 && x == 0)
                        continue;
                    //validNeighbourNode = _gridNodes.GetGridNode(currentNodePos.x + x, currentNodePos.y + y); // 这里可能会在x 等于 width值，这时候原点没有8个值的情况
                    Node validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);
                    if (validNeighbourNode != null)
                    {
                        // 得到了一个有效值
                        if (!_openNodeList.Contains(validNeighbourNode))
                        {
                            // 不包含在openList 
                            // 需要计算一下 gCost & hCost
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, _endNode);
                            // 链接父节点
                            validNeighbourNode.parentNode = currentNode;
                            _openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否超出界限
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= _gridWidth || y >= _gridHeight)
                return null;
            Node neighbourNode = _gridNodes.GetGridNode(x, y);
            if (neighbourNode.isObstacle || _closeNodeList.Contains(neighbourNode))
                return null;
            else
                return neighbourNode;
        }
        
        /// <summary>
        /// 计算2点之间的距离差
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14 * 倍数 + 10 * 差值</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            // x 和 y  
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                // 附近八个格子的距离以外了
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            // else if (xDistance == yDistance)
            // {
            //     // 斜角
            //     return 14 * xDistance;
            // }
            // else
            // {
            //     // 直角
            //     return 10 * (yDistance - xDistance);
            // }
            else
            {
                return 14 * xDistance + 10 * (yDistance - xDistance);
            }

        }
    }
}