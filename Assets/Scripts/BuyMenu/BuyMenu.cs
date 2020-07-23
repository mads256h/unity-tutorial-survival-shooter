using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;

namespace BuyMenu
{
    public class BuyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject itemsPanel;

        [SerializeField] private BuyMenuItem itemAsset;

        [SerializeField] private PlayerShooting playerShooting;

        [SerializeField] private Material gunLineUber;

        private int _currentY = -10;
        
        // Start is called before the first frame update
        void Start()
        {
            AddPanel(new BuyMenuItemProperties("Gun firerate",100, 2.0, (level) =>
            {
                playerShooting.timeBetweenBullets /= 2.0f;
                Debug.Log("1: " + level);
            }));
            AddPanel(new BuyMenuItemProperties("Gun damage",200, 3.0, (level) =>
            {
                playerShooting.damagePerShot *= 2;
                if (level == 1)
                {
                    playerShooting.gunLine.material = gunLineUber;
                }
                Debug.Log("2: " + level);
            }));
        }

        private void AddPanel(BuyMenuItemProperties properties)
        {
            var item = Instantiate(itemAsset, itemsPanel.transform);

            item.Initialize(properties);

            var rectTransform = item.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, _currentY);

            _currentY = -((int)rectTransform.rect.height) + -10;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }   
}
