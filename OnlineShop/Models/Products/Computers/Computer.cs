using OnlineShop.Common.Constants;
using OnlineShop.Models.Products.Components;
using OnlineShop.Models.Products.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Models.Products.Computers
{
    public abstract class Computer : Product, IComputer
    {
        private List<IComponent> components;
        private List<IPeripheral> peripherals;

        public Computer(int id, string manufacturer, string model, decimal price, double overallPerformance)
            : base(id, manufacturer, model, price, overallPerformance)
        {
            this.components = new List<IComponent>();
            this.peripherals = new List<IPeripheral>();
        }

        public override double OverallPerformance
        {
            get
            {
                if (this.components.Count == 0)
                {
                    return double.Parse(base.OverallPerformance.ToString("0.00"));
                }
                else
                {
                    double num = base.OverallPerformance + this.components.Average(c => c.OverallPerformance);

                    return double.Parse(num.ToString("0.00"));
                }
            }
        }

        public override decimal Price
        {
            get
            {
                decimal num = base.Price + this.components.Sum(c => c.Price) + this.peripherals.Sum(p => p.Price);

                return decimal.Parse(num.ToString("0.00"));
            }
        }

        public IReadOnlyCollection<IComponent> Components => this.components;

        public IReadOnlyCollection<IPeripheral> Peripherals => this.peripherals;

        public void AddComponent(IComponent component)
        {
            bool sameType = this.components.Any(c => c.GetType().Name == component.GetType().Name);

            if (sameType)
            {
                string msg = string.Format(ExceptionMessages.ExistingComponent, component.GetType().Name, this.GetType().Name, this.Id);

                throw new ArgumentException(msg);
            }

            this.components.Add(component);
        }

        public void AddPeripheral(IPeripheral peripheral)
        {
            bool sameType = this.peripherals.Any(c => c.GetType().Name == peripheral.GetType().Name);

            if (sameType)
            {
                string msg = string.Format(ExceptionMessages.ExistingPeripheral, peripheral.GetType().Name, this.GetType().Name, this.Id);

                throw new ArgumentException(msg);
            }

            this.peripherals.Add(peripheral);
        }

        public IComponent RemoveComponent(string componentType)
        {
            IComponent component = this.components.FirstOrDefault(c => c.GetType().Name == componentType);

            if (component == null)
            {
                string msg = string.Format(ExceptionMessages.NotExistingComponent, componentType, this.GetType().Name, this.Id);

                throw new ArgumentException(msg);
            }

            this.components = this.components
                .Where(c => c.GetType().Name != componentType)
                .ToList();

            return component;
        }

        public IPeripheral RemovePeripheral(string peripheralType)
        {
            IPeripheral peripheral = this.peripherals.FirstOrDefault(c => c.GetType().Name == peripheralType);

            if (peripheral == null)
            {
                string msg = string.Format(ExceptionMessages.NotExistingPeripheral, peripheralType, this.GetType().Name, this.Id);

                throw new ArgumentException(msg);
            }

            this.peripherals = this.peripherals
                .Where(c => c.GetType().Name != peripheralType)
                .ToList();

            return peripheral;
        }

        public override string ToString()
        {
            var report = new StringBuilder();
            report.AppendLine(base.ToString());

            report.AppendLine($" Components ({this.components.Count}):");

            foreach (var component in this.components)
            {
                report.AppendLine("  " + component.ToString());
            }

            var averagePerformance = this.peripherals.Average(p => p.OverallPerformance);

            report.AppendLine($" Peripherals ({this.peripherals.Count}); Average Overall Performance ({averagePerformance.ToString("0.00")}):");

            foreach (var peripheral in this.peripherals)
            {
                report.AppendLine("  " + peripheral.ToString());
            }

            return report.ToString().TrimEnd();
        }
    }
}
