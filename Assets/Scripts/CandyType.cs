using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    [CreateAssetMenu(fileName = "NewCandyType", menuName = "Candy Type")]
    public class CandyType : ScriptableObject
    {
        public int type;
        public int price;
    }
}