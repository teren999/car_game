using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public string playerTag = "Player";
    public float height = 8f;          // Высота камеры над землей
    public float distance = 12f;       // Дистанция от автомобиля
    public float positionSmoothTime = 0.25f; // Время сглаживания позиции
    public Vector3 cameraAngle = new Vector3(30f, 0f, 0f); // Угол наклона камеры

    private Transform target;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Vector3 cameraOffset;

    void Start()
    {
        // Находим автомобиль по тегу
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            target = player.transform;
            // Рассчитываем начальное смещение
            cameraOffset = CalculateCameraOffset();
        }
        else
        {
            Debug.LogError($"Объект с тегом '{playerTag}' не найден!");
        }

        // Устанавливаем начальную позицию и угол камеры
        if (target != null)
        {
            transform.position = target.position + cameraOffset;
            transform.rotation = Quaternion.Euler(cameraAngle);
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Рассчитываем целевую позицию для камеры
        Vector3 desiredPosition = target.position + cameraOffset;
        
        // Плавное перемещение камеры с улучшенным сглаживанием
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            positionSmoothTime
        );
        
        // Гарантируем фиксированный угол обзора
        transform.rotation = Quaternion.Euler(cameraAngle);
    }

    Vector3 CalculateCameraOffset()
    {
        return new Vector3(
            0,              // Смещение по X
            height,         // Высота
            -distance      // Смещение назад по Z
        );
    }
}