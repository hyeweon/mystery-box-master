using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Katniss
{
    public delegate void PlayerEventHandler(Candy _candy);

    public class Player : MonoBehaviour
    {
        public bool isInProcessing = false;
        private bool isHoldingCandy = false;

        private int inBoxLayer;

        private Ray screenRay;
        private RaycastHit hit;
        private LayerMask planeLayerMask;
        private LayerMask candyLayerMask;
        private Transform hitCandyTransform;
        private GameObject candyGameObject;
        private Candy candy;
        private Rigidbody candyRig;
        private Vector3 candyPos;

        [SerializeField] private GameObject box;
        [SerializeField] private ScrollRect scrollRect;

        public event PlayerEventHandler putCandyEvent;

        void Start()
        {
            inBoxLayer = LayerMask.NameToLayer("InBox");

            planeLayerMask = 1 << LayerMask.NameToLayer("Plane");
            candyLayerMask = 1 << LayerMask.NameToLayer("Candy");
        }

        void Update()
        {
            if (isInProcessing)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pickCandy();
                }
                if (Input.GetMouseButton(0))
                {
                    moveCandy();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    putCandy();
                }
            }
        }

        void pickCandy()
        {
            screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(screenRay, out hit, Mathf.Infinity, candyLayerMask))
            {
                isHoldingCandy = true;

                scrollRect.enabled = false;

                hitCandyTransform = hit.transform;
                candyGameObject = Instantiate(hitCandyTransform.gameObject, box.transform, true);
                candy = candyGameObject.GetComponent<Candy>();
                candyRig = candyGameObject.GetComponent<Rigidbody>();

                var candyGameObjectScale = candyGameObject.transform.localScale;
                candyGameObjectScale.x *= 1.5f;
                candyGameObjectScale.y *= 1.5f;
                candyGameObjectScale.z *= 1.5f;
                candyGameObject.transform.localScale = candyGameObjectScale;

                candyGameObject.layer = inBoxLayer;
            }
            else
            {
                isHoldingCandy = false;
            }
        }

        void moveCandy()
        {
            if (isHoldingCandy)
            {
                screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(screenRay, out hit, Mathf.Infinity, planeLayerMask))
                {
                    candyGameObject.transform.position = hit.point;
                }
            }
        }

        void putCandy()
        {
            if (isHoldingCandy)
            {
                if (candy.isInBox)
                {
                    candyPos = candyGameObject.transform.position;
                    candyGameObject.transform.position = candyPos;

                    candyRig.useGravity = true;
                    candyRig.isKinematic = false;

                    putCandyEvent(candy);
                }
                else
                {
                    Destroy(candyGameObject);
                }
            }

            isHoldingCandy = false;
            scrollRect.enabled = true;
        }
    }
}