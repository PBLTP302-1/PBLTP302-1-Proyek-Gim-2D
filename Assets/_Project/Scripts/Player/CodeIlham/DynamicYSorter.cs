using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapYSorter : MonoBehaviour
{
    [SerializeField] private int sortingOrderBase = 0;
    [SerializeField] private float precision = 100f;
    
    private TilemapRenderer tilemapRenderer;
    private Tilemap tilemap;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemap = GetComponent<Tilemap>();
        
        if (tilemapRenderer == null)
        {
            Debug.LogWarning("TilemapYSorter: No TilemapRenderer found on " + gameObject.name);
        }
    }

    void Start()
    {
        // Set mode ke Individual agar bisa sort per-tile
        if (tilemapRenderer != null)
        {
            tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
            tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopRight; // Atau BottomLeft, tidak masalah karena kita override
        }
    }

    void LateUpdate()
    {
        if (tilemapRenderer != null && tilemap != null)
        {
            // Untuk tilemap, kita set sorting order berdasarkan posisi tilemap
            // Karena tilemap biasanya statis, kita hanya perlu set sekali atau saat posisi berubah
            int newSortingOrder = sortingOrderBase - Mathf.RoundToInt(transform.position.y * precision);
            tilemapRenderer.sortingOrder = newSortingOrder;
        }
    }
}