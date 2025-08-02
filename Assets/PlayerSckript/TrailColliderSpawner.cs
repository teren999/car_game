using UnityEngine;

public class TrailColliderSpawner : MonoBehaviour
{
    public float spawnInterval = 0.01f;
    public float colliderLifetime = 1.5f;

    private float timer = 0f;

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= spawnInterval)
        {
            TailObjectPool.Instance.GetFromPool(transform.position, transform.rotation, transform.root, colliderLifetime);
            timer = 0f;
        }
    }
}
