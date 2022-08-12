using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteData;
    public List<NPCPosition> NPCPositions = new List<NPCPosition>();


    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();

        InitSceneRouteDict();
    }

    /// <summary>
    /// Init scene route dictionary
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteData.sceneRouteDataList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteData.sceneRouteDataList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                {
                    continue;
                }
                else
                {
                    sceneRouteDict.Add(key, route);
                }

            }
        }
    }

    /// <summary>
    /// Get the path of two scenes
    /// </summary>
    /// <param name="fromSceneName">from Scene Name</param>
    /// <param name="fromSceneName">from Scene Name</param>
    /// <returns>the path of two scenes</returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
