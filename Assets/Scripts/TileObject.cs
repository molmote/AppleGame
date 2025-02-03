using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField] int col;
    [SerializeField] int row;
    [SerializeField] int number;
    [SerializeField] TextMeshPro textNumber;
    [SerializeField] SpriteRenderer spRenderer;
    [SerializeField] bool isSelected;
    // [SerializeField] BoxCollider2D collider;

    public void Setup(int _col,  int _row, int _num)
    {
        col = _col;
        row = _row;
        number = _num;
        textNumber.text = $"{number}";
    }

    public int GetNumber()
    {
        return number;
    }

    public int GetColumn()
    {
        return col;
    }

    public int GetRow()
    {
        return row;
    }

    public void ToggleSelected(bool toggle)
    {
        Debug.Log($"change select of tile ({col},{row}) {toggle}");
        if (toggle)
        {
            spRenderer.color = new Color( 1f,0,0,1f  );
        }
        else
        {
            spRenderer.color = new Color(0, 1f,64/255f,1f);
        }

        isSelected = toggle;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetActive(bool active)
    {
        ToggleSelected(false);
        GetComponent<Collider2D>().enabled = (active);
        spRenderer.enabled = active;
        textNumber.gameObject.SetActive(active);
    }
}