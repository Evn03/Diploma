using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform towerButtonContainer;
    [SerializeField] private GameObject towerButtonPrefab;
    [SerializeField] private List<Tower> availableTowers;


    private int currency = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI(GameManager.Instance.gold);
        GenerateTowerButtons();
        Debug.Log("Количество башен в списке: " + availableTowers.Count);
    }


    public void AddCurrency(int amount)
    {
        GameManager.Instance.AddGold(amount);
    }

    public bool SpendCurrency(int amount)
    {
        return GameManager.Instance.SpendGold(amount);
    }

    public void UpdateGoldUI(int gold)
    {
        if (currencyText != null)
            currencyText.text = "Gold: " + gold;
    }

    public int GetCurrency()
    {
        return GameManager.Instance.gold;
    }
    
    private void GenerateTowerButtons()
    {
        foreach (Tower tower in availableTowers)
        {
            GameObject btnGO = Instantiate(towerButtonPrefab, towerButtonContainer);
            
            TextMeshProUGUI label = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = tower.name;

            Button button = btnGO.GetComponent<Button>();
            Tower localTower = tower;
            button.onClick.AddListener(() => TowerPlacer.Instance.SelectTower(localTower));
        }
    }

}
