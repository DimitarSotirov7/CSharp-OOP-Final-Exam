using OnlineShop.Common.Constants;
using OnlineShop.Common.Enums;
using OnlineShop.Models.Products.Components;
using OnlineShop.Models.Products.Computers;
using OnlineShop.Models.Products.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShop.Core
{
    public class Controller : IController
    {
        private List<IComputer> computers;
        private List<IComponent> components;
        private List<IPeripheral> peripherals;

        public Controller()
        {
            this.computers = new List<IComputer>();
            this.components = new List<IComponent>();
            this.peripherals = new List<IPeripheral>();
        }

        public string AddComponent(int computerId, int id, string componentType, string manufacturer, string model, decimal price, double overallPerformance, int generation)
        {
            this.CheckIfComputerExists(computerId);

            if (this.components.Any(c => c.Id == id))
            {
                throw new ArgumentException(ExceptionMessages.ExistingComponentId);
            }

            IComponent component = null;

            if (componentType == ComponentType.CentralProcessingUnit.ToString())
            {
                component = new CentralProcessingUnit(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else if (componentType == ComponentType.Motherboard.ToString())
            {
                component = new Motherboard(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else if (componentType == ComponentType.PowerSupply.ToString())
            {
                component = new PowerSupply(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else if (componentType == ComponentType.RandomAccessMemory.ToString())
            {
                component = new RandomAccessMemory(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else if (componentType == ComponentType.SolidStateDrive.ToString())
            {
                component = new SolidStateDrive(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else if (componentType == ComponentType.VideoCard.ToString())
            {
                component = new VideoCard(id, manufacturer, model, price, overallPerformance, generation);

                this.components.Add(component);
            }
            else
            {
                throw new ArgumentException(ExceptionMessages.InvalidComponentType);
            }

            foreach (var computer in this.computers.Where(c => c.Id == computerId))
            {
                computer.AddComponent(component);
            }

            return string.Format(SuccessMessages.AddedComponent, componentType, id, computerId);
        }

        public string AddComputer(string computerType, int id, string manufacturer, string model, decimal price)
        {
            bool exists = this.computers.Any(c => c.Id == id);

            if (exists)
            {
                throw new ArgumentException(ExceptionMessages.ExistingComputerId);
            }

            if (computerType == ComputerType.DesktopComputer.ToString())
            {
                this.computers.Add(new DesktopComputer(id, manufacturer, model, price));
            }
            else if (computerType == ComputerType.Laptop.ToString())
            {
                this.computers.Add(new Laptop(id, manufacturer, model, price));
            }
            else
            {
                throw new ArgumentException(ExceptionMessages.InvalidComputerType);
            }

            return string.Format(SuccessMessages.AddedComputer, id);
        }

        public string AddPeripheral(int computerId, int id, string peripheralType, string manufacturer, string model, decimal price, double overallPerformance, string connectionType)
        {
            this.CheckIfComputerExists(computerId);

            if (this.peripherals.Any(c => c.Id == id))
            {
                throw new ArgumentException(ExceptionMessages.ExistingPeripheralId);
            }

            IPeripheral peripheral = null;

            if (peripheralType == PeripheralType.Headset.ToString())
            {
                peripheral = new Headset(id, manufacturer, model, price, overallPerformance, connectionType);

                this.peripherals.Add(peripheral);
            }
            else if (peripheralType == PeripheralType.Keyboard.ToString())
            {
                peripheral = new Keyboard(id, manufacturer, model, price, overallPerformance, connectionType);

                this.peripherals.Add(peripheral);
            }
            else if (peripheralType == PeripheralType.Monitor.ToString())
            {
                peripheral = new Monitor(id, manufacturer, model, price, overallPerformance, connectionType);

                this.peripherals.Add(peripheral);
            }
            else if (peripheralType == PeripheralType.Mouse.ToString())
            {
                peripheral = new Mouse(id, manufacturer, model, price, overallPerformance, connectionType);

                this.peripherals.Add(peripheral);
            }
            else
            {
                throw new ArgumentException(ExceptionMessages.InvalidPeripheralType);
            }

            foreach (var computer in this.computers.Where(c => c.Id == computerId))
            {
                computer.AddPeripheral(peripheral);
            }

            return string.Format(SuccessMessages.AddedPeripheral, peripheralType, id, computerId);
        }

        public string BuyBest(decimal budget)
        {
            IComputer computer = this.computers
                 .Where(c => c.Price <= budget)
                 .OrderByDescending(c => c.OverallPerformance)
                 .FirstOrDefault();

            if (computer == null)
            {
                throw new ArgumentException(string.Format(ExceptionMessages.CanNotBuyComputer, budget));
            }
            else
            {
                this.computers.Remove(computer);

                return computer.ToString();
            }
        }

        public string BuyComputer(int id)
        {
            this.CheckIfComputerExists(id);

            IComputer computer = this.computers.FirstOrDefault(c => c.Id == id);

            this.computers = this.computers
                .Where(c => c.Id != id)
                .ToList();

            if (computer != null)
            {
                return computer.ToString();
            }
            else
            {
                return null;
            }
        }

        public string GetComputerData(int id)
        {
            this.CheckIfComputerExists(id);

            IComputer computer = this.computers
                 .Where(c => c.Id == id)
                 .FirstOrDefault();

            if (computer == null)
            {
                return null;
            }
            else
            {
                return computer.ToString();
            }
        }

        public string RemoveComponent(string componentType, int computerId)
        {
            this.CheckIfComputerExists(computerId);

            IComponent component = this.computers
                .FirstOrDefault(c => c.Id == computerId)
                .Components.FirstOrDefault(c => c.GetType().Name == componentType);

            foreach (var computer in this.computers.Where(c => c.Id == computerId))
            {
                computer.RemoveComponent(componentType);
            }

            this.components = this.components
                .Where(c => c.GetType().Name != componentType)
                .ToList();

            if (component != null)
            {
                return string.Format(SuccessMessages.RemovedComponent, componentType, component.Id);
            }
            else
            {
                return null;
            }
        }

        public string RemovePeripheral(string peripheralType, int computerId)
        {
            this.CheckIfComputerExists(computerId);

            IPeripheral peripheral = this.computers
                .FirstOrDefault(c => c.Id == computerId)
                .Peripherals.FirstOrDefault(p => p.GetType().Name == peripheralType);

            foreach (var computer in this.computers.Where(c => c.Id == computerId))
            {
                computer.RemovePeripheral(peripheralType);
            }

            this.peripherals = this.peripherals
                .Where(p => p.GetType().Name != peripheralType)
                .ToList();

            if (peripheral != null)
            {
                return string.Format(SuccessMessages.RemovedPeripheral, peripheralType, peripheral.Id);
            }
            else
            {
                return null;
            }
        }

        private void CheckIfComputerExists(int computerId)
        {
            if (!this.computers.Any(c => c.Id == computerId))
            {
                throw new ArgumentException(ExceptionMessages.NotExistingComputerId);
            }
        }
    }
}
