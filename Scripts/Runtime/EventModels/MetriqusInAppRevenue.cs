using System.Collections.Generic;

namespace MetriqusSdk
{
    /// <summary>
    /// Represents in-app revenue details, including product information, pricing, promotions, and transaction IDs.
    /// </summary>
    public class MetriqusInAppRevenue
    {
        /// <summary>
        /// The total revenue amount for the in-app purchase.
        /// </summary>
        public double? Revenue { get; private set; }

        /// <summary>
        /// The currency of the transaction.
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// The unique identifier for the purchased product.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// The name of the purchased product.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// The brand associated with the product.
        /// </summary>
        public string Brand { get; set; } = null;

        /// <summary>
        /// The product variant.
        /// </summary>
        public string Variant { get; set; } = null;

        /// <summary>
        /// The primary product category.
        /// </summary>
        public string Category { get; set; } = null;

        /// <summary>
        /// Additional product category levels.
        /// </summary>
        public string Category2 { get; set; } = null;
        public string Category3 { get; set; } = null;
        public string Category4 { get; set; } = null;
        public string Category5 { get; set; } = null;

        /// <summary>
        /// The price per unit of the product.
        /// </summary>
        public double? Price { get; set; } = null;

        /// <summary>
        /// The quantity of items purchased.
        /// </summary>
        public int? Quantity { get; set; } = null;

        /// <summary>
        /// The amount refunded for this purchase.
        /// </summary>
        public double? Refund { get; set; } = null;

        /// <summary>
        /// The applied coupon code, if any.
        /// </summary>
        public string Coupon { get; set; } = null;

        /// <summary>
        /// The store or affiliate responsible for the sale.
        /// </summary>
        public string Affiliation { get; set; } = null;

        /// <summary>
        /// The location identifier of the sale.
        /// </summary>
        public string LocationId { get; set; } = null;

        /// <summary>
        /// Identifier for the list in which the product appears.
        /// </summary>
        public string ListId { get; set; } = null;

        /// <summary>
        /// The name of the product list.
        /// </summary>
        public string ListName { get; set; } = null;

        /// <summary>
        /// The index position of the product within a list.
        /// </summary>
        public int? ListIndex { get; set; } = null;

        /// <summary>
        /// Promotion details for the product.
        /// </summary>
        public string PromotionId { get; set; } = null;
        public string PromotionName { get; set; } = null;

        /// <summary>
        /// The name and slot of the creative asset used in advertising.
        /// </summary>
        public string CreativeName { get; set; } = null;
        public string CreativeSlot { get; set; } = null;

        /// <summary>
        /// Additional parameters for the item.
        /// </summary>
        public List<TypedParameter> ItemParams { get; set; } = null;

#if UNITY_ANDROID
        /// <summary>
        /// The purchase token for Android transactions.
        /// </summary>
        public string PurchaseToken;
#elif UNITY_IOS
        /// <summary>
        /// The transaction ID for iOS purchases.
        /// </summary>
        public string TransactionId { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusInAppRevenue"/> class.
        /// </summary>
        public MetriqusInAppRevenue() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusInAppRevenue"/> class with a specified revenue amount and currency.
        /// </summary>
        /// <param name="amount">The revenue amount.</param>
        /// <param name="currency">The currency of the revenue.</param>
        public MetriqusInAppRevenue(double amount, string currency)
        {
            this.Revenue = amount;
            this.Currency = currency;
        }

        /// <summary>
        /// Sets the revenue amount and currency.
        /// </summary>
        /// <param name="amount">The revenue amount.</param>
        /// <param name="currency">The currency of the revenue.</param>
        public void SetRevenue(double amount, string currency)
        {
            this.Revenue = amount;
            this.Currency = currency;
        }

        /// <summary>
        /// Sets the transaction ID based on the platform.
        /// </summary>
        /// <param name="id">The transaction ID or purchase token.</param>
        public void SetTransactionId(string id)
        {
#if UNITY_ANDROID
        PurchaseToken = id;
#elif UNITY_IOS
            TransactionId = id;
#endif
        }

        /// <summary>
        /// Retrieves the transaction ID based on the platform.
        /// </summary>
        /// <returns>The transaction ID or purchase token.</returns>
        public string GetTransactionId()
        {
#if UNITY_ANDROID
        return PurchaseToken;
#elif UNITY_IOS
            return TransactionId;
#else
        return "";
#endif
        }
    }
}