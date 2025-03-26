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

    // Ǯ ��ųʸ�: ������Ʈ �������� Ű��, �ش� �������� Ǯ�� ������ ����
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� �����ɴϴ�. ������ ���� �����մϴ�.
    /// </summary>
    /// <param name="prefab">������ ������</param>
    /// <param name="position">��ġ</param>
    /// <param name="rotation">ȸ��</param>
    /// <param name="parent">�θ� Transform</param>
    /// <returns>Ǯ���� ������ ���� ������Ʈ</returns>
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject obj = null;

        // �ش� �������� Ǯ�� ������ ���� ����
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        // Ǯ�� ��� ������ ������Ʈ�� �ִ��� Ȯ��
        if (objectPools[prefab].Count > 0)
        {
            // Ǯ���� ������Ʈ ��������
            obj = objectPools[prefab].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            // Ǯ�� ��������� ���� ����
            obj = Instantiate(prefab);
        }

        // �θ� ����
        if (parent != null)
        {
            obj.transform.SetParent(parent, false);
        }

        // ��ġ�� ȸ�� ����
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    /// <summary>
    /// ������Ʈ�� Ǯ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="prefab">���� ������</param>
    /// <param name="obj">��ȯ�� ������Ʈ</param>
    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        // ������Ʈ ��Ȱ��ȭ
        obj.SetActive(false);

        // Ǯ�� ������Ʈ �߰�
        objectPools[prefab].Enqueue(obj);
    }

    /// <summary>
    /// Ư�� �������� Ǯ�� �̸� �����մϴ�.
    /// </summary>
    /// <param name="prefab">������ ������</param>
    /// <param name="count">�ʱ⿡ ������ ����</param>
    /// <param name="parent">�θ� Transform</param>
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