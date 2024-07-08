using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Timeline.Actions;
using Unity.VisualScripting;

public class SlotMachine : MonoBehaviour
{
    // Reference to the gameobject of the pulled lever
    [SerializeField] GameObject m_Pulled_Lever;

    // Reference to the gameobject of the unpulled lever
    [SerializeField] GameObject m_UnPulled_Lever;

    // Array of RectTransform components representing the reels
    [SerializeField] RectTransform[] reels;

    // Array of Sprite objects representing the slot symbols
    [SerializeField] Sprite[] symbols;

    // Duration of the spin animation for each reel
    [SerializeField] float spinDuration = 2f;

    // Speed of the scrolling animation
    [SerializeField] float scrollSpeed = 500f;

    // Text component to display the result message
    [SerializeField] TMP_Text resultText;

    // Balance in account
    [SerializeField] int accountBalance;

    // Balance text reference
    [SerializeField] TMP_Text accountBalanceText;

    // Dropdown selector reference
    [SerializeField] TMP_Dropdown betAmountSelector;

    // Particle Effect reference while winning the JACKPOT
    [SerializeField] ParticleSystem jackpotParticle;

    // Boolean flag to check if the reels are currently spinning
    private bool isSpinning = false;

    // Queue of symbols for each reel
    private Queue<Sprite>[] reelQueues;

    // Bet amount
    private int betAmount;

    void Awake()
    {
        DefaultSetup();
    }

    void Start()
    {
        betAmountSelector.onValueChanged.AddListener(LeverTrigger);

        // Initialize the queues for each reel
        reelQueues = new Queue<Sprite>[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            reelQueues[i] = new Queue<Sprite>(symbols);
        }
    }

