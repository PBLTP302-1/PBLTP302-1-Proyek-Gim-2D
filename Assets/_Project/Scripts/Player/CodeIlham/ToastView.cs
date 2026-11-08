using System.Collections;
using UnityEngine;
using TMPro;

public class ToastView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private float displayTime = 2f;

    private Coroutine toastCoroutine;

    public void Show(string message)
    {
        if (toastCoroutine != null) StopCoroutine(toastCoroutine);
        toastCoroutine = StartCoroutine(ShowToast(message));
    }

    private IEnumerator ShowToast(string message)
    {
        if (toastText != null)
        {
            toastText.text = message;
            toastText.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(displayTime);
            
            toastText.gameObject.SetActive(false);
        }
    }
}