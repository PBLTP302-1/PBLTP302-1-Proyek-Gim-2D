using UnityEngine;

public class VfxAnimator : MonoBehaviour
{
    private Animator anim;
    private bool isPlaying;

    void Awake()
    {
        anim = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void Play()
    {
        gameObject.SetActive(true);
        isPlaying = true;
        anim.Play("HitSlash", 0, 0f); 
    }

    void Update()
    {
        if (!isPlaying) return;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        
        if (info.normalizedTime >= 1f && !anim.IsInTransition(0))
        {
            gameObject.SetActive(false);
            isPlaying = false;
        }
    }
}