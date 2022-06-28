using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    public List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();


    private void OnEnable()
    {
        EventHandler.ParticaleEffectEvent += OnParticaleEffectEvent;
    }

    private void OnDisable()
    {
        EventHandler.ParticaleEffectEvent -= OnParticaleEffectEvent;
    }


    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// Create the Object pools
    /// </summary>
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticaleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        //WORKFLOW: Add pool effect object if have new vfx
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticaleEffectType.LeaveFalling01 => poolEffectList[0],
            ParticaleEffectType.LeaveFalling02 => poolEffectList[1],
            ParticaleEffectType.Rock => poolEffectList[2],
            ParticaleEffectType.ReapableScenery => poolEffectList[3],
            _ => null,
        };
        print("switch");

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutime(objPool, obj));
    }

    private IEnumerator ReleaseRoutime(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }
}
