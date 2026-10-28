using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "Warga";
    [SerializeField] private SpriteRenderer body;
    
    [SerializeField] private Sprite faceDown, faceLeft, faceRight, faceUp;

    public void Interact(PlayerInteractor who)
    {
        Vector2 direction = who.transform.position - transform.position;
        Face(direction);
        Debug.Log($"{npcName}: Halo!");
    }

    private void Face(Vector2 d)
    {
        if (Mathf.Abs(d.x) > Mathf.Abs(d.y))
            body.sprite = d.x > 0 ? faceRight : faceLeft;
        else
            body.sprite = d.y > 0 ? faceUp : faceDown;
    }

}
