using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class Order
    {
        public bool isAccept = false;

        public int amount;

        public CandyType essentialCandyType;

        public Order(int _amount, CandyType _essentialCandyType)
        {
            amount = _amount;
            essentialCandyType = _essentialCandyType;
        }
    }
}