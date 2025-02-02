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

    public void Setup(int _col,  int _row, int _num)
    {
        col = _col;
        row = _row;
        number = _num;
        textNumber.text = $"{number}";
    }
}