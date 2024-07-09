using UnityEngine;
using UnityEngine.UI;

public class LeverPullButton : MonoBehaviour
{
    [SerializeField] private Lever lever;

    public void OnPullButtonClicked()
    {
        lever.Pull();
    }
}
