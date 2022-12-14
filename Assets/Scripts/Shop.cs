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

        [SerializeField] private Button acceptBtn;
        [SerializeField] private GameObject declineBtn;
        [SerializeField] private GameObject premiumBtn;
        [SerializeField] private GameObject normalBtn;
        [SerializeField] private Canvas dmCanvas;
        [SerializeField] private Image bgImage;
        [SerializeField] private TextMeshProUGUI moneyTMP;

        [SerializeField] private Order[] orders;

        public event ShopEventHandler newOrderEvent;

        void Start()
        {
            orderIndex = 0;

            acceptBtn.gameObject.SetActive(false);
            declineBtn.SetActive(false);
            premiumBtn.SetActive(false);
            normalBtn.SetActive(false);
            bgImage.sprite = orders[orderIndex].baseSprite;
            updateMoney();

            StartCoroutine(showOrder());
        }

        public void acceptOrder()
        {
            acceptBtn.enabled = false;

            money += orders[orderIndex].amount;
            updateMoney();

            StartCoroutine(startOrder());
        }

        public void finishOrder()
        {
            dmCanvas.enabled = true;
            StartCoroutine(showReaction());
        }

        void updateMoney()
        {
            moneyTMP.text = $"{money}";
        }

        IEnumerator showOrder()
        {
            foreach (Sprite sprite in orders[orderIndex].dmSprites)
            {
                yield return new WaitForSeconds(1f);
                bgImage.sprite = sprite;
            }

            yield return new WaitForSeconds(0.5f);

            acceptBtn.gameObject.SetActive(true);
            declineBtn.SetActive(true);
        }

        IEnumerator startOrder()
        {
            bgImage.sprite = orders[orderIndex].acceptSprite;
            yield return new WaitForSeconds(1f);

            newOrderEvent(orders[orderIndex]);
            dmCanvas.enabled = false;
            acceptBtn.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            declineBtn.SetActive(false);
        }

        IEnumerator showReaction()
        {
            if (goodReaction)
            {
                foreach (Sprite sprite in orders[orderIndex].goodSprites)
                {
                    bgImage.sprite = sprite;
                    yield return new WaitForSeconds(0.5f);
                }
            }

            premiumBtn.SetActive(true);

            yield return new WaitForSeconds(3f);

            normalBtn.SetActive(true);
        }
    }
}