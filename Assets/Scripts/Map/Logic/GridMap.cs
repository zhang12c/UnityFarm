using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3Int = UnityEngine.Vector3Int;

/// <summary>
/// 挂载在每个地图的tilemap上
/// 主要目的是自动获取地图信息
/// 将数据保存在SO文件里
/// 在地图在关闭的时候，去读取全地图的数据信息，保存到SO里
/// </summary>
[ExecuteInEditMode] // 在编辑的时候就运行
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            // 清空一下原有的数据
            if (mapData != null)
            {
                mapData.titleProperties.Clear();
            }
        }
        
    }
    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            // 保存-更新一下数据
            UpdateTitleProperties();
            // ScriptableObject 不保存
            #if UNITY_EDITOR // 只在Unity 编辑器中执行
            if (mapData != null)
            {
                EditorUtility.SetDirty(mapData);
            }
            #endif
        }
    }

    private void UpdateTitleProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                // 绘制瓦片的范围
                Vector3Int startPoint = currentTilemap.cellBounds.min;
                Vector3Int endPoint = currentTilemap.cellBounds.max;
                for (int i = startPoint.x; i < endPoint.x; i++)
                {
                    for (int j = startPoint.y; j < endPoint.y; j++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(i, j, 0));
                        if (tile != null)
                        {
                            TileProperty tileProperty = new TileProperty()
                            {
                                gridType = this.gridType,
                                titleCoordinate = new Vector2Int(i, j),
                                BoolTypeValue = true,
                            };
                            mapData.titleProperties.Add(tileProperty);
                        }
                        
                    }
                }
            }
        }
        
    }


}
