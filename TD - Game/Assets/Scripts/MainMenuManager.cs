using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform targetObject;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private Vector2 positionOffset = new Vector2(-879f, 0f);
    [SerializeField] private float moveDuration = 1f;

    private Vector2 originalPosition; 
    private bool isAtOriginalPosition = true;

    private void Awake()
    {
        originalPosition = targetObject.anchoredPosition;
        targetPosition = originalPosition + positionOffset;
    }

    public void TogglePosition()
    {
        Vector2 destination = isAtOriginalPosition ? targetPosition : originalPosition;
        StartCoroutine(MoveCoroutine(destination));
        isAtOriginalPosition = !isAtOriginalPosition; 
    }
    public void LogOut()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    private IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        Vector2 startPos = targetObject.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            targetObject.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        targetObject.anchoredPosition = targetPos;
    }
}
