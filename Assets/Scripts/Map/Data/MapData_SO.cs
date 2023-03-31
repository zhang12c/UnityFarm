using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MapData",menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName] public string sceneName;
    public List<TileProperty> titleProperties;
    [Header("地图信息")]
    public int gridWidth;
    public int gridHeight;
    [Header("右下角原点")]
    public int originX;
    public int originY;
}
