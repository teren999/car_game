using UnityEngine;

public class TailCollider : MonoBehaviour
{
    private Transform owner;
    private static readonly System.Collections.Generic.HashSet<Transform> alreadyKilled = new();

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || owner == null) return;

        Transform otherRoot = other.transform.root;

        if (otherRoot == owner) return; // Игнорируем столкновение с собой

        if (alreadyKilled.Contains(otherRoot)) return; // Уже обрабатывался
        alreadyKilled.Add(otherRoot);

        string tag = otherRoot.tag;
        if (tag == "Player" || tag == "Enemy")
        {
           // Debug.Log($"Уничтожен: {otherRoot.name} с тегом: {tag}");
            GameOverManager.Instance.OnPlayerDied(otherRoot.tag);

            Destroy(otherRoot.gameObject);
        }
    }
}
