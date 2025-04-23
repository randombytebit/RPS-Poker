using UnityEngine;
using TMPro;
using System.Collections;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textmeshPro;
    [SerializeField] float timeBtwnChars;
    public GameManager gameManager;
    public ScrollingTextMeshPro scrollingTextMeshPro;

    private void Start()
    {
        // Subscribe to the OnPlayerNameChanged event
        gameManager.OnPlayerNameChanged += UpdateWelcomeMessage;

        // Initialize the welcome message
        UpdateWelcomeMessage(gameManager.playerName);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnPlayerNameChanged event
        gameManager.OnPlayerNameChanged -= UpdateWelcomeMessage;
    }

    private void UpdateWelcomeMessage(string playerName)
    {
        string finalMessage = "WELCOME BACK " + playerName.ToString().ToUpper();

        if (finalMessage.Length > 0)
        {
            _textmeshPro.text = finalMessage;
            StartCoroutine(TextVisible(finalMessage));
        }
    }

    private IEnumerator TextVisible(string finalMessage)
    {
        _textmeshPro.ForceMeshUpdate();
        int totalvisibleCharacters = _textmeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visbleCount = counter % (totalvisibleCharacters + 1);
            _textmeshPro.maxVisibleCharacters = visbleCount;

            if (visbleCount >= totalvisibleCharacters)
            {
                scrollingTextMeshPro.StartScrolling();
                break;
            }

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }
}
