using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private Lever lever;
    [SerializeField] private Reel[] reels;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private BetManager betManager;
    [SerializeField] private ParticleSystem jackpotParticle;

    private bool isSpinning = false;

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        betManager.Initialize();
        lever.Initialize(this);
        foreach (var reel in reels)
        {
            reel.Initialize();
        }
    }

    public void OnBetAmountSelected(int betAmount)
    {
        if (isSpinning || !betManager.CanBet(betAmount))
            return;

        betManager.PlaceBet(betAmount);
        StartCoroutine(SpinReels());
    }

    private IEnumerator SpinReels()
    {
        isSpinning = true;
        resultText.text = "Spinning...";

        List<Coroutine> spinCoroutines = new List<Coroutine>();
        for (int i = 0; i < reels.Length; i++)
        {
            spinCoroutines.Add(StartCoroutine(reels[i].Spin(Random.Range(1f, 2f))));
        }

        foreach (var coroutine in spinCoroutines)
        {
            yield return coroutine;
        }

        CheckResults();
        isSpinning = false;
    }

    private void CheckResults()
    {
        bool allSame = true;
        Sprite firstSymbol = reels[0].GetSymbol();

        foreach (var reel in reels)
        {
            if (reel.GetSymbol() != firstSymbol)
            {
                allSame = false;
                break;
            }
        }

        if (allSame)
        {
            jackpotParticle.Play();
            resultText.text = "You Win!";
            betManager.Payout();
        }
        else
        {
            resultText.text = "Try Again!";
        }
    }
}
