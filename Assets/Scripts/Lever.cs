using System.Collections;
using UnityEngine;
using TMPro;

public class Lever : MonoBehaviour
{
    [SerializeField] private GameObject pulledLever;
    [SerializeField] private GameObject unpulledLever;
    [SerializeField] private AudioSource leverPullSound;
    [SerializeField] private TMP_Dropdown betAmountSelector;
    [SerializeField] private SlotMachine slotMachine;

    public void Initialize(SlotMachine slotMachineInstance)
    {
        slotMachine = slotMachineInstance;
        betAmountSelector.gameObject.SetActive(false);
    }

    public void Pull()
    {
        betAmountSelector.gameObject.SetActive(true);
    }

    private IEnumerator LeverPullAnimation()
    {
        pulledLever.SetActive(true);
        unpulledLever.SetActive(false);
        yield return new WaitForSeconds(.08f);
        pulledLever.SetActive(false);
        unpulledLever.SetActive(true);
        betAmountSelector.value = 0;
    }

    public void OnBetAmountSelected()
    {
        leverPullSound.Play();
        StartCoroutine(LeverPullAnimation());

        int betAmount = 0;
        switch (betAmountSelector.value)
        {
            case 1: betAmount = 10; break;
            case 2: betAmount = 50; break;
            case 3: betAmount = 100; break;
            case 4: betAmount = 500; break;
        }

        betAmountSelector.gameObject.SetActive(false);
        slotMachine.OnBetAmountSelected(betAmount);
    }
}
