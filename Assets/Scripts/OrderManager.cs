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

        private Order currOrder;

        [SerializeField] private Image amountProgressFill;

        [SerializeField] private Shop shop;
        [SerializeField] private Player player;

        void Start()
        {
            amountProgressFill.fillAmount = 0f;
            shop.newOrderEvent += new ShopEventHandler(setOrder);
            player.putCandyEvent += new PlayerEventHandler(getCandy);
        }

        void setOrder(Order order)
        {
            currOrder = order;
            player.isInProcessing = true;
        }

        void getCandy(Candy candy)
        {
            amount += candy.type.price;
            StartCoroutine(fillAmountProgress());

            if (candy.type == currOrder.essentialCandyType)
            {
                isContaining = true;
            }

            if (isContaining && amount >= currOrder.amount)
            {
                isComplete = true;
            }
        }

        public void finishOrder()
        {
            if (isComplete)
                shop.goodReaction = true;
            player.isInProcessing = false;
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
    }
}