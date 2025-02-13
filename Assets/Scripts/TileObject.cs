using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    private int col;
    private int row;
    private int number;
    bool isSelected;
    [SerializeField] TextMeshPro textNumber;
    [SerializeField] SpriteRenderer spRenderer;
    [SerializeField] Collider2D myCollider;
    [SerializeField] Color selectedColor;

    [SerializeField] TextMeshPro testTextNumber;

    public bool IsVisited
        { get; set; }

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

    public void ToggleSelectedOnly(bool toggle)
    {
        isSelected = toggle;
    }

    public void ToggleSelected(bool toggle)
    {
        // MyLogger.Log($"change select of tile ({col},{row}) {toggle}");
        if (toggle)
        {
            spRenderer.color = Color.red;
        }
        else
        {
            spRenderer.color = selectedColor;
        }

        isSelected = toggle;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool IsActive()
    {
        return myCollider.enabled;
    }

    public void SetActive(bool active, bool showRed = false)
    {
        ToggleSelected(false);
        myCollider.enabled = active;
        if (showRed)
        {
            spRenderer.color = Color.red;
        }
        else
        {
            spRenderer.enabled = active;
            textNumber.gameObject.SetActive(active);
        }
    }

    public void SetTestText(string text)
    {
        // if (testTextNumber.gameObject.activeInHierarchy)
        {
            testTextNumber.gameObject.SetActive(true);
            testTextNumber.text = text;
        }
    }
}