using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TextInput: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] rows;
    [SerializeField] private int columns = 1;

    [Header("Warna")]
    [SerializeField] private Color normal = new Color(0.93f, 0.91f, 0.97f, 1f);
    [SerializeField] private Color selected = new Color(1f, 0.78f, 0.1f, 1f);
    [SerializeField] private Color disabled = new Color(0.36f, 0.34f, 0.5f, 1f);

    public event Action<int> Picked;

    private string[] labels;
    private bool[] usable;
    private int count;
    private int selectedRow;
    private bool acceptInput;

    void Awake()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] == null) continue;

            int index = i;

            // TextMeshProUGUI sudah Graphic, cukup aktifkan raycast
            rows[i].raycastTarget = true;

            // Tambahkan Button component untuk klik mouse
            Button btn = rows[i].GetComponent<Button>();
            if (btn == null) btn = rows[i].gameObject.AddComponent<Button>();

            btn.transition = Selectable.Transition.None;
            btn.navigation = new Navigation { mode = Navigation.Mode.None };

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Click(index));

            AddHoverTrigger(rows[i].gameObject, index);
        }
    }

    private void AddHoverTrigger(GameObject obj, int index)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = obj.AddComponent<EventTrigger>();
        else trigger.triggers.Clear();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => Hover(index));
        trigger.triggers.Add(entry);
    }

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null) for (int i = 0; i < count; i++) usable[i] = true;

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] != null) rows[i].gameObject.SetActive(i < count);
        }

        selectedRow = Mathf.Clamp(start, 0, count - 1);
        if (!usable[selectedRow]) selectedRow = FirstUsable();

        gameObject.SetActive(true);
        acceptInput = false;
        Redraw();

        Debug.Log($"[TextMenu] Open: {count} options, selected={selectedRow}, labels=[{string.Join(",", labels)}]");
    }

    public void Click(int index)
    {
        if (index < 0 || index >= count || !usable[index]) return;
        selectedRow = index;
        Redraw();
        Pick(selectedRow);
    }

    public void Hover(int index)
    {
        if (index < 0 || index >= count || !usable[index]) return;
        selectedRow = index;
        Redraw();
    }

    void Update()
    {
        if (count == 0) return;
        if (!acceptInput) { acceptInput = true; return; }

        if (MenuInput.Down) Move(+1, 0);
        if (MenuInput.Up) Move(-1, 0);
        if (MenuInput.Left) Move(0, -1);
        if (MenuInput.Right) Move(0, +1);
        
        if (MenuInput.Confirm)
        {
            Debug.Log($"[TextMenu] Confirm ditekan! selectedRow={selectedRow}, label={labels[selectedRow]}");
            Pick(selectedRow);
        }
    }

    private void Move(int dRow, int dCol)
    {
        int next = selectedRow + dRow + dCol * columns;
        if (next < 0 || next >= count) return;
        if (!usable[next]) next = FirstUsable();
        selectedRow = next;
        Redraw();
    }

    private int FirstUsable()
    {
        for (int i = 0; i < count; i++) if (usable[i]) return i;
        return 0;
    }

    private void Pick(int index)
    {
        if (!usable[index]) return;
        Debug.Log($"[TextMenu] Pick({index}) -> label={labels[index]}");
        Picked?.Invoke(index);
    }

    private void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            if (rows[i] == null) continue;
            bool on = i == selectedRow;
            rows[i].text = (on ? "▶ " : "  ") + labels[i];
            rows[i].color = !usable[i] ? disabled : on ? selected : normal;
        }
    }
}