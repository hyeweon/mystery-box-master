using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Katniss {

    public delegate void ShopEventHandler(Order _order);

    public class Shop : MonoBehaviour
    {
        public bool goodReaction = false;

        private int money = 0;
        private int orderIndex;

        [SerializeField] private int orderCount = 3;
        [SerializeField] private int[] amounts;

        private Order[] orders;

        [SerializeField] private CandyType[] essentialCandyTypes;

        [SerializeField] private Canvas dmCanvas;
        [SerializeField] private TextMeshProUGUI moneyTMP;

        public event ShopEventHandler newOrderEvent;

        void Start()
        {
            orderIndex = 0;

            orders = new Order[orderCount];
            for(int i = 0; i < orderCount; i++)
            {
                orders[i] = new Order(amounts[i], essentialCandyTypes[i]);
            }

            updateMoney();
        }

        public void acceptOrder()
        {
            updateMoney();

            newOrderEvent(orders[orderIndex]);

            orderIndex++;
            money += orders[orderIndex].amount;
            dmCanvas.enabled = false;
        }

        public void finishOrder()
        {
            dmCanvas.enabled = true;
        }

        void updateMoney()
        {
            moneyTMP.text = $"{money}";
        }
    }
}