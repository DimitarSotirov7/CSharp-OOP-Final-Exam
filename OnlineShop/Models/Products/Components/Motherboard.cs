﻿namespace OnlineShop.Models.Products.Components
{
    public class Motherboard : Component
    {
        private const double DefaultMultiplier = 1.25;

        public Motherboard(int id, string manufacturer, string model, decimal price, double overallPerformance, int generation)
            : base(id, manufacturer, model, price, overallPerformance * DefaultMultiplier, generation)
        {
        }
    }
}
