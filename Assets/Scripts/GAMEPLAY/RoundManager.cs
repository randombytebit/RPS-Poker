using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    // Private variables for game state
    private string _playerName;
    private int _potMoney = 0;
    private int _playerMoney = 500;
    private int _aiMoney;
    private int _playerBet = 0;
    private int _playerRaiseMoney = 0;
    private int _aiBet = 0;
    private int _aiRaiseMoney = 0;
    private int _smallBlindAmount = 5;
    private string _roundDealer = "none";
    private string _roundWinner = "none";
    private string _currentOptionSelector = "none";
    private List<string> _optionsHistory = new List<string>();
    private int _roundCounts = 0;

    // Public references to UI elements
    public GameObject playerDealerDisplay;
    public GameObject aiDealerDisplay;
    public Card playerSelectedCard;
    public Card aiSelectedCard;
    public Card[] cardSelection;
    public GameManager gameManager;
    public AnimationManager animationManager;
    public TextMeshProUGUI potCountText;
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI aiCountText;
    public GameObject foldAnimation;
    public GameObject callAnimation;
    public GameObject raiseAnimation;
    public GameObject playerWinAnimation;
    public GameObject aiWinAnimation;
    public GameObject tieAnimation;
    public GameObject potPanel;
    public GameObject cardPanel;
    public GameObject aiCardSelectionPanel;
    public GameObject playerCardSelectionPanel;
    public GameObject optionPanel;
    public GameObject raisePanel;
    public GameObject moneyDisplayPanel;
    public GameObject gameOverPanel;
    public GameObject gameOverImage;
    public Slider _raiseSlider;
    public TextMeshProUGUI _raiseText;

    // Game states enumeration
    public enum GameState {
        StartGame,
        RoundInitalization,
        OptionSelection,
        PlayerAction,
        AIAction,
        RoundEnd,
    }

    // Initiates the game by setting the initial game state
    public void GameStart() {
        SetState(GameState.StartGame);
    }

    // Sets the current game state and starts the state handler coroutine
    public void SetState(GameState currentGameState) {
        StopAllCoroutines();
        StartCoroutine(HandleState(currentGameState));
    }
    // Handles different game states and their transitions
    private IEnumerator HandleState(GameState currentGameState) {
        switch (currentGameState) {

            case GameState.StartGame:

                // Initalize value from main menu
                _roundCounts = 0;
                _playerMoney = 500;
                _playerName = gameManager.playerName;
                _aiMoney = gameManager.AIDefaultMoney;
                SetState(GameState.RoundInitalization);
                break;

            case GameState.RoundInitalization:
                // Check gameover or not
                if (_playerMoney <= 0 || _aiMoney <= 0) {
                    StartCoroutine(GameOver());
                } else {
                    _roundCounts++;
                    InitalizeObjects();
                    InitalizeScreen();
                    UpdateMoneyText();
                    ShowCards();
                    SelectDealer();
                    SetBlinds();
                    UpdateMoneyText();
                }
                yield return new WaitForSeconds(1f);
                break;

            case GameState.OptionSelection:
                yield return new WaitForSeconds(1f);
                if (_optionsHistory.Count >= 2) {
                    string lastAction = _optionsHistory[_optionsHistory.Count - 1];
                    string secondLastAction = _optionsHistory[_optionsHistory.Count - 2];

                    if ((secondLastAction == "raise" && lastAction == "call") || (secondLastAction == "call" && lastAction == "call")){
                        ShowCards();
                        yield return new WaitForSeconds(0.5f);
                        CheckWinner();
                        yield return new WaitForSeconds(2f);
                        SetState(GameState.RoundEnd);
                    } else {
                        OptionSelection();
                    }
                } else {
                    OptionSelection();
                }        
                break;

            case GameState.PlayerAction:
                optionPanel.SetActive(true);
                break;

            case GameState.AIAction: 
                StartCoroutine(AIAction());   
                break;

            case GameState.RoundEnd:
                RoundEnd();
                UpdateMoneyText();
                optionPanel.SetActive(false);
                raisePanel.SetActive(false);
                gameOverPanel.SetActive(false);
                aiCardSelectionPanel.SetActive(false);
                playerCardSelectionPanel.SetActive(false);
                potPanel.SetActive(false);
                moneyDisplayPanel.SetActive(false);
                cardPanel.SetActive(false);
                playerDealerDisplay.SetActive(false);
                aiDealerDisplay.SetActive(false);
                yield return new WaitForSeconds(2f);
                SetState(GameState.RoundInitalization);
                break;
        }
    }

    // Determines the winner by comparing player and AI cards
    private void CheckWinner()
    {
        if (playerSelectedCard.Beats(aiSelectedCard)){
            _roundWinner = "player";
            StartCoroutine(PlayerWinSequence());
        } else if (aiSelectedCard.Beats(playerSelectedCard)){
            _roundWinner = "ai";
            StartCoroutine(AIWinSequence());
        } else {
            _roundWinner = "tie";
            StartCoroutine(TieSequence());
        }
    }

    // Resets all game objects and variables for a new round
    private void InitalizeObjects()
    {
        aiCardSelectionPanel.SetActive(false);
        playerCardSelectionPanel.SetActive(false);
        playerDealerDisplay.SetActive(false);
        aiDealerDisplay.SetActive(false);
        _potMoney = 0;
        _playerBet = 0;
        _aiBet = 0;
        _playerRaiseMoney = 0;
        _aiRaiseMoney = 0;
        _roundWinner = "none";
        _currentOptionSelector = "none";
        _optionsHistory.Clear();
        playerSelectedCard.cardData = cardSelection[3].cardData;
        aiSelectedCard.cardData = cardSelection[3].cardData;
    }

    // Sets up the UI elements for a new round
    private void InitalizeScreen()
    {
        optionPanel.SetActive(false);
        raisePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        aiCardSelectionPanel.SetActive(true);
        playerCardSelectionPanel.SetActive(true);
        potPanel.SetActive(true);
        moneyDisplayPanel.SetActive(true);
        cardPanel.SetActive(true);
    }

    // Randomly selects the dealer and updates UI accordingly
    private void SelectDealer()
    {
        bool randDealer = UnityEngine.Random.value > 0.5f;
        if (randDealer)
        {
            _roundDealer = "player";
            playerDealerDisplay.SetActive(true);
            StartCoroutine(FadeIn(playerDealerDisplay, 1f));
        }
        else
        {
            _roundDealer = "ai";
            aiDealerDisplay.SetActive(true);
            StartCoroutine(FadeIn(aiDealerDisplay, 1f));
        }
        _currentOptionSelector = (_roundDealer == "player") ? "player" : "ai";
    }

    // Sets the initial blind bets based on dealer position
    private void SetBlinds()
    {
        if (_roundDealer == "player")
        {
            _aiRaiseMoney = _smallBlindAmount;
            _aiMoney -= _smallBlindAmount;
            _aiBet += _smallBlindAmount;
        }
        else if (_roundDealer == "ai")
        {
            _playerRaiseMoney = _smallBlindAmount;
            _playerMoney -= _smallBlindAmount;
            _playerBet += _smallBlindAmount;
        }
    }

    // Updates the display of player and AI cards
    private void ShowCards()
    {
        if (playerSelectedCard == null || aiSelectedCard == null)
        {
            Debug.LogError($"{(playerSelectedCard == null ? "Player" : "AI")} card is not assigned.");
            return;
        }

        if (playerSelectedCard.cardData == null || playerSelectedCard.cardData.cardImage == null || aiSelectedCard.cardData == null || aiSelectedCard.cardData.cardImage == null)
        {
            Debug.LogError($"{(playerSelectedCard.cardData == null || playerSelectedCard.cardData.cardImage == null ? "Player" : "AI")} card data or image is not assigned.");
            return;
        }

        playerSelectedCard.transform.GetComponent<Image>().sprite = playerSelectedCard.cardData.cardImage;
        aiSelectedCard.transform.GetComponent<Image>().sprite = aiSelectedCard.cardData.cardImage;
    }

    // Updates all money-related displays
    private void UpdateMoneyText()
    {
        UpdatePotMoney();
        UpdateMoneyDisplay();
    }

    // Calculates and updates the pot money
    private void UpdatePotMoney()
    {
        _potMoney = _playerBet + _aiBet;
    }

    // Updates the display of all money values
    private void UpdateMoneyDisplay()
    {
        potCountText.text = _potMoney.ToString();
        playerCountText.text = _playerMoney.ToString();
        aiCountText.text = _aiMoney.ToString();
    }

    // Determines which player should make a decision
    private void OptionSelection()
    {
        if (_currentOptionSelector == "player"){
            Debug.Log("Player do decision");
            SetState(GameState.PlayerAction);
            
        }
        else if (_currentOptionSelector == "ai")
        {
            Debug.Log("Ai do decision");
            SetState(GameState.AIAction);
        }
    }

    // Handles player's card selection and triggers AI response
    public void SelectCard(Card selectedCard)
    {
        playerSelectedCard.cardData = selectedCard.cardData;
        AICardSelection();
        StartCoroutine(FadeOut(playerCardSelectionPanel, 1f));
        StartCoroutine(FadeOut(aiCardSelectionPanel, 1f));
        aiCardSelectionPanel.SetActive(false);
        playerCardSelectionPanel.SetActive(false);
        SetState(GameState.OptionSelection);
    }

    // Randomly selects a card for the AI
    private void AICardSelection()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3);
        aiSelectedCard.cardData = cardSelection[randomIndex].cardData;
    }

    // Distributes pot money based on round winner
    private void RoundEnd()
    {
        Debug.Log("adding money to winner");
        if (_roundWinner == "player")
        {
            _playerMoney += _potMoney;
        }
        else if (_roundWinner == "ai")
        {
            _aiMoney += _potMoney;
        } else if (_roundWinner == "tie"){
            _playerMoney += _playerBet;
            _aiMoney += _aiBet;
        }
    }

    // Handles AI decision making process
    private IEnumerator AIAction()
    {
        float randomActionNumber = UnityEngine.Random.value; 
        if (randomActionNumber < 0.2f){
            yield return StartCoroutine(FoldSequence());
            yield return new WaitForSeconds(2f);
            HandleAction("fold");

        } else if (randomActionNumber >= 0.2f && randomActionNumber < 0.7f){
            yield return StartCoroutine(CallSequence());
            yield return new WaitForSeconds(2f);
            HandleAction("call");
        
        } else if (randomActionNumber >= 0.7f && randomActionNumber < 1f){
            yield return StartCoroutine(RaiseSequence());
            yield return new WaitForSeconds(2f);
            HandleAction("raise");
        }
    }

    // Processes player and AI actions (fold, call, raise)
    public void HandleAction(string action)
    {
        optionPanel.SetActive(false);
        if (action == "fold"){

            if (_currentOptionSelector == "player"){
                _roundWinner = "ai";
            } else if (_currentOptionSelector == "ai"){
                _roundWinner = "player";
            }

            SetState(GameState.RoundEnd);

        } else if (action == "call"){

            if (_currentOptionSelector == "player"){
                if (_playerMoney >= _aiRaiseMoney){
                    _playerMoney -= _aiRaiseMoney;
                    _playerBet += _aiRaiseMoney;
                } else {
                    _playerBet += _playerMoney;
                    _playerMoney = 0;
                }
            } else if (_currentOptionSelector == "ai"){
                if (_aiMoney >= _playerRaiseMoney){
                    _aiMoney -= _playerRaiseMoney;
                    _aiBet += _playerRaiseMoney;
                } else {
                    _aiBet += _aiMoney;
                    _aiMoney = 0;
                }
            }

            UpdateMoneyText();
            _optionsHistory.Add(action);
            _currentOptionSelector = (_currentOptionSelector == "player") ? "ai" : "player";
            SetState(GameState.OptionSelection);

        } else if (action == "raise"){
            if (_currentOptionSelector == "player"){
                if (_playerMoney >= 50){
                    UpdateRaiseText(_raiseSlider.value);
                    _raiseSlider.maxValue = _playerMoney - _playerBet;
                    _raiseSlider.onValueChanged.AddListener(UpdateRaiseText);
                    raisePanel.SetActive(true);
                    if (_roundDealer == "player"){
                        playerDealerDisplay.SetActive(false);
                    } else if (_roundDealer == "ai"){
                        aiDealerDisplay.SetActive(false);
                    }
                    potPanel.SetActive(false);
                    moneyDisplayPanel.SetActive(false);
                    cardPanel.SetActive(false);
                } else {
                    SetState(GameState.OptionSelection);
                }

            } else if (_currentOptionSelector == "ai"){
                float randomRaiseValue = UnityEngine.Random.Range(100, _aiMoney - _aiBet);
                int raiseValue = Mathf.RoundToInt(randomRaiseValue / 100) * 100;
                _aiMoney -= raiseValue;
                _aiBet += raiseValue;
                _aiRaiseMoney += raiseValue;
                UpdateMoneyText();
                _optionsHistory.Add("raise");
                _currentOptionSelector = (_currentOptionSelector == "player") ? "ai" : "player";
                SetState(GameState.OptionSelection);
            }
        }
    }

    // Processes the raise action and updates UI
    public void Raise()
    {
        raisePanel.SetActive(false);
        _playerMoney -= Mathf.RoundToInt(_raiseSlider.value);
        _playerBet += Mathf.RoundToInt(_raiseSlider.value);
        _playerRaiseMoney += Mathf.RoundToInt(_raiseSlider.value);

        UpdateMoneyText();
        _optionsHistory.Add("raise");
        _currentOptionSelector = (_currentOptionSelector == "player") ? "ai" : "player";
        if (_roundDealer == "player"){
            playerDealerDisplay.SetActive(true);
        } else if (_roundDealer == "ai"){
            aiDealerDisplay.SetActive(true);
        }
        cardPanel.SetActive(true);
        potPanel.SetActive(true);
        moneyDisplayPanel.SetActive(true);
        SetState(GameState.OptionSelection);
    }

    // Updates the raise slider text and value
    private void UpdateRaiseText(float value)
    {
        int raiseValue = Mathf.RoundToInt(value / 100) * 100;
        _raiseText.text = value.ToString();
        _raiseSlider.value = raiseValue;
    }

    // Handles the game over sequence and transitions
    private IEnumerator GameOver() {
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(animationManager.AnimateLeftToRight(gameOverImage, -1080, 1080));
        yield return new WaitForSeconds(1.75f);

        // Deactivate panels
        optionPanel.SetActive(false);
        raisePanel.SetActive(false);
        aiCardSelectionPanel.SetActive(false);
        playerCardSelectionPanel.SetActive(false);
        potPanel.SetActive(false);
        moneyDisplayPanel.SetActive(false);
        cardPanel.SetActive(false);

        // Activate gameOverPanel
        gameOverPanel.SetActive(true);
        TextMeshProUGUI[] textMessages = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (_playerMoney <= 0) {
            Debug.Log("Player Money is 0 or less. AI Wins!");
            textMessages[0].text = "AI Wins!";
        } else if (_aiMoney <= 0) {
            UploadData();
            Debug.Log("AI Money is 0 or less. Player Wins!");
            textMessages[0].text = "Player Wins!";
        }

        // Perform fade animations
        yield return StartCoroutine(FadeOut(textMessages[0].gameObject, 1f));
        yield return StartCoroutine(FadeIn(textMessages[1].gameObject, 1f));

        // Wait for a few seconds before returning to the main menu
        yield return new WaitForSeconds(3f);
        gameManager.SetState(GameManager.MenuState.MainMenu);
    }

    // Fades in a UI element over specified duration
    private IEnumerator FadeIn(GameObject target, float duration)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    // Fades out a UI element over specified duration
    private IEnumerator FadeOut(GameObject target, float duration)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator CallSequence() {
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateRightToLeft(callAnimation, 1200, -1200));
    }

    private IEnumerator FoldSequence() {
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateLeftToRight(foldAnimation, -1200, 1200));
    }

    private IEnumerator RaiseSequence() {
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateRightToLeft(raiseAnimation, 1200, -1200));
    }

    private IEnumerator PlayerWinSequence(){
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateLeftToRight(playerWinAnimation, -1200, 1200));
    }

    private IEnumerator AIWinSequence(){
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateRightToLeft(aiWinAnimation, 1200, -1200));
    }

    private IEnumerator TieSequence(){
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(animationManager.AnimateLeftToRight(tieAnimation, -1200, 1200));
    }

    private async void UploadData(){
        try{
                var gameData = new GameAPIClient.GameData
                {
                    finalName = _playerName,     
                    finalMoney = _playerMoney,   
                    roundsToWin = _roundCounts
                };

              // Use the singleton instance
              string response = await GameAPIClient.Instance.PostDataAsync("savegame", gameData);
              Debug.Log($"Game result saved to server: {response}");
        } catch (System.Exception e){
              Debug.LogError($"Failed to save game result: {e.Message}");
        }
    }

}
