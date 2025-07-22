using System;
using System.Collections.Generic;
using UnityEngine;
public enum TileType
{
    None = 0,
    Cheese = 1,
    Bacon = 2,
    BakingPowder = 3,
    Banana = 4,
    BandageBox = 5,
    BellPepper = 6,
    EggBrown = 7,
    Cabbage = 8,
    PotatoChipBlue = 9,
    Butter = 10,
    Obstacles = 5000,
}
namespace Scriptable
{
    [Serializable]
    public class TileSpriteData
    {
        public TileType tileType;
        public Sprite sprite;
    }
    [CreateAssetMenu(menuName = "ScriptableObject/TileData")]
    public class TilesData : ScriptableObject
    {
        [SerializeField] private List<TileSpriteData> tileSpriteMappings;

        private Dictionary<TileType, Sprite> spriteDict;
        private void OnEnable()
        {
            spriteDict = new Dictionary<TileType, Sprite>();
            foreach (var data in tileSpriteMappings)
            {
                if (!spriteDict.ContainsKey(data.tileType))
                {
                    spriteDict.Add(data.tileType, data.sprite);
                }
            }
        }
        public Sprite GetSprite(TileType type)
        {
            if (spriteDict == null || !spriteDict.TryGetValue(type, out var sprite))
                return null;
            return sprite;
        }
    }
    
}