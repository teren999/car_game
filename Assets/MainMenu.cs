using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainButtons; // Родитель кнопок "Играть" и "Магазин"
    public GameObject shopUI;      // Магазин
    public StoreManager storeManager;

    
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
