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

        private Color color;
        private Ray screenRay;
        private RaycastHit hit;
        private LayerMask planeLayerMask;
        private LayerMask candyLayerMask;
        private Transform hitCandyTransform;
        private GameObject candyGameObject;
        private Candy candy;
        private Rigidbody candyRig;
        private Vector3 candyPos;
        private Vector3 effectPos;
        private Image wordEffect;

        [SerializeField] private GameObject box;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private Image[] wordEffects;

        public event PlayerEventHandler putCandyEvent;

        void Start()
        {
            inBoxLayer = LayerMask.NameToLayer("InBox");

            planeLayerMask = 1 << LayerMask.NameToLayer("Plane");
            candyLayerMask = 1 << LayerMask.NameToLayer("Candy");

            foreach (Image _wordEffect in wordEffects)
            {
                _wordEffect.enabled = false;
            }
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

                    wordEffect = wordEffects[Random.Range(0, maxExclusive: wordEffects.Length)];
                    effectPos = Input.mousePosition;
                    effectPos.y += 50f;
                    wordEffect.transform.position = effectPos;
                    color = wordEffect.color;
                    color.a = 0;
                    wordEffect.color = color;
                    wordEffect.enabled = true;
                    StartCoroutine(fadeInOut());

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

        IEnumerator fadeInOut()
        {
            var size = 0.5f;
            var effectTime = 0.3f;

            for (var time = 0f; time <= effectTime * 2; time += Time.deltaTime)
            {
                if (time <= effectTime)
                {
                    color.a = (time / effectTime);
                    wordEffect.color = color;
                    wordEffect.transform.localScale = Vector3.one * (1 + size * time);
                }
                else
                {
                    color.a = (2 - time / effectTime);
                    wordEffect.color = color;
                    wordEffect.transform.localScale = Vector3.one * (2 * size * effectTime + 1 - time * size);
                }

                yield return null;
            }

            wordEffect.enabled = false;
        }
    }
}