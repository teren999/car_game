using UnityEngine;
using UnityEngine.UI; // Добавляем для работы с Text

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Text nicknameText; // Ссылка на компонент Text

    // Массив с никнеймами (40 вариантов)
    private string[] nicknames = new string[]
    {
        "ShadowHunter", "CyberNinja", "QuantumLeap", "NeonPhantom", "SteelTitan",
        "DriftKing", "PixelPirate", "CosmicWolf", "ToxicAvenger", "DigitalGhost",
        "VortexRider", "BlazeStorm", "IronFist", "NightCrawler", "PhantomX",
        "RustyBullet", "DarkKnight", "SolarFlare", "NetRunner", "FrostBite",
        "КиберВоин", "ТемныйЛорд", "СтальнойКлинок", "Невидимка", "Космодесант",
        "Виртуальный", "КвантовыйКот", "ЯдерныйГриб", "Безымянный", "Скорость",
        "ОгненныйШар", "ЛедянойВзгляд", "Механический", "ТеневойБег", "Ракетчик",
        "Гравитация", "Электроник", "ВихрьАтаки", "ПесчаныйШторм", "Абсолют"
    };

    void Start()
    {
        mainCamera = Camera.main;
        
        // Получаем компонент Text
        nicknameText = GetComponentInChildren<Text>();
        
        // Если Text найден, выбираем случайный никнейм
        if (nicknameText != null)
        {
            int randomIndex = Random.Range(0, nicknames.Length);
            nicknameText.text = nicknames[randomIndex];
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Поворачивает объект к камере
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }
}