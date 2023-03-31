using UnityEngine;
namespace AStart
{
    /// <summary>
    /// nodes 的集合 一个一个格子绘制成的一个面
    /// </summary>
    public class GridNodes
    {
        private int _width;
        private int _height;
        private Node[,] _gridNodes;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public GridNodes(int width,int height)
        {
            this._width = width;
            this._height = height;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Node node = new Node(new Vector2Int(x,y));
                    _gridNodes[x, y] = node;
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