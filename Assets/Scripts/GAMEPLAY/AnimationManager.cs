using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // Speed of movement in pixels per second

    // Coroutine that handles the right to left movement animation
    public IEnumerator AnimateLeftToRight(GameObject moveObject, float startXPosition, float endXPosition) {
        Debug.Log($"Starting LeftToRight animation for {moveObject.name} from {startXPosition} to {endXPosition}");
        moveObject.SetActive(true);
        RectTransform rectTransform = moveObject.GetComponent<RectTransform>();
        bool hasPaused = false;
        
        Vector2 startPosition = rectTransform.anchoredPosition;
        startPosition.x = startXPosition;
        rectTransform.anchoredPosition = startPosition;
        Debug.Log($"Initial position set to: {startPosition}");
        
        while (rectTransform.anchoredPosition.x < endXPosition) {
            Vector2 position = rectTransform.anchoredPosition;

            if (!hasPaused && position.x <= 10 && position.x >= -10) {
                Debug.Log($"Pausing at center position: {position}");
                yield return new WaitForSeconds(0.5f);
                hasPaused = true;
            }
            
            position.x += moveSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = position;
            
            if (Time.frameCount % 30 == 0) // Log every 30 frames to avoid spam
                Debug.Log($"Current position: {position}");
                
            yield return null;
        }

        Vector2 endPosition = rectTransform.anchoredPosition;
        endPosition.x = endXPosition;
        rectTransform.anchoredPosition = endPosition;
        Debug.Log($"Animation completed. Final position: {endPosition}");
        moveObject.SetActive(false);
    }

    public IEnumerator AnimateRightToLeft(GameObject moveObject, float startXPosition, float endXPosition) {
        Debug.Log($"Starting RightToLeft animation for {moveObject.name} from {startXPosition} to {endXPosition}");
        moveObject.SetActive(true);
        RectTransform rectTransform = moveObject.GetComponent<RectTransform>();
        bool hasPaused = false;
        
        Vector2 startPosition = rectTransform.anchoredPosition;
        startPosition.x = startXPosition;
        rectTransform.anchoredPosition = startPosition;
        Debug.Log($"Initial position set to: {startPosition}");
        
        while (rectTransform.anchoredPosition.x > endXPosition) {
            Vector2 position = rectTransform.anchoredPosition;

            if (!hasPaused && position.x <= 10 && position.x >= -10) {
                Debug.Log($"Pausing at center position: {position}");
                yield return new WaitForSeconds(0.5f);
                hasPaused = true;
            }
            
            position.x -= moveSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = position;
            
            if (Time.frameCount % 30 == 0) // Log every 30 frames to avoid spam
                Debug.Log($"Current position: {position}");
                
            yield return null;
        }

        Vector2 endPosition = rectTransform.anchoredPosition;
        endPosition.x = startXPosition;
        rectTransform.anchoredPosition = endPosition;
        Debug.Log($"Animation completed. Final position: {endPosition}");
        moveObject.SetActive(false);
    }

    public IEnumerator AnimateStop(GameObject moveObject, float StartX, float EndX, Vector2 direction) {
        Debug.Log($"Starting Stop animation for {moveObject.name} from {StartX} to {EndX}, direction: {direction}");
        moveObject.SetActive(true);
        RectTransform rectTransform = moveObject.GetComponent<RectTransform>();

        Vector2 startPosition = rectTransform.anchoredPosition;
        startPosition.x = StartX;
        rectTransform.anchoredPosition = startPosition;
        Debug.Log($"Initial position set to: {startPosition}");

        if (direction == Vector2.right) {
            while (rectTransform.anchoredPosition.x < EndX) {
                Vector2 position = rectTransform.anchoredPosition;
                position.x += moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = position;
                
                if (Time.frameCount % 30 == 0)
                    Debug.Log($"Current position: {position}");
                    
                yield return null;
            }
        } else if (direction == Vector2.left) {
            while (rectTransform.anchoredPosition.x > EndX) {
                Vector2 position = rectTransform.anchoredPosition;
                position.x -= moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = position;
                
                if (Time.frameCount % 30 == 0)
                    Debug.Log($"Current position: {position}");
                    
                yield return null;
            }
        }
        Debug.Log($"Stop animation completed. Final position: {rectTransform.anchoredPosition}");
    }

    public IEnumerator AnimeReset(GameObject moveObject, float StartX, float EndX){
        RectTransform rectTransform = moveObject.GetComponent<RectTransform>();
        Vector2 endPosition = rectTransform.anchoredPosition;
        endPosition.x = StartX;
        rectTransform.anchoredPosition = endPosition;
        moveObject.SetActive(false);
        yield return null;
    }

    public IEnumerator WaitTime(float time){
        yield return new WaitForSeconds(time);
    }
}
