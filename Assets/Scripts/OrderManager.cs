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

        private float currAmount = 0;
        //private float boxCameraFov;

        private Vector3 readyCameraPos;
        private Quaternion readyCameraRot;
        private Vector3 finishCameraPos;
        private Quaternion finishCameraRot;
        private Order currOrder;

        [SerializeField] private Camera boxCamera;
        [SerializeField] private Animator boxAnimator;
        [SerializeField] private Shop shop;
        [SerializeField] private Player player;
        [SerializeField] private ParticleSystem circleLightParticle;

        [SerializeField] private GameObject lowerCanvas;
        [SerializeField] private GameObject coinIcon;
        [SerializeField] private GameObject tapToContinue;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Canvas finishCanvas;
        [SerializeField] private Image amountProgressFill;
        [SerializeField] private Image checkMark;

        void Start()
        {
            readyCameraPos = new Vector3(0, 50, -9);
            readyCameraRot = Quaternion.Euler(80f, 0f, 0f);
            finishCameraPos = new Vector3(0, 50, -50);
            finishCameraRot = Quaternion.Euler(45f, 0f, 0f);

            boxCamera.enabled = false;

            lowerCanvas.SetActive(false);
            tapToContinue.SetActive(false);
            canvas.enabled = false;
            finishCanvas.enabled = false;
            checkMark.enabled = false;
            amountProgressFill.fillAmount = 0f;

            shop.newOrderEvent += new ShopEventHandler(setOrder);
            player.putCandyEvent += new PlayerEventHandler(getCandy);
        }

        void setOrder(Order order)
        {
            currOrder = order;
            player.isInProcessing = true;

            StartCoroutine(getReady2Process());
        }

        void getCandy(Candy candy)
        {
            currAmount += candy.type.price;
            if (amountProgressFill.fillAmount < 1f)
                StartCoroutine(fillAmountProgress());

            if (!isContaining && candy.type == currOrder.essentialCandyType)
            {
                isContaining = true;
                Debug.Log(isContaining);
                checkMark.enabled = true;
            }

            if (!isComplete && currAmount >= currOrder.amount)
            {
                if (isContaining)
                {
                    isComplete = true;
                    Debug.Log(isComplete);
                }
            }
        }

        public void finishOrder()
        {
            if (isComplete)
                shop.goodReaction = true;

            player.isInProcessing = false;

            circleLightParticle.Play();
            boxAnimator.SetTrigger("Close");
            lowerCanvas.SetActive(false);
            canvas.enabled = false;

            StartCoroutine(moveMainCamera());
            StartCoroutine(hightlightBox());
        }

        IEnumerator fillAmountProgress()
        {
            var fillAmount = currAmount / currOrder.amount;
            var delta = fillAmount - amountProgressFill.fillAmount;

            var coinPosX = fillAmount * 400 - 200;
            var coinPos = coinIcon.transform.localPosition;

            for (; delta > 0; delta -= Time.deltaTime)
            {
                amountProgressFill.fillAmount = fillAmount - delta;

                coinPos.x = coinPosX - delta * 400;
                if (coinPos.x < 200)
                {
                    coinIcon.transform.localPosition = coinPos;
                }
                yield return null;
            }

            if(isComplete)
                StartCoroutine(coinIconEffect());
        }

        IEnumerator getReady2Process()
        {
            var mainCamera = Camera.main;
            var movingTime = 1f;

            var camPos = mainCamera.transform.localPosition;
            var camRot = mainCamera.transform.localRotation;

            for (var time = 0f; time < movingTime; time += Time.deltaTime)
            {
                mainCamera.transform.localPosition = Vector3.Lerp(camPos, readyCameraPos, time / movingTime);
                mainCamera.transform.localRotation = Quaternion.Lerp(camRot, readyCameraRot, time / movingTime);
                yield return null;
            }

            lowerCanvas.SetActive(true);
            canvas.enabled = true;
        }

        IEnumerator moveMainCamera()
        {
            var mainCamera = Camera.main;
            var movingTime = 1f;

            var camPos = mainCamera.transform.localPosition;
            var camRot = mainCamera.transform.localRotation;
            //var camFov = boxCamera.fieldOfView;

            for (var time = 0f; time < movingTime; time += Time.deltaTime)
            {
                mainCamera.transform.localPosition = Vector3.Lerp(camPos, finishCameraPos, time / movingTime);
                mainCamera.transform.localRotation = Quaternion.Lerp(camRot, finishCameraRot, time / movingTime);
                //boxCamera.fieldOfView = Mathf.Lerp(camFov, boxCameraFov, time / movingTime);
                yield return null;
            }
        }

        IEnumerator hightlightBox()
        {
            yield return new WaitUntil(() => (boxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Closing")));
            yield return new WaitWhile(() => (boxAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f));

            finishCanvas.enabled = true;
            boxCamera.enabled = true;

            boxAnimator.SetTrigger("Roll");

            yield return null;

            yield return new WaitUntil(() => (boxAnimator.GetCurrentAnimatorStateInfo(1).IsName("Rolling")));
            yield return new WaitWhile(() => (boxAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime < 0.99f));

            tapToContinue.SetActive(true);

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            //particles

            finishCanvas.enabled = false;
            boxCamera.enabled = false;
            shop.finishOrder();
        }

        IEnumerator coinIconEffect()
        {
            var size = 3f;
            var effectTime = 0.2f;

            for (var time = 0f; time <= effectTime * 2; time += Time.deltaTime)
            {
                if (time <= effectTime)
                {
                    coinIcon.transform.localScale = Vector3.one * (1 + size * time);
                }
                else
                {
                    coinIcon.transform.localScale = Vector3.one * (2 * size * effectTime + 1 - time * size);
                }

                yield return null;
            }
        }
    }
}