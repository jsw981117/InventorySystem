using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPoolManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ObjectPoolManager");
                    _instance = obj.AddComponent<ObjectPoolManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    // 풀 딕셔너리: 오브젝트 프리팹을 키로, 해당 프리팹의 풀을 값으로 가짐
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        // 싱글톤 초기화
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옵니다. 없으면 새로 생성합니다.
    /// </summary>
    /// <param name="prefab">생성할 프리팹</param>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <param name="parent">부모 Transform</param>
    /// <returns>풀에서 가져온 게임 오브젝트</returns>
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject obj = null;

        // 해당 프리팹의 풀이 없으면 새로 생성
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        // 풀에 사용 가능한 오브젝트가 있는지 확인
        if (objectPools[prefab].Count > 0)
        {
            // 풀에서 오브젝트 가져오기
            obj = objectPools[prefab].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            // 풀이 비어있으면 새로 생성
            obj = Instantiate(prefab);
        }

        // 부모 설정
        if (parent != null)
        {
            obj.transform.SetParent(parent, false);
        }

        // 위치와 회전 설정
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    /// <summary>
    /// 오브젝트를 풀로 반환합니다.
    /// </summary>
    /// <param name="prefab">원본 프리팹</param>
    /// <param name="obj">반환할 오브젝트</param>
    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        // 오브젝트 비활성화
        obj.SetActive(false);

        // 풀에 오브젝트 추가
        objectPools[prefab].Enqueue(obj);
    }

    /// <summary>
    /// 특정 프리팹의 풀을 미리 생성합니다.
    /// </summary>
    /// <param name="prefab">생성할 프리팹</param>
    /// <param name="count">초기에 생성할 개수</param>
    /// <param name="parent">부모 Transform</param>
    public void PreWarm(GameObject prefab, int count, Transform parent = null)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            objectPools[prefab].Enqueue(obj);
        }
    }
}