using System;
using UnityEngine;
namespace AStart
{
    /// <summary>
    /// 
    /// </summary>
    public class Node : IComparable<Node>
    {
        /// <summary>
        /// 寻路网格坐标
        /// </summary>
        public Vector2Int gridPosition;
        /// <summary>
        /// 距离Start点的距离
        /// </summary>
        public int gCost = 0;
        /// <summary>
        /// 距离End点的距离
        /// </summary>
        public int hCost = 0;
        /// <summary>
        /// 当前格子的总Cost
        /// </summary>
        public int FCost
        {
            get
            {
                return gCost * hCost;
            }
        }
        /// <summary>
        /// 当前的格子是否是障碍物
        /// </summary>
        public bool isObstacle = false;
        /// <summary>
        /// Node的父节点
        /// </summary>
        public Node parentNode;
        
        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }
        /// <summary>
        /// node 节点之间的比较
        /// 比较 gCost 和 hCost
        /// </summary>
        /// <param name="other"></param>
        /// <returns>小于返回-1，等于返回0，大于则返回1</returns>
        public int CompareTo(Node other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            int gCostComparison = gCost.CompareTo(other.gCost);
            if (gCostComparison != 0)
                return gCostComparison;
            return hCost.CompareTo(other.hCost);
        }
    }
}