using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    private int pendingTrailIndex = -1;

    [Header("3D-просмотр")]
    public Transform carPreviewRoot;

    [Header("Массивы")]
    public GameObject[] carPrefabs;
    public Material[] trailMaterials;

    [Header("UI")]
    public GameObject carItemPrefab;
    public GameObject trailItemPrefab;
    public Transform carContentParent;
    public Transform trailContentParent;
    public GameObject lockIcon;
    public GameObject buyPanel;
    public Button buyButton;
    public Text buyPriceText;
    public Text rewardCounterText;
    public GameObject trailBuyPanel;
    public Image trailBuyImage;
    public Button trailBuyButton;
    public Button trailBuyCloseButton;

    [Header("Настройки")]
    public Sprite[] carSprites;
    public Sprite[] trailSprites;
    public int carPrice = 3;
    public int trailPrice = 1;

    private int selectedCarIndex;
    private int selectedTrailIndex;
    private int lastUnlockedCarIndex;

    private GameObject currentPreviewCar;

    private List<Button> carButtons = new List<Button>();
    private List<Button> trailButtons = new List<Button>();

    public int rewardCoins = 0;

    void Start()
    {
        rewardCoins = PlayerPrefs.GetInt("RewardCoin", 0);
        rewardCounterText.text = rewardCoins.ToString();
        LoadData();
        PopulateShop();
        ShowSelectedCar();
    }

    void LoadData()
    {
        selectedCarIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        selectedTrailIndex = PlayerPrefs.GetInt("SelectedTrail", 0);

        if (IsCarUnlocked(selectedCarIndex))
            lastUnlockedCarIndex = selectedCarIndex;
        else
            lastUnlockedCarIndex = 0;
    }

    void PopulateShop()
    {
        for (int i = 0; i < carPrefabs.Length; i++)
        {
            GameObject btnObj = Instantiate(carItemPrefab, carContentParent);
            btnObj.GetComponent<Image>().sprite = carSprites[i];

            int index = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectCar(index));
            btnObj.transform.Find("Lock").gameObject.SetActive(!IsCarUnlocked(i));

            carButtons.Add(btnObj.GetComponent<Button>());
        }

        for (int i = 0; i < trailMaterials.Length; i++)
        {
            GameObject btnObj = Instantiate(trailItemPrefab, trailContentParent);
            btnObj.GetComponent<Image>().sprite = trailSprites[i];

            int index = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectTrail(index));
            btnObj.transform.Find("Lock").gameObject.SetActive(!IsTrailUnlocked(i));

            trailButtons.Add(btnObj.GetComponent<Button>());
        }
    }

    void ShowSelectedCar()
    {
        if (currentPreviewCar != null) Destroy(currentPreviewCar);

        currentPreviewCar = Instantiate(carPrefabs[selectedCarIndex], Vector3.zero, Quaternion.identity, carPreviewRoot);
        currentPreviewCar.transform.localPosition = Vector3.zero;
        currentPreviewCar.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Отключаем TrailColliderSpawner в режиме превью
        TrailColliderSpawner tcs = currentPreviewCar.GetComponent<TrailColliderSpawner>();
        if (tcs != null) tcs.enabled = false;

        var renderer = currentPreviewCar.GetComponentInChildren<TrailRenderer>();
        renderer.material = trailMaterials[selectedTrailIndex];

        bool carLocked = !IsCarUnlocked(selectedCarIndex);
        lockIcon.SetActive(carLocked);

        HighlightSelected(carButtons, selectedCarIndex);
        HighlightSelected(trailButtons, selectedTrailIndex);
    }

    void HighlightSelected(List<Button> buttons, int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var outline = buttons[i].GetComponent<Outline>();
            outline.enabled = (i == index);
        }
    }

    bool IsCarUnlocked(int index) => PlayerPrefs.GetInt($"CarUnlocked_{index}", index == 0 ? 1 : 0) == 1;
    bool IsTrailUnlocked(int index) => PlayerPrefs.GetInt($"TrailUnlocked_{index}", index == 0 ? 1 : 0) == 1;

    void SelectCar(int index)
    {
        selectedCarIndex = index;

        if (!IsCarUnlocked(index))
        {
            ShowBuyPanel("машину", carPrice, () =>
            {
                UnlockCar(index);
                lastUnlockedCarIndex = index;
                SaveSelection();
                UpdateCarLocks();
                ShowSelectedCar();
            });
        }
        else
        {
            lastUnlockedCarIndex = index;
            SaveSelection();
            buyPanel.SetActive(false);
        }

        ShowSelectedCar();
    }

   void SelectTrail(int index)
{
    if (!IsTrailUnlocked(index))
    {
        pendingTrailIndex = index;
        trailBuyPanel.SetActive(true);
        trailBuyImage.sprite = trailSprites[index];

        trailBuyButton.onClick.RemoveAllListeners();
        if (rewardCoins >= trailPrice)
        {
            trailBuyButton.GetComponentInChildren<Text>().text = "Купить";
            trailBuyButton.onClick.AddListener(() =>
            {
                SpendRewardCoins(trailPrice);
                UnlockTrail(pendingTrailIndex);
                selectedTrailIndex = pendingTrailIndex;
                SaveSelection();
                trailBuyPanel.SetActive(false);
                UpdateTrailLocks();
                ShowSelectedCar();
            });
        }
        else
        {
            trailBuyButton.GetComponentInChildren<Text>().text = "Смотреть рекламу";
            trailBuyButton.onClick.AddListener(() =>
            {
                AddRewardCoin();
                if (rewardCoins >= trailPrice)
                {
                    // Автоматически активируем кнопку "Купить"
                    SelectTrail(pendingTrailIndex);
                }
            });
        }

        trailBuyCloseButton.onClick.RemoveAllListeners();
        trailBuyCloseButton.onClick.AddListener(() =>
        {
            trailBuyPanel.SetActive(false);
            pendingTrailIndex = -1;
            ShowSelectedCar(); // Обновим UI чтобы вернуть подсветку последнего купленного
        });
    }
    else
    {
        selectedTrailIndex = index;
        SaveSelection();
        ShowSelectedCar();
    }
}



    void ShowBuyPanel(string itemType, int price, System.Action onBuy)
{
    buyPanel.SetActive(true);
    buyPriceText.text = $"{itemType} за {price} RewardCoin";

    void UpdateBuyButton()
    {
        buyButton.onClick.RemoveAllListeners();

        if (rewardCoins >= price)
        {
            buyButton.GetComponentInChildren<Text>().text = "Купить";
            buyButton.onClick.AddListener(() =>
            {
                SpendRewardCoins(price);
                onBuy.Invoke();
                UpdateCarLocks();
                buyPanel.SetActive(false);
            });
        }
        else
        {
            buyButton.GetComponentInChildren<Text>().text = "Смотреть рекламу";
            buyButton.onClick.AddListener(() =>
            {
                AddRewardCoin();
                UpdateBuyButton(); // <-- пересчитываем кнопку
            });
        }
    }

    UpdateBuyButton();
}


    void UnlockCar(int index) => PlayerPrefs.SetInt($"CarUnlocked_{index}", 1);
    void UnlockTrail(int index) => PlayerPrefs.SetInt($"TrailUnlocked_{index}", 1);

    void SaveSelection()
    {
        PlayerPrefs.SetInt("SelectedCar", selectedCarIndex);
        PlayerPrefs.SetInt("SelectedTrail", selectedTrailIndex);
    }

    void SpendRewardCoins(int amount)
    {
        rewardCoins -= amount;
        PlayerPrefs.SetInt("RewardCoin", rewardCoins);
        rewardCounterText.text = rewardCoins.ToString();
    }

    void AddRewardCoin()
    {
        rewardCoins++;
        PlayerPrefs.SetInt("RewardCoin", rewardCoins);
        rewardCounterText.text = rewardCoins.ToString();
    }

    public void OnBackFromShop()
    {
        if (!IsCarUnlocked(selectedCarIndex))
        {
            selectedCarIndex = lastUnlockedCarIndex;
            SaveSelection();
            ShowSelectedCar();
        }
    }

    void UpdateCarLocks()
    {
        for (int i = 0; i < carButtons.Count; i++)
        {
            carButtons[i].transform.Find("Lock").gameObject.SetActive(!IsCarUnlocked(i));
        }
    }

    void UpdateTrailLocks()
    {
        for (int i = 0; i < trailButtons.Count; i++)
        {
            trailButtons[i].transform.Find("Lock").gameObject.SetActive(!IsTrailUnlocked(i));
        }
    }

    [ContextMenu("Reset All Data")]
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    

}
