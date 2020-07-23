using System.Collections;
using System.Collections.Generic;
using BuyMenu;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuyMenuItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI price;

    [SerializeField] private Button button;
    
    [SerializeField] private Image[] images;

    [SerializeField] private Sprite activatedSprite;
    
    private BuyMenuItemProperties _properties;

    private int _currentPrice = 0;

    private int _level = 0;
    

    public void Initialize(BuyMenuItemProperties properties)
    {
        _properties = properties;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        title.text = _properties.Title;
        _currentPrice = _properties.BasePrice;
        price.text = "$" + _currentPrice;
    }

    public void OnButtonPress()
    {
        images[_level].sprite = activatedSprite;
        ScoreManager.score -= _currentPrice;
        _level++;
        _properties.OnPurchase(_level);
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        _currentPrice = (int)((float)_currentPrice * _properties.PriceMultiplier);
        price.text = "$" + _currentPrice;
    }
    
    // Update is called once per frame
    private void Update()
    {
        button.interactable = ScoreManager.score >= _currentPrice;
    }
}
