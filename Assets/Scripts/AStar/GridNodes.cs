using UnityEngine;
namespace AStar
{
    /// <summary>
    /// nodes 的集合 一个一个格子绘制成的一个面
    /// </summary>
    public class GridNodes
    {
        private readonly int _width;
        private readonly int _height;
        private Node[,] _gridNodes;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public GridNodes(int width,int height)
        {
            _width = width;
            _height = height;

            _gridNodes = new Node[width, height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _gridNodes[x, y] = new Node(new Vector2Int(x,y));
                }
            }
        }
        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos <= _width && yPos <= _height)
            {
                return _gridNodes[xPos, yPos];
            }
            Debug.LogError("获取" + $"x:{xPos}  y:{yPos}" + "是空的？");
            return null;
        }
    }
}