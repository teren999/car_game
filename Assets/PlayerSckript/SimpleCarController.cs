using UnityEngine; // Добавляем пространство имён для MonoBehaviour и Rigidbody
using UnityEngine.UI;

public class SimpleCarController : MonoBehaviour
{
    public float maxSpeed = 50f; 
    public float turnSpeed = 150f;
    private Rigidbody rb;
    private float bounceBackTimer = 0f;
    private bool isBouncingBack = false;
    public float bounceBackTime = 0.5f;
    
    private float turnInput = 0f;

    public void SetTurnInput(float input)
    {
     turnInput = input;
    }

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

     rb.linearVelocity = transform.forward * maxSpeed;

     // Если мобильное управление, используем turnInput, иначе — ось
     float input = turnInput != 0f ? turnInput : Input.GetAxis("Horizontal");
     transform.Rotate(0, input * turnSpeed * Time.fixedDeltaTime, 0);
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