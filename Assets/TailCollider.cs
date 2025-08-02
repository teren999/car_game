using UnityEngine;

public class TailCollider : MonoBehaviour
{
    private Transform owner;

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || owner == null) return;

        Transform otherRoot = other.transform.root;

        // Игнорируем столкновение с владельцем
        if (otherRoot == owner) return;

        if (otherRoot.CompareTag("Player") || otherRoot.CompareTag("Enemy"))
        {
            Debug.Log("Уничтожен: " + otherRoot.name); // ⬅ Добавим лог
            Destroy(otherRoot.gameObject);
        }
    }
}
