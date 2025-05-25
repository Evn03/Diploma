using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class ProgressResponse { public int passedLevel; }
    
    [System.Serializable]
    public class LevelWrapper
    {
        public string name;
        public string difficulty;
        public LevelInnerData data;
        public string level_id;
    }

    [System.Serializable]
    public class LevelInnerData
    {
        public int enemyCount;
        public List<string> map;
    }

    [System.Serializable]
    public class LevelList
    {
        public List<LevelWrapper> levels;
    }

    public GameObject levelButtonPrefab;
    public RectTransform levelContainer;

    private int passedLevel = -1;

    private void Start()
    {
        StartCoroutine(LoadProgressAndLevels());
    }

    private IEnumerator LoadProgressAndLevels()
    {
        string token = UserSession.JwtToken;
        string userId = UserSession.UserId;

        UnityWebRequest progressRequest = UnityWebRequest.Get($"https://hamster-invation.ru/users/{userId}/progress?user_id={userId}");
        progressRequest.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return progressRequest.SendWebRequest();

        if (progressRequest.result == UnityWebRequest.Result.Success)
        {
            ProgressResponse progress = JsonUtility.FromJson<ProgressResponse>(progressRequest.downloadHandler.text);
            passedLevel = progress.passedLevel;
            UserSession.MaxLevel = passedLevel;
        }
        else
        {
            Debug.LogError("Прогресс не загрузился: " + progressRequest.error);
            yield break;
        }

        UnityWebRequest levelRequest = UnityWebRequest.Get("https://hamster-invation.ru/levels/");
        yield return levelRequest.SendWebRequest();

        if (levelRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Уровни не загрузились: " + levelRequest.error);
            yield break;
        }

        string json = "{\"levels\":" + levelRequest.downloadHandler.text + "}";
        LevelList levelList = JsonUtility.FromJson<LevelList>(json);

        levelList.levels.Sort((a, b) =>
        {
            int.TryParse(a.name, out var aNum);
            int.TryParse(b.name, out var bNum);
            return aNum.CompareTo(bNum);
        });

        CreateButtons(levelList.levels);
    }

private void CreateButtons(List<LevelWrapper> levels)
{
    int columns = 5;
    float spacingRatio = 0.02f;
    float sizeRatio = 0.15f; 

    RectTransform container = levelContainer;
    Canvas.ForceUpdateCanvases();

    float canvasWidth = container.rect.width;
    float canvasHeight = container.rect.height;

    float spacing = canvasWidth * spacingRatio;
    float buttonSize = canvasWidth * sizeRatio;

    for (int i = 0; i < levels.Count; i++)
    {
        var level = levels[i];
        GameObject buttonObj = Instantiate(levelButtonPrefab, container);
        RectTransform rect = buttonObj.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(buttonSize, buttonSize);

        int row = i / columns;
        int col = i % columns;

        float x = col * (buttonSize + spacing);
        float y = row * (buttonSize + spacing);
        rect.anchoredPosition = new Vector2(x, -y);

        TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
        text.text = level.name;

        int levelNum = int.TryParse(level.name, out var parsed) ? parsed : -1;
        bool canPlay = (levelNum <= passedLevel + 1);

        Button btn = buttonObj.GetComponent<Button>();
        btn.interactable = canPlay;

        if (canPlay)
        {
            btn.onClick.AddListener(() => OnLevelSelected(level));
        }
        else
        {
            text.color = Color.gray;
        }
    }
}


    private void OnLevelSelected(LevelWrapper level)
    {
        Debug.Log("Выбран уровень: " + level.name);
        UserSession.Level = int.Parse(level.name);

        LevelDataWrapper wrapper = new LevelDataWrapper
        {
            enemyCount = level.data.enemyCount,
            map = level.data.map
        };

        string json = JsonUtility.ToJson(wrapper);
        UserSession.LevelJson = json;

        Debug.Log("JSON уровня сохранён в сессию:\n" + json);

        SceneManager.LoadScene("Game"); 
    }

    [System.Serializable]
    public class LevelDataWrapper
    {
        public int enemyCount;
        public List<string> map;
    }
}