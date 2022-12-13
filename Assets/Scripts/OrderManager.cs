using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Katniss
{
    public class OrderManager : MonoBehaviour
    {
        private bool isComplete;
        private bool isContaining;

        private float amount = 0;
        private float boxCameraFov;

        private Vector3 boxCameraPos;
        private Quaternion boxCameraRot;
        private Order currOrder;

        [SerializeField] private Camera boxCamera;
        [SerializeField] private Animator boxAnimator;
        [SerializeField] private Shop shop;
        [SerializeField] private Player player;

        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas lowerCanvas;
        [SerializeField] private Canvas finishCanvas;
        [SerializeField] private Image amountProgressFill;

        void Start()
        {
            boxCamera.enabled = false;

            canvas.enabled = false;
            lowerCanvas.enabled = false;
            finishCanvas.enabled = false;
            amountProgressFill.fillAmount = 0f;

            shop.newOrderEvent += new ShopEventHandler(setOrder);
            player.putCandyEvent += new PlayerEventHandler(getCandy);
        }

        void setOrder(Order order)
        {
            currOrder = order;
            player.isInProcessing = true;

            canvas.enabled = true;
            lowerCanvas.enabled = true;
        }

        void getCandy(Candy candy)
        {
            amount += candy.type.price;
            StartCoroutine(fillAmountProgress());

            if (!isContaining && candy.type == currOrder.essentialCandyType)
            {
                isContaining = true;
            }

            if (isComplete && isContaining && amount >= currOrder.amount)
            {
                isComplete = true;
            }
        }

        public void finishOrder()
        {
            if (isComplete)
                shop.goodReaction = true;

            player.isInProcessing = false;

            boxAnimator.SetTrigger("Close");

            StartCoroutine(moveCamera());
            StartCoroutine(hightlightBox());
        }

        IEnumerator fillAmountProgress()
        {
            var fillAmount = amount / currOrder.amount;
            var delta = fillAmount - amountProgressFill.fillAmount;

            for (; delta > 0; delta -= Time.deltaTime)
            {
                amountProgressFill.fillAmount = fillAmount - delta;
                yield return null;
            }
        }

        IEnumerator moveCamera()
        {
            var movingTime = 2f;

            var camPos = boxCamera.transform.localPosition;
            var camRot = boxCamera.transform.localRotation;
            var camFov = boxCamera.fieldOfView;

            for (var time = 0f; time < movingTime; time += Time.deltaTime)
            {
                boxCamera.transform.localPosition = Vector3.Lerp(camPos, boxCameraPos, time / movingTime);
                boxCamera.transform.localRotation = Quaternion.Lerp(camRot, boxCameraRot, time / movingTime);
                boxCamera.fieldOfView = Mathf.Lerp(camFov, boxCameraFov, time / movingTime);
                yield return null;
            }
        }

        IEnumerator hightlightBox()
        {
            yield return new WaitUntil(() => (boxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Closing")));
            yield return new WaitWhile(() => (boxAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f));

            canvas.enabled = false;
            lowerCanvas.enabled = false;
            finishCanvas.enabled = true;
            boxCamera.enabled = true;

            //finishCanvas.enabled = false;
            //boxCamera.enabled = false;
            //shop.finishOrder();
        }
    }
}