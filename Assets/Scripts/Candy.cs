using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class Candy : MonoBehaviour
    {
        public bool isInBox = false;

        private int boxLayer;

        public CandyType type;

        void Start()
        {
            boxLayer = LayerMask.NameToLayer("Box");
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == boxLayer)
            {
                isInBox = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == boxLayer)
            {
                isInBox = false;
            }
        }
    }
}