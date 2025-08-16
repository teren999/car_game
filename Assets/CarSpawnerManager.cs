using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CarSpawnerManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] carPrefabs;
    public Material[] trailMaterials;

    [Header("Spawn Points")]
    public Transform[] enemySpawnPoints;
    public Transform playerSpawnPoint;

    [Header("AI Settings")]
    public float enemySpeed = 30f;
    public float enemyTurnSpeed = 15f;

    [Header("Player Settings")]
    public int playerCarIndex = 0;
    public int playerTrailIndex = 0;

    [Header("Mobile Control")]
    public bool isMobileControl = false; // true — управление с кнопок
    public GameObject mobileControlsParent; // родительский объект с 2 кнопками
    public Button leftButton;
    public Button rightButton;

    void Start()
    {
        LoadPlayerPrefs();
        SpawnPlayer();
        SpawnEnemies();
    }

    void SpawnPlayer()
    {
        GameObject prefab = carPrefabs[playerCarIndex];
        GameObject player = Instantiate(prefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        EnableScript<SimpleCarController>(player, true);
        EnableScript<EnemyAIAdvanced>(player, false);

        ApplyTrailMaterial(player, trailMaterials[playerTrailIndex]);
        player.tag = "Player";
        if (isMobileControl)
{
    mobileControlsParent.SetActive(true);

    SimpleCarController controller = player.GetComponent<SimpleCarController>();

    // Левая кнопка
    EventTrigger leftTrigger = leftButton.gameObject.GetComponent<EventTrigger>();
    if (leftTrigger == null) leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();
    leftTrigger.triggers.Clear();

    EventTrigger.Entry leftDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
    leftDown.callback.AddListener((data) => { controller.SetTurnInput(-1f); });
    leftTrigger.triggers.Add(leftDown);

    EventTrigger.Entry leftUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
    leftUp.callback.AddListener((data) => { controller.SetTurnInput(0f); });
    leftTrigger.triggers.Add(leftUp);

    // Правая кнопка
    EventTrigger rightTrigger = rightButton.gameObject.GetComponent<EventTrigger>();
    if (rightTrigger == null) rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();
    rightTrigger.triggers.Clear();

    EventTrigger.Entry rightDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
    rightDown.callback.AddListener((data) => { controller.SetTurnInput(1f); });
    rightTrigger.triggers.Add(rightDown);

    EventTrigger.Entry rightUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
    rightUp.callback.AddListener((data) => { controller.SetTurnInput(0f); });
    rightTrigger.triggers.Add(rightUp);
}
else
{
    mobileControlsParent.SetActive(false);
}

    }

    void SpawnEnemies()
{
    foreach (var point in enemySpawnPoints)
    {
        int modelIndex = Random.Range(0, carPrefabs.Length);
        int materialIndex = Random.Range(0, trailMaterials.Length);

        GameObject enemy = Instantiate(carPrefabs[modelIndex], point.position, point.rotation);

        EnableScript<SimpleCarController>(enemy, false);
        var ai = EnableScript<EnemyAIAdvanced>(enemy, true);

        if (ai != null)
        {
            ai.maxSpeed = enemySpeed;
            ai.turnSpeed = enemyTurnSpeed;
        }

        ApplyTrailMaterial(enemy, trailMaterials[materialIndex]);
        EnableNameCanvas(enemy); // включаем канвас

        enemy.tag = "Enemy";
        GameOverManager.Instance.Initialize(enemySpawnPoints.Length);
    }
}

void EnableNameCanvas(GameObject car)
{
    Canvas canvas = car.GetComponentInChildren<Canvas>(true); // ищем даже если отключён
    if (canvas != null)
        canvas.gameObject.SetActive(true);
}


    T EnableScript<T>(GameObject obj, bool enable) where T : MonoBehaviour
    {
        T script = obj.GetComponent<T>();
        if (script != null)
            script.enabled = enable;
        return script;
    }

    void ApplyTrailMaterial(GameObject car, Material material)
    {
        TrailRenderer trail = car.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
            trail.material = material;
    }
    void LoadPlayerPrefs()
    {
        playerCarIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        playerTrailIndex = PlayerPrefs.GetInt("SelectedTrail", 0);
    }
}
