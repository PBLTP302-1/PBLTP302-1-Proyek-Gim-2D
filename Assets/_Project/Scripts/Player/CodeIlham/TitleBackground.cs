using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    [SerializeField] private float amplitude = 240f;
    [SerializeField] private float period = 6f;
    
    private Vector3 startPos; // 🟢 UBAH dari Vector2 ke Vector3

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float x = startPos.x + Mathf.Sin(Time.time / period) * amplitude;
        transform.localPosition = new Vector3(x, startPos.y, startPos.z);
    }
}