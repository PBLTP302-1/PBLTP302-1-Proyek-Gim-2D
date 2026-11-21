using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    [SerializeField] private VfxAnimator prefab;
    [SerializeField] private int poolSize = 8;
    
    private Queue<VfxAnimator> pool = new Queue<VfxAnimator>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var vfx = Instantiate(prefab, transform);
            vfx.gameObject.SetActive(false);
            pool.Enqueue(vfx);
        }
    }

    public void Play(Vector3 pos)
    {
        if (pool.Count == 0) return;
        
        var vfx = pool.Dequeue();
        vfx.transform.position = pos;
        vfx.Play();
        
        StartCoroutine(ReturnAfterDelay(vfx, 0.3f));
    }

    System.Collections.IEnumerator ReturnAfterDelay(VfxAnimator vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Enqueue(vfx);
    }
}