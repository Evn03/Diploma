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
        UpdateGoldUI(currency);
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateGoldUI(currency);
    }

    public bool SpendCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            UpdateGoldUI(currency);
            return true;
        }
        return false;
    }

    public void UpdateGoldUI(int gold)
    {
        if (currencyText != null)
            currencyText.text = "Gold: " + gold;
    }

    public int GetCurrency()
    {
        return currency;
    }
}
