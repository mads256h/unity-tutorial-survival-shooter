using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.Serialization;

namespace BuyMenu
{
    public class BuyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject itemsPanel;

        [SerializeField] private BuyMenuItem itemAsset;

        [SerializeField] private PlayerShooting playerShooting;

        private int _currentY = -10;
        
        // Start is called before the first frame update
        private void Start()
        {
            AddPanel(new BuyMenuItemProperties("Gun firerate",1, 2.0, playerShooting.UpgradeFirerate));
            AddPanel(new BuyMenuItemProperties("Gun damage",1, 1.0, playerShooting.UpgradeDamage));
        }

        private void AddPanel(BuyMenuItemProperties properties)
        {
            var item = Instantiate(itemAsset, itemsPanel.transform);

            item.Initialize(properties);

            var rectTransform = item.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, _currentY);

            _currentY = -((int)rectTransform.rect.height) + -10;
        }
    }   
}
