using UnityEngine;
using System.Collections.Generic;

public enum CatType
{
    Orange,
    Brown,
    // Tambah di sini nanti
    // Black,
    // White,
}

[System.Serializable]
public struct CatVisualData
{
    public CatType type;
    public Sprite normalSprite;
    public Sprite madSprite;
}




[CreateAssetMenu(fileName = "CatDatabase", menuName = "Game/Cat Database")]
public class CatDatabase : ScriptableObject
{
    public List<CatVisualData> cats;
}
