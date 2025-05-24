using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
     private const string VK_APP_ID = "ваш_app_id";
    private const string REDIRECT_URI = "mygame://vk_auth"; // Должен совпадать с настройками VK
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void StartVKAuth()
    {
        string authUrl = $"https://oauth.vk.com/authorize?client_id={VK_APP_ID}&display=page&redirect_uri={REDIRECT_URI}&response_type=token&v=5.131";
        Application.OpenURL(authUrl);
    }
}