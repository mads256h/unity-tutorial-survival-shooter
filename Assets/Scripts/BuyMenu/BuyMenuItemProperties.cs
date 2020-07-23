namespace BuyMenu
{
    public sealed class BuyMenuItemProperties
    {
        public BuyMenuItemProperties(string title, int basePrice, double priceMultiplier, OnPurchaseDelegate onPurchase)
        {
            Title = title;
            BasePrice = basePrice;
            PriceMultiplier = priceMultiplier;
            OnPurchase = onPurchase;
        }
        
        public string Title { get; }
        public int BasePrice { get; }
        public double PriceMultiplier { get; }
        public delegate void OnPurchaseDelegate(int level);
        public OnPurchaseDelegate OnPurchase { get; }
    }
}