    void Update()
    {
        // Check for the Space key press and ensure the reels are not currently spinning
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinning)
        {
            betAmountSelector.gameObject.SetActive(true);
        }
    }

    // Click Button Click Detection For Starting The Wheel
    public void Pull()
    {
        // Check for the button press and ensure the reels are not currently spinning
        if (!isSpinning)
        {
            betAmountSelector.gameObject.SetActive(true);
        }
    }

    // Default Setup While Starting The Scene
    void DefaultSetup()
    {
        betAmountSelector.gameObject.SetActive(false);

        m_Pulled_Lever.SetActive(false);
        m_UnPulled_Lever.SetActive(true);

        accountBalanceText.text = accountBalance.ToString();
    }

    // Coroutine to handle the spinning of all reels
    // Coroutine to handle the spinning of all reels
    IEnumerator SpinReels()
    {
        // Set the spinning flag to true
        isSpinning = true;

        // Clear the result text
        resultText.text = "Spinning...";

        // Store the coroutines for each reel
        List<Coroutine> spinCoroutines = new List<Coroutine>();

        // Spin each reel one by one
        for (int i = 0; i < reels.Length; i++)
        {
            Coroutine spinCoroutine = StartCoroutine(SpinReel(reels[i], reelQueues[i], Random.Range(spinDuration, spinDuration * i)));
            spinCoroutines.Add(spinCoroutine);
        }

        // Wait for all reels to stop spinning
        foreach (var coroutine in spinCoroutines)
        {
            yield return coroutine;
        }

        // Check the results after all reels have stopped
        CheckResults();

        // Set the spinning flag to false
        isSpinning = false;
    }


    // Coroutine to handle the spinning of a single reel
    IEnumerator SpinReel(RectTransform reel, Queue<Sprite> reelQueue, float duration)
    {
        // Elapsed time for the spin animation
        float elapsed = 0f;

        // Height of a single symbol
        float symbolHeight = reel.rect.height / symbols.Length;

        // Original position of the reel
        Vector2 originalPosition = reel.anchoredPosition;

        // Extended distance for the scrolling effect
        float extendedDistance = symbolHeight * symbols.Length;

        // Start position for the scrolling effect
        Vector2 startPosition = originalPosition + new Vector2(0, extendedDistance + 2);

        // Set the reel's position to the start position
        reel.anchoredPosition = startPosition;

        // Continue spinning until the duration is reached
        while (elapsed < duration)
        {
            // Scroll the reel down
            reel.anchoredPosition -= new Vector2(0, scrollSpeed * Time.deltaTime);

            // If the reel has scrolled past the original position, reset to the start position
            if (reel.anchoredPosition.y <= originalPosition.y * 3)
            {
                // Set the reel's position back to the start position
                reel.anchoredPosition = startPosition;

                // Rotate the queue for the reel
                RotateQueue(reelQueue);
                UpdateReelSymbols(reel, reelQueue);
            }

            // Increment the elapsed time by the delta time
            elapsed += Time.deltaTime;

            // Yield execution until the next frame
            yield return null;
        }

        // Set the reel's position back to the original position
        reel.anchoredPosition = originalPosition;

        // Ensure the reel symbols are correctly aligned at the end
        UpdateReelSymbols(reel, reelQueue);
    }

    // Function to rotate the queue
    void RotateQueue(Queue<Sprite> queue)
    {
        // Dequeue the first element and enqueue it to the end
        Sprite symbol = queue.Dequeue();
        queue.Enqueue(symbol);
    }

    // Function to update the symbols on the reel
    void UpdateReelSymbols(RectTransform reel, Queue<Sprite> reelQueue)
    {
        // Get all Image components in the reel
        Image[] reelImages = reel.GetComponentsInChildren<Image>();

        // Set the sprites for each image from the queue
        int i = 0;
        foreach (Sprite symbol in reelQueue)
        {
            reelImages[i].sprite = symbol;
            i++;
            if (i >= reelImages.Length) break;
        }
    }

    void LeverTrigger(int value)
    {
        // Lever Pull Action
        StartCoroutine(LeverPull());

        betAmountSelector.gameObject.SetActive(false);

        switch (value)
        {
            case 1:
                UpdateAccount(false, 10);
                betAmount = 10;
                break;
            case 2:
                UpdateAccount(false, 50);
                betAmount = 50;
                break;
            case 3:
                UpdateAccount(false, 100);
                betAmount = 100;
                break;
            case 4:
                UpdateAccount(false, 500);
                betAmount = 500;
                break;
            default:
                break;
        }

        // Start the coroutine to spin the reels
        if (!isSpinning)
        {

            StartCoroutine(SpinReels());
        }
    }

    IEnumerator LeverPull()
    {
        m_Pulled_Lever.SetActive(true);
        m_UnPulled_Lever.SetActive(false);
        yield return new WaitForSeconds(.08f);
        m_Pulled_Lever.SetActive(false);
        m_UnPulled_Lever.SetActive(true);

        betAmountSelector.value = 0;
    }

    // This Method Updates Account Balance
    void UpdateAccount(bool inc, int amt)
    {
        if (inc)
        {
            accountBalance += amt;
            accountBalanceText.text = accountBalance.ToString();
        }
        else
        {
            accountBalance -= amt;
            accountBalanceText.text = accountBalance.ToString();
        }
    }

    // Function to check the results after spinning
    void CheckResults()
    {
        // Get the sprite of the first reel
        Sprite firstSymbol = reels[0].GetComponentsInChildren<Image>()[0].sprite;

        // Flag to check if all reels have the same symbol
        bool allSame = true;

        // Loop through each reel to compare symbols
        foreach (var reel in reels)
        {
            // If any reel's symbol is different, set the flag to false
            if (reel.GetComponentsInChildren<Image>()[0].sprite != firstSymbol)
            {
                allSame = false;
                break;
            }
        }

        // Display the result based on the flag
        if (allSame)
        {
            jackpotParticle.Play();
            resultText.text = "You Win!";
            // Add payout logic here
            UpdateAccount(true, betAmount * 2);
            betAmount = 0;
        }
        else
        {
            resultText.text = "Try Again!";
            betAmount = 0;
        }
    }
}
