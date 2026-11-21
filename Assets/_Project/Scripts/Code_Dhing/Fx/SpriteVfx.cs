using UnityEngine;

public class SpriteVfx : MonoBehaviour
{
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] private float lifetime = 0.3f;
    
    private float timer;
    private Vector3 endScale;
    private System.Action onFinished;

    public void Init(Sprite sprite, Vector3 pos, Vector3 start, Vector3 end, float life, System.Action callback)
    {
        if (sr != null)
        {
            sr.sprite = sprite;
            sr.sortingLayerName = "Characters";
            sr.sortingOrder = 50;
        }
        transform.position = pos;
        transform.localScale = start;
        endScale = end;
        lifetime = life;
        timer = 0f;
        onFinished = callback;
        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / lifetime);
        transform.localScale = Vector3.Lerp(transform.localScale, endScale, t);
        
        if (sr != null) 
            sr.color = new Color(1, 1, 1, 1 - t);
        
        if (t >= 1f) 
        {
            gameObject.SetActive(false);
            onFinished?.Invoke();
        }
    }
}