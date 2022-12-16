using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    [CreateAssetMenu(fileName = "NewOrder", menuName = "Order")]
    public class Order: ScriptableObject
    {
        public bool isAccept = false;
        public bool isSecret;

        public int amount;

        public CandyType essentialCandyType;
        public Sprite baseSprite;
        public Sprite acceptSprite;

        public Sprite[] dmSprites;
        public Sprite[] goodSprites;
        public Sprite[] badSprites;
    }
}