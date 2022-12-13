using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    [CreateAssetMenu(fileName = "New Candy Type", menuName = "Candy Type")]
    public class CandyType : ScriptableObject
    {
        public int type;
        public int price;
    }
}