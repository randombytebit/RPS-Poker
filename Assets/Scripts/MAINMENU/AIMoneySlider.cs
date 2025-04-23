using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIMoneySlider : MonoBehaviour
{
    [SerializeField] private Slider _moneySlider;
    [SerializeField] private TextMeshProUGUI _moneyText;

    void Start()
    {
        UpdateMoneyText(_moneySlider.value);
        _moneySlider.onValueChanged.AddListener(UpdateMoneyText);
    }

    private void UpdateMoneyText(float value)
    {
        int moneyValue = Mathf.RoundToInt(value / 100) * 100;
        _moneyText.text = value.ToString();
        _moneySlider.value = moneyValue;
    }
}