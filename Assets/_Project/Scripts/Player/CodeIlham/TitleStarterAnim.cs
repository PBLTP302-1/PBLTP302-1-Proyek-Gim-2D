using UnityEngine;

public class TitleStarterAnim : MonoBehaviour
{
    [SerializeField] private float bounceHeight = 20f;
    [SerializeField] private float speed = 0.35f;
    
    private Vector3 startPos; // 🟢 UBAH dari Vector2 ke Vector3
    private float offset;

    void Start()
    {
        startPos = transform.localPosition;
        offset = Random.Range(0f, Mathf.PI * 2);
    }

    void Update()
    {
        float y = startPos.y + Mathf.Sin(Time.time / speed + offset) * bounceHeight;
        transform.localPosition = new Vector3(startPos.x, y, startPos.z);
    }
}