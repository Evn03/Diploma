using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    [System.Serializable]
    public class LoginResponse
    {
        public string access_token;
    }

    [System.Serializable]
    public class AuthPayload
    {
        public string username;
        public string password;

        public AuthPayload(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    [Header("Login UI")]
    public GameObject loginPanel;
    public TMP_InputField loginUsernameField;
    public TMP_InputField loginPasswordField;
    public TMP_Text loginErrorText;
    public Button loginButton;
    public Button goToRegisterButton;

    [Header("Register UI")]
    public GameObject registerPanel;
    public TMP_InputField registerUsernameField;
    public TMP_InputField registerPasswordField;
    public TMP_Text registerErrorText;
    public Button registerButton;
    public Button backToLoginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        goToRegisterButton.onClick.AddListener(ShowRegister);
        registerButton.onClick.AddListener(OnRegisterClicked);
        backToLoginButton.onClick.AddListener(ShowLogin);

        ShowLogin();
    }

    private void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginErrorText.text = "";
        registerErrorText.text = "";
    }

    private void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        loginErrorText.text = "";
        registerErrorText.text = "";
    }

    private void OnLoginClicked()
    {
        loginErrorText.text = "";
        string login = loginUsernameField.text.Trim();
        string password = loginPasswordField.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "Введите логин и пароль.";
            return;
        }

        StartCoroutine(LoginRoutine(login, password));
    }

    private void OnRegisterClicked()
    {
        registerErrorText.text = "";
        string login = registerUsernameField.text.Trim();
        string password = registerPasswordField.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            registerErrorText.text = "Введите логин и пароль.";
            return;
        }

        StartCoroutine(RegisterRoutine(login, password));
    }

    private IEnumerator LoginRoutine(string login, string password)
    {
        loginErrorText.text = "";

        string rawData = $"username={UnityWebRequest.EscapeURL(login)}&password={UnityWebRequest.EscapeURL(password)}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(rawData);

        UnityWebRequest www = new UnityWebRequest("https://hamster-invation.ru/auth/login", "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();
        Debug.Log($"Request Body: {rawData}");
        Debug.Log($"Response Code: {www.responseCode}");
        Debug.Log($"Response Body: {www.downloadHandler.text}");

        if (www.result != UnityWebRequest.Result.Success)
        {
            loginErrorText.text = "Ошибка соединения: " + www.error;
        }
        else if (www.responseCode >= 400)
        {
            loginErrorText.text = "Неверный логин или пароль.";
        }
        else
        {
            string json = www.downloadHandler.text;
            string token = ParseTokenFromJson(json);
            if (!string.IsNullOrEmpty(token))
            {
                UserSession.JwtToken = token;
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                loginErrorText.text = "Не удалось получить токен.";
            }
        }
    }



    private IEnumerator RegisterRoutine(string login, string password)
    {
        var payload = new AuthPayload(login, password);
        string json = JsonUtility.ToJson(payload);

        UnityWebRequest www = new UnityWebRequest("https://hamster-invation.ru/users/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            registerErrorText.text = "Ошибка запроса.";
            Debug.Log(www.error);
        }
        else if (www.responseCode >= 400)
        {
            string response = www.downloadHandler.text;
            if (response.Contains("Username already exists"))
                registerErrorText.text = "Пользователь с таким логином уже существует.";
            else
                registerErrorText.text = "Невозможно зарегистрироваться.";
        }
        else if (www.responseCode == 200)
        {
            registerErrorText.text = "Вы успешно зарегистрированы";
            StartCoroutine(LoginRoutine(login, password));
        }
        else
        {
            registerErrorText.text = www.downloadHandler.text;
        }
    }

    private string ParseTokenFromJson(string json)
    {
        try
        {
            LoginResponse data = JsonUtility.FromJson<LoginResponse>(json);
            string token = data.access_token;

            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                Debug.LogError("Неверный формат токена");
                return null;
            }

            string payload = parts[1];

            // Добавляем padding к base64, если нужно
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            string jsonPayload = System.Text.Encoding.UTF8.GetString(
                System.Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'))
            );

            Debug.Log("JWT payload: " + jsonPayload);

            JwtPayload parsed = JsonUtility.FromJson<JwtPayload>(jsonPayload);
            UserSession.UserId = parsed.sub;
            Debug.Log("User ID: " + UserSession.UserId);

            return token;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка при парсинге токена: " + ex.Message);
            return null;
        }
    }

    [System.Serializable]
    public class JwtPayload
    {
        public string sub;
    }
}