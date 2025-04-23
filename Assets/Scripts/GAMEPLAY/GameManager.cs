using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    // CanvasGroups of menu
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private CanvasGroup _playMenu;
    [SerializeField] private CanvasGroup _settingsMenu;
    [SerializeField] private CanvasGroup _quitMenu;
    [SerializeField] private CanvasGroup _leaderboard;
    [SerializeField] private TextMeshProUGUI[] recordTexts = new TextMeshProUGUI[7];


    public TextMeshProUGUI _aiMoneyText;
    public RoundManager _roundManager;

    // Animation of Start Game, Quit Game and Game Over
    public AnimationManager animationManager;
    public GameObject GameStartAnimation;
    public GameObject QuitGameTextBox;
    public GameObject QuitGameImage;
    public GameObject QuitGameTitle;


    private float _fadeDuration = 0.1f;
    private MenuState _currentMenuState;
    private string _playerName = "Anonymous";
    private int _aidefaultmoney = 500;

    public event Action<string> OnPlayerNameChanged;

    public string playerName {
        get => _playerName;
        set{
            if (_playerName != value){
                _playerName = value;
                OnPlayerNameChanged?.Invoke(_playerName);
            }
        }
    }

    public int AIDefaultMoney {
        get => _aidefaultmoney;
        set => _aidefaultmoney = value;
    }

    public enum MenuState {
        MainMenu,
        PlayMenu,
        SettingsMenu,
        LeaderBoard,
        Quit
    }

    private void Start() {
        SetState(MenuState.MainMenu);
    }

    // function for button call
    public void OnSettingsButtonClicked(string newMenuState) {
        switch(newMenuState) {
            case "MainMenu":
                SetState(MenuState.MainMenu);
                break;
            case "PlayMenu":
                SetState(MenuState.PlayMenu);
                break;
            case "SettingsMenu":
                SetState(MenuState.SettingsMenu);
                break;
            case "LeaderBoard":
                SetState(MenuState.LeaderBoard);
                break;
            case "Quit":
                SetState(MenuState.Quit);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // function for another class call
    public void SetState(MenuState newMenuState) {
        _currentMenuState = newMenuState;
        StopAllCoroutines();

        switch (_currentMenuState) {

            case MenuState.MainMenu:

                // Reset quit animation
                StartCoroutine(animationManager.AnimeReset((QuitGameTextBox), 1080, 60));
                StartCoroutine(animationManager.AnimeReset((QuitGameImage), -1080, 20));

                // Fade in canvas group, interactable and blocksraycasts
                StartCoroutine(FadeInCanvasGroup(_mainMenu));
                _mainMenu.interactable = true;
                _mainMenu.blocksRaycasts = true;

                // disable interactable and blocksraycasts
                _settingsMenu.interactable = false;
                _settingsMenu.blocksRaycasts = false; 
                _playMenu.interactable = false;
                _playMenu.blocksRaycasts = false;
                _quitMenu.interactable = false;
                _quitMenu.blocksRaycasts = false;
                _leaderboard.interactable = false;
                _leaderboard.blocksRaycasts = false;

                // set alpha zero to other canvas groups
                _settingsMenu.alpha = 0;
                _playMenu.alpha = 0;
                _quitMenu.alpha = 0;
                _leaderboard.alpha = 0;


                _aidefaultmoney = _aiMoneyText.text == "" ? _aidefaultmoney : int.Parse(_aiMoneyText.text);
                break;

            case MenuState.PlayMenu:
                // disable interactable and blocksraycasts
                StartCoroutine(FadeOutCanvasGroup(_mainMenu));
                _mainMenu.interactable = false;
                _mainMenu.blocksRaycasts = false;

                // Fade in canvas group, interactable and blocksraycasts
                _playMenu.interactable = true;
                _playMenu.blocksRaycasts = true;

                StartCoroutine(PlayGameStartSequence());
                break;

            case MenuState.SettingsMenu:

                // Fade out canvas group, interactable and blocksraycasts
                StartCoroutine(FadeOutCanvasGroup(_mainMenu));
                _mainMenu.interactable = false;
                _mainMenu.blocksRaycasts = false;

                StartCoroutine(FadeInCanvasGroup(_settingsMenu));
                _settingsMenu.interactable = true;
                _settingsMenu.blocksRaycasts = true;
                break;

            case MenuState.LeaderBoard:
                // Fade out canvas group, interactable and blocksraycasts
                StartCoroutine(FadeOutCanvasGroup(_settingsMenu));
                _settingsMenu.interactable = false;
                _settingsMenu.blocksRaycasts = false;

                StartCoroutine(FadeInCanvasGroup(_leaderboard));
                _leaderboard.interactable = true;
                _leaderboard.blocksRaycasts = true;
                LoadAllGameData();
                break;


            case MenuState.Quit:
                StartCoroutine(FadeOutCanvasGroup(_mainMenu));
                _mainMenu.interactable = false;
                _mainMenu.blocksRaycasts = false;

                StartCoroutine(FadeInCanvasGroup(_quitMenu));
                _quitMenu.interactable = true;
                _quitMenu.blocksRaycasts = true;

                StartCoroutine(QuitGameSequence());
                break;
                
            default:
                throw new ArgumentOutOfRangeException();

        }
    }

    // Sequences
    private IEnumerator PlayGameStartSequence() {
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateRightToLeft(GameStartAnimation, 1080, -1080));
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(FadeInCanvasGroup(_playMenu));
        _roundManager.GameStart();
    }

    private IEnumerator QuitGameSequence() {
        yield return StartCoroutine(animationManager.AnimateStop((QuitGameImage), -1080, -50, Vector2.right));
        yield return StartCoroutine(animationManager.AnimateStop((QuitGameTextBox), 1080, 60, Vector2.left));
    }


    // FadeIn FadeOut Animation
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / _fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / _fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public async void LoadAllGameData()
    {
        try 
        {
            Debug.Log("Starting to load game data...");
            GameAPIClient.GameResponse[] allGames = await GameAPIClient.Instance.GetAllGameDataAsync();
            
            if (allGames != null && allGames.Length > 0)
            {
                Debug.Log($"Retrieved {allGames.Length} game records");
                
                // Sort the array by roundsToWin (ascending order)
                Array.Sort(allGames, (a, b) => a.roundsToWin.CompareTo(b.roundsToWin));
                
                // Clear all record texts first
                foreach (var recordText in recordTexts)
                {
                    if (recordText != null)
                        recordText.text = "";
                }

                // Display up to 7 records
                for (int i = 0; i < Math.Min(allGames.Length, 7); i++)
                {
                    if (recordTexts[i] != null)
                    {
                        var game = allGames[i];
                        recordTexts[i].text = $"Player: {game.finalName}" +
                                            $" Money: ${game.finalMoney}" +
                                            $" Rounds: {game.roundsToWin}";
                    }
                }

                // If we have fewer records than text components, clear the remaining
                for (int i = allGames.Length; i < 7; i++)
                {
                    if (recordTexts[i] != null)
                        recordTexts[i].text = "No Record";
                }
            }
            else
            {
                Debug.LogWarning("No game records found");
                // Show "No Records" in all text components
                foreach (var recordText in recordTexts)
                {
                    if (recordText != null)
                        recordText.text = "No Records";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            // Show error in all text components
            foreach (var recordText in recordTexts)
            {
                if (recordText != null)
                    recordText.text = "Error Loading Data";
            }
        }
    }
}