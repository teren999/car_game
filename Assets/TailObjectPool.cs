using System.Collections.Generic;
using UnityEngine;

public class TailObjectPool : MonoBehaviour
{
    public static TailObjectPool Instance;

    public GameObject tailPrefab;
    public int poolSize = 100;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(tailPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool(Vector3 position, Quaternion rotation, Transform owner, float lifetime)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(tailPrefab);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        // передаём владельца
        var tailScript = obj.GetComponent<TailCollider>();
        if (tailScript != null) tailScript.SetOwner(owner);

        StartCoroutine(ReturnToPool(obj, lifetime));
        return obj;
    }

    private System.Collections.IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
