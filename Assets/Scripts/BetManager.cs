using UnityEngine;
using TMPro;

public class BetManager : MonoBehaviour
{
    [SerializeField] private TMP_Text accountBalanceText;
    [SerializeField] private int accountBalance;

    private int currentBetAmount;

    public void Initialize()
    {
        UpdateBalanceText();
    }

    public bool CanBet(int amount)
    {
        return accountBalance >= amount;
    }

    public void PlaceBet(int amount)
    {
        accountBalance -= amount;
        currentBetAmount = amount;
        UpdateBalanceText();
    }

    public void Payout()
    {
        accountBalance += currentBetAmount * 2;
        UpdateBalanceText();
    }

    private void UpdateBalanceText()
    {
        accountBalanceText.text = $"Balance: {accountBalance}";
    }
}
