using UnityEngine;

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
            enemy.tag = "Enemy";
        }
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
