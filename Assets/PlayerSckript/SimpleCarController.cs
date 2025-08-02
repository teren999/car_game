using UnityEngine; // Добавляем пространство имён для MonoBehaviour и Rigidbody

public class SimpleCarController : MonoBehaviour
{
    public float maxSpeed = 50f; 
    public float turnSpeed = 150f;
    private Rigidbody rb;
    private float bounceBackTimer = 0f;
    private bool isBouncingBack = false;
    public float bounceBackTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Замораживаем ненужные вращения для стабильности
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (isBouncingBack)
        {
            bounceBackTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = -transform.forward * maxSpeed;

            if (bounceBackTimer <= 0f)
                isBouncingBack = false;

            return;
        }
        // Всегда двигаемся вперёд с максимальной скоростью
        rb.linearVelocity = transform.forward * maxSpeed;

        // Плавный поворот без изменения скорости
        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Trail") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall")|| collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Player"))
        {
            isBouncingBack = true;
            bounceBackTimer = bounceBackTime;
        }
    }
}