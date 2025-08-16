using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainButtons; // Родитель кнопок "Играть" и "Магазин"
    public GameObject shopUI;      // Магазин
    public StoreManager storeManager;
    public Text ScoreText;
    public int Score;

    public void Start()
    {
        Score = PlayerPrefs.GetInt("TotalScore", 0);
         ScoreText.text = $"{Score}";
    }

    
    //private void Awake()
  //  {
    //    PlayerPrefs.DeleteAll();
   // PlayerPrefs.Save();
   // }
    public void OpenShop()
    {
        shopUI.SetActive(true);
        mainButtons.SetActive(false);
    }

    public void CloseShop()
    {
        storeManager.OnBackFromShop();
        shopUI.SetActive(false);
        mainButtons.SetActive(true);
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0); // Название сцены с игрой
    }

    public void OnBackButtonPressed()
    {
        storeManager.OnBackFromShop();
    }
}
