using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToastView : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private float duration = 2.5f;

    public void Show(string message)
    {
        gameObject.SetActive(true);
        
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        if (messageText != null) messageText.text = message;

        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }
}