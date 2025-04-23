using System.Collections;
using TMPro;
using UnityEngine;

public class ScrollingTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI _textMeshPro;
    public float _scrollSpeed = 10f;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;
    private string sourceText;

    private void Awake()
    {
        _rectTransform = _textMeshPro.GetComponent<RectTransform>();
        _parentRectTransform = _rectTransform.parent.GetComponent<RectTransform>();
    }

    private void Start()
    {
        sourceText = _textMeshPro.text;
    }

    public void StartScrolling()
    {
        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        float textWidth = _textMeshPro.preferredWidth;
        float parentWidth = _parentRectTransform.rect.width;
        Vector3 startPosition = _rectTransform.localPosition;

        while (true)
        {
            _rectTransform.localPosition -= new Vector3(_scrollSpeed * Time.deltaTime, 0, 0);

            if (_rectTransform.localPosition.x <= -textWidth)
            {
                _rectTransform.localPosition = new Vector3(parentWidth, startPosition.y, startPosition.z);
            }

            yield return null;
        }
    }
}
