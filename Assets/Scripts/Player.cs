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

        private Ray screenRay;
        private RaycastHit hit;
        private LayerMask planeLayerMask;
        private LayerMask candyLayerMask;
        private Transform hitCandyTransform;
        private GameObject candyGameObject;
        private Candy candy;
        private Rigidbody candyRig;
        private Vector3 candyPos;

        [SerializeField] private GameObject desk;
        [SerializeField] private ScrollRect scrollRect;

        public event PlayerEventHandler putCandyEvent;

        void Start()
        {
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
                candyGameObject = Instantiate(hitCandyTransform.gameObject, desk.transform, true);
                candy = candyGameObject.GetComponent<Candy>();
                candyRig = candyGameObject.GetComponent<Rigidbody>();
                candyRig.useGravity = true;
                candyRig.isKinematic = false;
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
                    candyGameObject.layer = 0;
                    candyPos = candyGameObject.transform.position;
                    if (candyPos.z > 1 || candyPos.z < -1)
                        candyPos.z = candy.posZ;
                    candyGameObject.transform.position = candyPos;

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