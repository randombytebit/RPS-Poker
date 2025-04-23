using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private RectTransform underlineBar;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Color selectedColor = new Color(0.26f, 0.26f, 0.26f);
    [SerializeField] private Color deselectedColor = new Color(0.69f, 0.69f, 0.69f); 

    private Vector2 originalSize;
    private Coroutine animationCoroutine;

    void Start()
    {
        originalSize = new Vector2(buttonText.rectTransform.rect.width, 2);
        underlineBar.sizeDelta = new Vector2(0, originalSize.y);
        buttonText.color = deselectedColor;
    }

    public void StartUnderlineAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateUnderlineBar());
        buttonText.color = selectedColor;
    }

    public void ResetUnderlineAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        underlineBar.sizeDelta = new Vector2(0, originalSize.y);
        buttonText.color = deselectedColor;
    }

    private IEnumerator AnimateUnderlineBar()
    {
        float elapsedTime = 0f;
        float targetWidth = buttonText.rectTransform.rect.width;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            underlineBar.sizeDelta = new Vector2(progress * targetWidth, originalSize.y);
            yield return null;
        }
        underlineBar.sizeDelta = new Vector2(targetWidth, originalSize.y);
    }
}