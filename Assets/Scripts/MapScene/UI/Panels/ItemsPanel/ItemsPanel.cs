﻿using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown {

    public class ItemsPanel : MonoBehaviour, IItemsContainer {
        public GameObject itemPrefab;
        public Transform itemsLocation;

        public GameObject[] Items {
            get {
                return items;
            }
        }

        private readonly GameObject[] items = new GameObject[ItemsOptions.totalAmount];

        private void Start() {
            gameObject.SetActive(false);
            Dictionary<ItemProperty, string> itemDict;
            for (int i = 0; i < ItemsOptions.totalAmount; i++) {
                itemDict = ItemsOptions.GetItemData(i);

                items[i] = Instantiate(itemPrefab, itemsLocation);
                items[i].GetComponent<ItemCustomization>().number = i;
                items[i].GetComponent<ItemCustomization>().itemImage.sprite = ItemsOptions.Instance.itemsSprites[i];
                items[i].GetComponent<ItemCustomization>().itemName.text = itemDict[ItemProperty.name];
            }
            UpdateItemsPanel();
        }


        public void UpdateItemsPanel() {
            for (int i = 0; i < ItemsOptions.totalAmount; i++) {
                UpdateSingleItemWithAnimation(i);
            }
        }


        public void UpdateSingleItem(int num) {
            items[num].GetComponent<ItemCustomization>().itemCount.text = DataScript.SData.ItemsCount[num].ToString();
            if (DataScript.SData.ItemsCount[num] == 0)
                items[num].gameObject.SetActive(false);
            else
                items[num].gameObject.SetActive(true);
        }

        public void UpdateSingleItemWithAnimation(int num) {
            int count;
            if (DataScript.SData.ItemsCount[num] == 0)
                items[num].gameObject.SetActive(false);
            else if (int.TryParse(items[num].GetComponent<ItemCustomization>().itemCount.text, out count)) {
                items[num].gameObject.SetActive(true);
                if (DataScript.SData.ItemsCount[num] != count) {
                    items[num].GetComponent<ItemCustomization>().itemCount.text = DataScript.SData.ItemsCount[num].ToString();
                    Debug.Log("++");
                    if (gameObject.activeInHierarchy)
                        StartCoroutine(items[num].GetComponent<ItemCustomization>().ItemAnimation());
                }
            } else
                UpdateSingleItem(num);
        }
    }

}