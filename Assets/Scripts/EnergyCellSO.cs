using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnergyCellSO : ScriptableObject
{
   public enum CellState
    {
        Idle,
        Picked,
        Delivered
    }

   public enum CellType
    {
        Base,
        Red,
        Green,
        Blue,
        Yellow
    }

    public Vector3 color;
    public CellType label;
    public CellState state;
}
