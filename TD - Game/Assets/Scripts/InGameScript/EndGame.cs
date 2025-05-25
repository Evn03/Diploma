using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void ShowVictory()
    {
        winPanel.SetActive(true);
    }

    public void ShowDefeat()
    {
        losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        StartCoroutine(HandleReturn());
    }

    private IEnumerator HandleReturn()
    {
        if (winPanel.activeSelf && UserSession.MaxLevel < UserSession.Level)
        {
            string token = UserSession.JwtToken;
            string userId = UserSession.UserId;

            var payload = new AuthPayload(UserSession.Level);
            string json = JsonUtility.ToJson(payload);

            UnityWebRequest progressRequest = new UnityWebRequest(
                $"https://hamster-invation.ru/users/{userId}/progress?user_id={userId}",
                "PATCH");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            progressRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            progressRequest.downloadHandler = new DownloadHandlerBuffer();

            progressRequest.SetRequestHeader("Authorization", $"Bearer {token}");
            progressRequest.SetRequestHeader("Content-Type", "application/json");

            yield return progressRequest.SendWebRequest();

            if (progressRequest.result == UnityWebRequest.Result.Success)
            {
                ProgressResponse progress = JsonUtility.FromJson<ProgressResponse>(progressRequest.downloadHandler.text);
                UserSession.MaxLevel = progress.passedLevel;
            }
            else
            {
                Debug.LogError("Ошибка при обновлении прогресса: " + progressRequest.error);
            }
        }

        SceneManager.LoadScene("MainMenu");
    }

    [System.Serializable]
    public class AuthPayload
    {
        public int passedLevel;

        public AuthPayload(int passedLevel)
        {
            this.passedLevel = passedLevel;
        }
    }

    [System.Serializable]
    public class ProgressResponse
    {
        public int passedLevel;
    }
}
