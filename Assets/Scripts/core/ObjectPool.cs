using System;
using UnityEngine;
using System.Collections.Generic;

using System.Linq;

internal interface IPoolObject
{
    void onBeforeLock();
    void onBeforeUnlock();
}

public class ObjectPool
{
    private Queue<PoolObject> _free = new Queue<PoolObject>();
    private List<PoolObject> _total = new List<PoolObject>();

    private GameObject _prefab;
    private Transform _root;

    private Action<GameObject> OnCreate;

    public ObjectPool(GameObject prefab, Transform root, int reserveCount, Action<GameObject> onCreate = null)
    {
        _prefab = prefab;
        _root = root;

        OnCreate = onCreate;

        reserve(reserveCount);
    }

    public void reserve(int totalCount, bool add = false)
    {
        if (!add)
        {
            totalCount -= _free.Count;
        }

        while (totalCount > 0)
        {
            --totalCount;

            var data = _create();

            _free.Enqueue(data);
            _total.Add(data);
        }
    }

    public int TotalCount
    {
        get { return _total.Count; }
    }

    public int CreatedCount
    {
        get { return _total.Count - _free.Count; }
    }

    private PoolObject _create()
    {
        var obj = _root.instantiateChild(_prefab, false);

        if (OnCreate != null)
        {
            OnCreate(obj);
        }

        var poolObj = obj.GetComponent<PoolObject>() ?? obj.AddComponent<PoolObject>();

        poolObj.pool = this;
        poolObj.poolCB = obj.GetComponents<IPoolObject>();
        poolObj.prefab = _prefab;

        if (poolObj.poolCB != null)
        {
            try
            {
                for (int i = 0, c = poolObj.poolCB.Length; i < c; ++i)
                {
                    poolObj.poolCB[i].onBeforeUnlock();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }


        return poolObj;
    }

    public void deleteAll(bool onlyUnused = true)
    {
        foreach (var data in _free)
        {
            _total.Remove(data);
            GameObject.Destroy(data.gameObject);
        }

        _free.Clear();

        if (!onlyUnused)
        {
            foreach (var data in _total)
            {
                GameObject.Destroy(data.gameObject);
            }

            _total.Clear();
        }
    }


    public GameObject lockObj()
    {
        PoolObject data;
        if (_free.Count > 0)
        {
            data = _free.Dequeue();
        }
        else
        {
            data = _create();
            _total.Add(data);
        }

        if (data.poolCB != null)
        {
            try
            {
                for (int i = 0, c = data.poolCB.Length; i < c; ++i)
                {
                    data.poolCB[i].onBeforeLock();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        return data.gameObject;
    }

    public void unlockObj(GameObject obj)
    {
        PoolObject data = obj.GetComponent<PoolObject>();

        if (!_total.Contains(data))
        {
            Debug.LogError("ObjectPool: Cannot find object in _total List: pool=" + _prefab.name + " obj=" + obj.name, obj);
            return;
        }

        if (!_free.Contains(data))
        {
            if (data.poolCB != null)
            {
                try
                {
                    for (int i = 0, c = data.poolCB.Length; i < c; ++i)
                    {
                        data.poolCB[i].onBeforeUnlock();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (_root != null) data.transform.SetParent(_root);
            _free.Enqueue(data);
        }
        else
        {
            Debug.LogWarning("ObjectPool: This object is currently in _free Stack. " + obj.name);
        }
    }
}

public class MultiObjectPool
{
    private Dictionary<GameObject, ObjectPool> _pool = new Dictionary<GameObject, ObjectPool>();
    private Transform _root;

    public Transform Root
    {
        get { return _root; }
    }

    private int _reserveCount = 0;

    private Action<GameObject> OnCreate;

    public MultiObjectPool(Transform root, int reserveCount, Action<GameObject> onCreate = null)
    {
        _reserveCount = reserveCount;
        _root = root;
        OnCreate = onCreate;
    }

    public void deleteAll(bool onlyUnused = true)
    {
        foreach (var p in _pool)
        {
            p.Value.deleteAll(onlyUnused);
        }
    }

    public void reserve(string prefabName, int count, bool add = false)
    {
        var prefab = Resources.Load(prefabName) as GameObject;
        if (prefab == null)
        {
            Debug.LogError("MultiObjectPool: Cannot load prefab '" + prefabName + "'!");
            return;
        }

        reserve(prefab, count, add);
    }


    public void reserve(GameObject prefab, int count, bool add = false)
    {
        if (prefab == null)
        {
            Debug.LogError("MultiObjectPool: prefab passed to lockObj function is null.");
            return;
        }

        ObjectPool pool = null;

        if (!_pool.TryGetValue(prefab, out pool))
        {
            pool = new ObjectPool(prefab, _root, count, OnCreate);
            _pool.Add(prefab, pool);
        }
        else
        {
            pool.reserve(count, add);
        }
    }

    public void reserve(Component prefab, int count, bool add = false)
    {
        reserve(prefab.gameObject, count, add);
    }

    public GameObject lockObj(string prefabName)
    {
        var prefab = _pool.Keys.FirstOrDefault(x => x.name == prefabName);
        if (prefab != null) return lockObj(prefab);

        var pathId = prefabName.IndexOf('/');

        if (pathId >= 0)
        {
            var n = prefabName.Substring(pathId + 1);

            prefab = _pool.Keys.FirstOrDefault(x => x.name == n);
            if (prefab != null) return lockObj(prefab);

            prefab = Extensions.loadPrefab(prefabName);
            if (prefab != null) return lockObj(prefab);
        }

        Debug.LogError("MultiObjectPool: Cannot find prefab '" + prefabName + "'!");
        return null;
    }

    public GameObject lockObj(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("MultiObjectPool: prefab passed to lockObj function is null.");
            return null;
        }

        ObjectPool pool = null;

        if (!_pool.TryGetValue(prefab, out pool))
        {
            pool = new ObjectPool(prefab, _root, _reserveCount, OnCreate);
            _pool.Add(prefab, pool);
        }

        return pool.lockObj();
    }

    public T lockObj<T>(T prefab)
        where T : Component
    {
        return lockObj(prefab.gameObject).GetComponent<T>();
    }

    public void unlockObj(GameObject obj)
    {
        var poolObj = obj.GetComponent<PoolObject>();

        if (poolObj == null)
        {
            Debug.LogError("MultiObjectPool: Object " + obj.name + " doesn't contain PoolObject component.");
            return;
        }

        poolObj.pool.unlockObj(obj);
    }

    public void unlockObj(Component obj)
    {
        unlockObj(obj.gameObject);
    }

    public int getTotalCount(GameObject prefab)
    {
        ObjectPool pool = null;

        if (!_pool.TryGetValue(prefab, out pool)) return 0;

        return pool.TotalCount;
    }

    public int getObjectCount(GameObject prefab)
    {
        ObjectPool pool = null;

        if (!_pool.TryGetValue(prefab, out pool)) return 0;

        return pool.CreatedCount;
    }
}