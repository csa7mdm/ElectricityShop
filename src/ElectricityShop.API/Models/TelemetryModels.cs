using System.Collections.Generic;

namespace ElectricityShop.API.Models
{
    /// <summary>
    /// Example request model for telemetry login
    /// </summary>
    public class TelemetryLoginRequest
    {
        /// <summary>
        /// Username for login
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Password for login
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Example request model for order creation
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// List of items in the order
        /// </summary>
        public List<OrderItem> Items { get; set; }
        
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Example order item model
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// ID of the product
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Quantity of the product
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }
    }
}