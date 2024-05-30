using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinsUI;
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtn;
    private PlayerBehavior player;
    private Scout_Move move;
    // public GameObject ply;

    //private CanvasGroup canvasGroup;

    public Canvas canvas;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerBehavior>();
        move = FindAnyObjectByType<Scout_Move>();
        //canvasGroup.alpha = 0f;
        canvas.enabled = false;
        for (int i = 0; i < shopItemsSO.Length; i++)
            shopPanelsGO[i].SetActive(true);

        coinsUI.text = "Coins: " + coins.ToString();
        //canvasGroup = GetComponent<CanvasGroup>();

        LoadPanels();
        checkPurchasable();
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.P))
        {
            canvas.enabled = !canvas.enabled;
        }

        
    }

    public void AddCoins() {
        coins++;
        coinsUI.text = "Coins: " + coins.ToString();
        checkPurchasable();
    }

    public void LoadPanels() {
        for(int i = 0;i < shopItemsSO.Length; i++)
        {
            //Debug.Log("i = " + i);
            //Debug.Log("shopItemsSo.length = " + shopItemsSO.Length);
            //Debug.Log("shopPanels.length = " + shopPanels.Length);
            shopPanels[i].titleTxt.text = shopItemsSO[i].title;
            shopPanels[i].descriptionTxt.text = shopItemsSO[i].description;
            shopPanels[i].costTxt.text = "Coins: " + shopItemsSO[i].baseCost.ToString();
        }
    }

    public void checkPurchasable()
    {
        for(int i = 0; i < shopItemsSO.Length; i++)
        {
            if (coins >= shopItemsSO[i].baseCost)
                myPurchaseBtn[i].interactable = true;
            else
                myPurchaseBtn[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if(coins >= shopItemsSO[btnNo].baseCost)
        {
            bool purchased = false;

            // heart
            if(btnNo == 0)
            {
                if(player.currentHealth + 25 <= player.startingHealth)
                {
                    player.IncreaseHealth();
                    purchased = true;

                    // player.healthBar.transform
                }
            }

            // speed
            else if(btnNo == 1)
            {
                move.Speed += 0.3f;
                purchased = true;
            }

            // jump
            else if(btnNo == 2)
            {
                move.Jump += 0.3f;
                purchased = true;
            }

            // damage increase
            else
            {
                move.IncraseAmmo();
                purchased = true;
            }


            if(purchased)
            {
                coins = coins - shopItemsSO[btnNo].baseCost;
                coinsUI.text = "Coins: " + coins.ToString();
                checkPurchasable();
            }
        }
    }
}
