using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAIAdvanced : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 50f;
    public float turnSpeed = 150f;
    public float avoidDistance = 25f;
    public float randomTurnInterval = 2f;
    public float randomTurnStrength = 0.5f;

    [Header("Borders")]
    public Vector2 areaMin = new Vector2(-181.1f, 4.2f);
    public Vector2 areaMax = new Vector2(-14.5f, 174.9f);

    [Header("Bounce Back")]
    public float bounceBackTime = 0.5f;

    private float timeSinceLastRandomTurn = 0f;
    private float currentRandomTurn = 0f;
    private float bounceBackTimer = 0f;
    private bool isBouncingBack = false;

    private Rigidbody rb;
    private int obstacleMask;

    private enum AvoidDirection { None, Left, Right }
    private AvoidDirection currentAvoid = AvoidDirection.None;

    private bool forcedAvoidAll = false;
private AvoidDirection forcedAvoidDirection = AvoidDirection.None;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        obstacleMask = LayerMask.GetMask("Wall", "Trail");
        PickNewRandomTurn();
    }

    void FixedUpdate()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        // Bounce-back logic
        if (isBouncingBack)
        {
            bounceBackTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = -transform.forward * maxSpeed;

            if (bounceBackTimer <= 0f)
                isBouncingBack = false;

            return;
        }

        // Stay within play area
        Vector3 pos = transform.position;
        if (pos.x < areaMin.x || pos.x > areaMax.x || pos.z < areaMin.y || pos.z > areaMax.y)
        {
            Vector3 toCenter = new Vector3(
                Mathf.Clamp(pos.x, areaMin.x, areaMax.x),
                pos.y,
                Mathf.Clamp(pos.z, areaMin.y, areaMax.y)
            ) - pos;

            Quaternion rotateToCenter = Quaternion.LookRotation(toCenter);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateToCenter, turnSpeed * Time.fixedDeltaTime);

            rb.linearVelocity = transform.forward * maxSpeed;
            return;
        }

        // Debug rays
        Debug.DrawRay(rayOrigin, transform.forward * avoidDistance, Color.red);
        Debug.DrawRay(rayOrigin, Quaternion.Euler(0, -30, 0) * transform.forward * avoidDistance, Color.blue);
        Debug.DrawRay(rayOrigin, Quaternion.Euler(0, 30, 0) * transform.forward * avoidDistance, Color.green);

        // Обработка препятствий
        AvoidDirection detected = CheckForObstacles(rayOrigin);

        if (detected != AvoidDirection.None)
        {
            currentAvoid = detected; // Запоминаем уклонение
        }
        else if (currentAvoid != AvoidDirection.None)
        {
            // Проверяем, можно ли прекратить уклоняться
            Vector3 checkDir = currentAvoid == AvoidDirection.Left
                ? Quaternion.Euler(0, -30, 0) * transform.forward
                : Quaternion.Euler(0, 30, 0) * transform.forward;

            if (!Physics.Raycast(rayOrigin, checkDir, avoidDistance, obstacleMask))
            {
                currentAvoid = AvoidDirection.None;
            }
        }

        float turn = 0f;

        if (currentAvoid != AvoidDirection.None)
        {
            // Активное уклонение — случайные повороты игнорируются
            turn = currentAvoid == AvoidDirection.Left ? 1f : -1f;
        }
        else
        {
            // Случайные колебания только если безопасно
            timeSinceLastRandomTurn += Time.fixedDeltaTime;
            if (timeSinceLastRandomTurn >= randomTurnInterval)
            {
                PickNewRandomTurn();
                timeSinceLastRandomTurn = 0f;
            }

            turn = currentRandomTurn;
        }

        // Движение и поворот
        rb.linearVelocity = transform.forward * maxSpeed;
        transform.Rotate(0, turn * turnSpeed * Time.fixedDeltaTime, 0);
    }

   AvoidDirection CheckForObstacles(Vector3 origin)
{
    RaycastHit hit;
    bool left = Physics.Raycast(origin, Quaternion.Euler(0, -30, 0) * transform.forward, out hit, avoidDistance, obstacleMask);
    bool right = Physics.Raycast(origin, Quaternion.Euler(0, 30, 0) * transform.forward, out hit, avoidDistance, obstacleMask);
    bool center = Physics.Raycast(origin, transform.forward, out hit, avoidDistance, obstacleMask);

    // Сохраняем "состояние всех лучей"
    if (left && right && center)
    {
        if (!forcedAvoidAll)
        {
            forcedAvoidAll = true;
            forcedAvoidDirection = Random.value < 0.5f ? AvoidDirection.Left : AvoidDirection.Right;
        }

        return forcedAvoidDirection;
    }

    // Если ранее был выбран forcedAvoid — проверим, можно ли выйти
    if (forcedAvoidAll)
    {
        if (!(left && right && center))
        {
            // Всё, вышли из критической ситуации
            forcedAvoidAll = false;
            forcedAvoidDirection = AvoidDirection.None;
        }
        else
        {
            // Продолжаем избегать
            return forcedAvoidDirection;
        }
    }

    // Остальная обычная логика
    if (left && center) return AvoidDirection.Left;
    if (right && center) return AvoidDirection.Right;
    if (left) return AvoidDirection.Left;
    if (right) return AvoidDirection.Right;
    if (center) return Random.value < 0.5f ? AvoidDirection.Left : AvoidDirection.Right;

    return AvoidDirection.None;
}


    void PickNewRandomTurn()
    {
        currentRandomTurn = Random.Range(-randomTurnStrength, randomTurnStrength);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Trail") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Player"))
        {
            isBouncingBack = true;
            bounceBackTimer = bounceBackTime;
        }
    }
}
