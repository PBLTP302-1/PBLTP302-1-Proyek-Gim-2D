using System;
using UnityEngine;
using UnityEngine.UI;

public class TextMenu : MonoBehaviour
{
    [SerializeField] private Text[] rows;
    [SerializeField] private Color normal   = UiTheme.Text;
    [SerializeField] private Color selected = UiTheme.Gold;
    [SerializeField] private Color disabled = UiTheme.Disabled;

    public event Action<int> Picked;

    private string[] labels;
    private bool[] usable;
    private int count;
    private int selectedIndex;
    private bool acceptInput;

    public int Selected
    {
        get => selectedIndex;
        set
        {
            selectedIndex = Mathf.Clamp(value, 0, count - 1);
            if (count > 0 && !usable[selectedIndex])
                selectedIndex = FirstUsable();
            Redraw();
        }
    }

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null)
            for (int i = 0; i < count; i++) usable[i] = true;

        for (int i = 0; i < rows.Length; i++)
            rows[i].gameObject.SetActive(i < count);

        selectedIndex = Mathf.Clamp(start, 0, count - 1);
        if (!usable[selectedIndex]) selectedIndex = FirstUsable();

        gameObject.SetActive(true);
        acceptInput = false;
        Redraw();
    }

    public void Close() => gameObject.SetActive(false);

    void Update()
    {
        if (count == 0) return;
        if (!acceptInput) { acceptInput = true; return; }

        if (MenuInput.Up)   Move(-1);
        if (MenuInput.Down) Move(+1);
        if (MenuInput.Left) Move(-1);
        if (MenuInput.Right)Move(+1);
        if (MenuInput.Confirm) Pick(selectedIndex);
    }

    void Move(int delta)
    {
        int next = selectedIndex + delta;
        if (next < 0) next = count - 1;
        if (next >= count) next = 0;
        if (!usable[next]) return;
        selectedIndex = next;
        Redraw();
    }

    void Pick(int index)
    {
        if (index < 0 || index >= count || !usable[index]) return;
        Picked?.Invoke(index);
    }

    public void Click(int index)
    {
        if (!acceptInput) return;
        selectedIndex = index;
        Redraw();
        Pick(index);
    }

    public void Hover(int index)
    {
        if (!acceptInput) return;
        if (usable[index])
        {
            selectedIndex = index;
            Redraw();
        }
    }

    void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            bool on = i == selectedIndex;
            rows[i].text = (on ? "▶ " : "  ") + labels[i];
            rows[i].color = !usable[i] ? disabled : (on ? selected : normal);
        }
    }

    int FirstUsable()
    {
        for (int i = 0; i < count; i++) if (usable[i]) return i;
        return 0;
    }
}