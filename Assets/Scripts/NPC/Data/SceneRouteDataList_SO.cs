using System.Collections.Generic;
using UnityEngine;
namespace NPC.Data
{
    [CreateAssetMenu(fileName = "SceneRouteDataList",menuName = "NPC/SceneRouteDataList_SO")]
    public class SceneRouteDataList_SO : ScriptableObject
    {
        public List<SceneRoute> sceneRoutes;
    }
}