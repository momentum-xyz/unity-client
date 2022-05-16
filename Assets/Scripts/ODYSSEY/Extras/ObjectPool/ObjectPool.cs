using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public class ObjectPool
    {
        public ObjectPool(GameObject prefab, int initialeSize = 0)
        {
            _pool = new Stack<GameObject>(initialeSize);
            _prefab = prefab;
        }

        public GameObject Instantiate(string name)
        {
            GameObject go;
            if (_pool.Count == 0)
                go = GameObject.Instantiate(_prefab);
            else
                go = _pool.Pop();

            go.name = name;
            go.SetActive(true);

            return go;
        }

        public GameObject Instantiate(string name, Vector3 pos, Quaternion rot)
        {
            GameObject go = Instantiate(name);

            go.transform.position = pos;
            go.transform.rotation = rot;

            return go;
        }

        public void Destroy(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(null);
            _pool.Push(go);
        }

        public void Clear()
        {
            foreach (GameObject go in _pool)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(go);
#else
                GameObject.Destroy(go);
#endif
            }

            _pool.Clear();
        }

        readonly GameObject _prefab;
        readonly Stack<GameObject> _pool;
    }
}