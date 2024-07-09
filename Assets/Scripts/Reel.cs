using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reel : MonoBehaviour
{
    [SerializeField] private RectTransform reelTransform;
    [SerializeField] private Sprite[] symbols;
    [SerializeField] private float scrollSpeed = 500f;

    private Queue<Sprite> reelQueue;

    public void Initialize()
    {
        reelQueue = new Queue<Sprite>(symbols);
        UpdateReelSymbols();
    }

    public IEnumerator Spin(float duration)
    {
        float elapsed = 0f;
        float symbolHeight = reelTransform.rect.height / symbols.Length;
        Vector2 originalPosition = reelTransform.anchoredPosition;
        Vector2 startPosition = originalPosition + new Vector2(0, symbolHeight * symbols.Length);

        reelTransform.anchoredPosition = startPosition;

        while (elapsed < duration)
        {
            reelTransform.anchoredPosition -= new Vector2(0, scrollSpeed * Time.deltaTime);

            if (reelTransform.anchoredPosition.y <= originalPosition.y - symbolHeight)
            {
                reelTransform.anchoredPosition = startPosition;
                RotateQueue();
                UpdateReelSymbols();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        reelTransform.anchoredPosition = originalPosition;
        UpdateReelSymbols();
    }

    public Sprite GetSymbol()
    {
        return reelQueue.Peek();
    }

    private void RotateQueue()
    {
        Sprite symbol = reelQueue.Dequeue();
        reelQueue.Enqueue(symbol);
    }

    private void UpdateReelSymbols()
    {
        Image[] reelImages = reelTransform.GetComponentsInChildren<Image>();
        int i = 0;
        foreach (Sprite symbol in reelQueue)
        {
            reelImages[i].sprite = symbol;
            i++;
            if (i >= reelImages.Length) break;
        }
    }
}
