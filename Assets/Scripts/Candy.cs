using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katniss
{
    public class Candy : MonoBehaviour
    {
        public bool isInBox = false;

        private bool isPut = false;

        private int boxLayer;
        private int inBoxLayer;
        private int bufferLayer;

        public CandyType type;

        [SerializeField] AudioSource source;
        [SerializeField] AudioClip clip;

        void Start()
        {
            boxLayer = LayerMask.NameToLayer("Box");
            inBoxLayer = LayerMask.NameToLayer("InBox");
            bufferLayer = LayerMask.NameToLayer("Buffer");
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

        private void OnCollisionEnter(Collision collision)
        {
            if (!isPut)
            {
                if (collision.gameObject.layer == bufferLayer)
                {
                    isPut = true;
                    source.PlayOneShot(clip);
                }
                else if (collision.gameObject.layer == inBoxLayer)
                {
                    isPut = true;
                    source.Play();
                }
            }
        }
    }
}