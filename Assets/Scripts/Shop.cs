using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Katniss {

    public delegate void ShopEventHandler(Order _order);

    public class Shop : MonoBehaviour
    {
        public bool goodReaction = false;

        private int money = 0;
        private int orderIndex;

        private Vector3 targetCoinPos = new Vector3(250, 535, 0);

        [SerializeField] private Camera particleCamera;
        [SerializeField] private ParticleSystem heartParticle;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioSource moneyAudioSource;

        [SerializeField] private Button acceptBtn;
        [SerializeField] private GameObject declineBtn;
        [SerializeField] private GameObject premiumBtn;
        [SerializeField] private GameObject normalBtn;
        [SerializeField] private GameObject secretCoin;
        [SerializeField] private GameObject newItemImage;
        [SerializeField] private Canvas dmCanvas;
        [SerializeField] private Canvas adCanvas;
        [SerializeField] private Image bgImage;
        [SerializeField] private TextMeshProUGUI moneyTMP;

        [SerializeField] private Order[] orders;
        public CandyType[] candyTypes;

        public event ShopEventHandler newOrderEvent;

        void Start()
        {
            orderIndex = PlayerPrefs.GetInt("OrderIndex");
            money = PlayerPrefs.GetInt("Money");

            particleCamera.enabled = false;
            adCanvas.enabled = false;
            acceptBtn.gameObject.SetActive(false);
            declineBtn.SetActive(false);
            premiumBtn.SetActive(false);
            normalBtn.SetActive(false);
            secretCoin.SetActive(false);
            newItemImage.SetActive(false);
            bgImage.sprite = orders[orderIndex].baseSprite;
            moneyTMP.text = $"{money}";

            if (orderIndex == 2)
            {
                for (int i = 0; i < 7; i++)
                {
                    candyTypes[i].isActive = true;
                }
            }

            StartCoroutine(showOrder());
        }

        public void acceptOrder()
        {
            acceptBtn.enabled = false;

            money += orders[orderIndex].amount;

            for (int i = 0; i < 2 + orders[orderIndex].amount / 5; i++)
            {
                GameObject newCoin = Instantiate(secretCoin, dmCanvas.transform, true);
                StartCoroutine(earnMoney(newCoin));
            }
            secretCoin.SetActive(false);

            StartCoroutine(startOrder());
        }

        public void finishOrder()
        {
            dmCanvas.enabled = true;
            StartCoroutine(showReaction());
        }

        public void nextOrder()
        {
            PlayerPrefs.SetInt("OrderIndex", ++orderIndex);
            PlayerPrefs.SetInt("Money", money);
            StartCoroutine(nextPremiumOrder());
        }

        void updateMoney()
        {
            moneyTMP.text = $"{money}";
            if (!moneyAudioSource.isPlaying)
                moneyAudioSource.Play();
        }

        IEnumerator showOrder()
        {
            foreach (Sprite sprite in orders[orderIndex].dmSprites)
            {
                yield return new WaitForSeconds(0.5f);
                bgImage.sprite = sprite;
                source.Play();
            }

            yield return new WaitForSeconds(0.5f);

            acceptBtn.gameObject.SetActive(true);
            secretCoin.SetActive(true);
            declineBtn.SetActive(true);
        }

        IEnumerator startOrder()
        {
            bgImage.sprite = orders[orderIndex].acceptSprite;
            source.Play();
            yield return new WaitForSeconds(1.5f);

            newOrderEvent(orders[orderIndex]);
            acceptBtn.gameObject.SetActive(false);
            declineBtn.SetActive(false);
            dmCanvas.enabled = false;
        }

        IEnumerator showReaction()
        {
            bgImage.sprite = orders[orderIndex].baseSprite;

            if (goodReaction)
            {
                particleCamera.enabled = true;

                foreach (Sprite sprite in orders[orderIndex].goodSprites)
                {
                    yield return new WaitForSeconds(0.5f);
                    bgImage.sprite = sprite;
                    source.Play();
                }
                heartParticle.Play();
            }

            premiumBtn.SetActive(true);

            yield return new WaitForSeconds(2f);

            normalBtn.SetActive(true);
        }

        IEnumerator nextPremiumOrder()
        {
            adCanvas.enabled = true;
            yield return new WaitForSeconds(1f);

            if (money == 5)
            {
                newItemImage.SetActive(true);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                candyTypes[2].isActive = true;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        IEnumerator earnMoney(GameObject coin)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            var coinPos = coin.transform.localPosition;

            for (var time = 0f; time < 0.5f; time += Time.deltaTime)
            {
                coin.transform.localPosition = Vector3.Lerp(coinPos, targetCoinPos, time / 0.5f);
                yield return null;
            }

            Destroy(coin);
            updateMoney();
        }
    }
}