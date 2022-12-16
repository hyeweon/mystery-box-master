using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Katniss
{
    public class PrefsReset : MonoBehaviour
    {
        private void Reset()
        {
            PlayerPrefs.DeleteAll();
            Shop shop = GameObject.FindGameObjectWithTag("Shop").GetComponentInChildren<Shop>();
            for (int i = 2; i < shop.candyTypes.Length; i++)
            {
                shop.candyTypes[i].isActive = false;
            }
        }
    }

}