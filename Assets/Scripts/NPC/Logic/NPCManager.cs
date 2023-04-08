using System;
using System.Collections.Generic;
using NPC.Data;
using NPC.Logic;
using Utility;
namespace NPC
{
    public class NPCManager : Singleton<NPCManager>
    {
        public List<NPCPosition> npcList = new List<NPCPosition>();

        public SceneRouteDataList_SO sceneRouteDataListSo;

        private readonly Dictionary<string, SceneRoute> _sceneRouteDict = new Dictionary<string, SceneRoute>();

        protected override void Awake()
        {
            base.Awake();
            InitSceneRouteDic();
        }

        private void OnEnable()
        {
            MyEventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.StartNewGameEvent -= OnStartNewGameEvent;

        }
        private void OnStartNewGameEvent(int obj)
        {
            foreach (NPCPosition cNpc in npcList)
            {
                cNpc.npc.position = cNpc.position;
                cNpc.npc.GetComponent<NPCMovement>().StartScene = cNpc.startScene;
            }
        }

        private void InitSceneRouteDic()
        {
            if (sceneRouteDataListSo.sceneRoutes.Count > 0)
            {
                foreach (SceneRoute sceneRoute in sceneRouteDataListSo.sceneRoutes)
                {
                    var key = sceneRoute.fromSceneName + sceneRoute.toSceneName;
                    if (_sceneRouteDict.ContainsKey(key))
                    {
                        //sceneRouteDict[key] = sceneRoute;
                        continue;
                    }
                    else
                    {
                        _sceneRouteDict.Add(key,sceneRoute);
                    }
                }
            }
        }

        /// <summary>
        /// 查
        /// </summary>
        /// <param name="fromSceneName"></param>
        /// <param name="toSceneName"></param>
        /// <returns></returns>
        public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
        {
            return _sceneRouteDict[fromSceneName + toSceneName];
        }
    }
}