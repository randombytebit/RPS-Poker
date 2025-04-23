using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color defaultBtnColor = Color.white;
    public Color selectedBtnColor = Color.yellow;

    // Changes the button and text color to the selected color
    public void SetSelected()
    {
        button.image.color = selectedBtnColor;
        buttonText.color = selectedColor;
    }

    // Changes the button and text color back to the default color
    public void SetDefault()
    {
        button.image.color = defaultBtnColor;
        buttonText.color = defaultColor;
    }
}
