using System;
using System.Collections.Generic;

namespace Car_service
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CarService carService = new CarService();
            carService.Work();
        }
    }

    class UserUtils
    {
        private static Random _random = new Random();

        public static bool GenerateRandomStatus(int trueNumber, int falseNumber)
        {
            int status = _random.Next(falseNumber, trueNumber + 1);
            return status == trueNumber;
        }
    }

    class CarService
    {
        private Queue<Car> _cars = new Queue<Car>();
        private List<CarDetail> _warehouse = new List<CarDetail>();
        private int _money = 0;

        public CarService()
        {
            CreateQueueClients();
            FillWarehouse();
        }

        public void Work()
        {
            const string CommandStart = "1";
            const string CommandStop = "2";

            Console.WriteLine("Добро пожаловать в автосервис!");
            bool isWork = true;

            while (isWork)
            {
                Console.WriteLine($"На данный момент в очереди на починку {_cars.Count} машин.");
                Console.WriteLine($"Баланс - {_money} .");
                Console.WriteLine($"Введите {CommandStart} , чтобы обслужить следующую машину.");
                Console.WriteLine($"Введите {CommandStop} , чтобы завершить работу.");
                CreateNewClient();
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case CommandStart:
                        RepairCar(_cars.Dequeue());
                        break;

                    case CommandStop:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Неккоректный ввод...");
                        break;
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void CreateNewClient()
        {
            if (_cars.Count == 0)
            {
                Console.WriteLine("К нам приехал новый клиент!");
                _cars.Enqueue(new Car());
            }
        }

        private void CreateQueueClients()
        {
            int quantityCars = 3;

            for (int i = 0; i < quantityCars; i++)
            {
                _cars.Enqueue(new Car());
            }
        }

        private void FillWarehouse()
        {
            int quantityOneNameDetail = 2;
            bool detailServiseableState = true;

            for (int i = 0; i < quantityOneNameDetail; i++)
            {
                _warehouse.Add(new CarDetail("Двигатель", 100000, detailServiseableState));
                _warehouse.Add(new CarDetail("Коробка передач", 120000, detailServiseableState));
                _warehouse.Add(new CarDetail("Трансмиссия", 80000, detailServiseableState));
                _warehouse.Add(new CarDetail("Аккамулятор", 40000, detailServiseableState));
                _warehouse.Add(new CarDetail("Подвеска", 50000, detailServiseableState));
            }
        }

        private void ShowDetailsInWarehouse()
        {
            Console.WriteLine("В наличии следущие детали: ");

            for (int i = 0; i < _warehouse.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {_warehouse[i].Name}.");
            }
        }

        private void RepairCar(Car car)
        {
            bool isWork = true;

            while (isWork == true)
            {
                if (car.IsFullServisiable() == false)
                {
                    Console.WriteLine($"Баланс - {_money} .");
                    int workCost = 10000;
                    int penalty = 5000;
                    Console.WriteLine("Машина клиента: ");
                    car.ShowTechicalState();
                    Console.WriteLine("\nВыберите, какую деталь вы хотите поменять: ");
                    ShowDetailsInWarehouse();
                    string userInput = Console.ReadLine();

                    if (int.TryParse(userInput, out int detailWarehouseIndex))
                    {
                        if (detailWarehouseIndex > 0 && detailWarehouseIndex <= _warehouse.Count && _warehouse.Count > 0)
                        {
                            if (car.ReplaceDetail(_warehouse[detailWarehouseIndex - 1]))
                            {
                                Console.WriteLine("Вы успешно заменили деталь.");
                                _money += workCost + _warehouse[detailWarehouseIndex - 1].Price;
                                Console.WriteLine($"Стоимость работ - {workCost} + стоимость детали - {_warehouse[detailWarehouseIndex - 1].Price}");
                                _warehouse.Remove(_warehouse[detailWarehouseIndex - 1]);
                            }
                            else
                            {
                                Console.WriteLine("Вы пытались заменить исправную деталь...");
                                Console.WriteLine($"Вам придётся оплатить штраф в размере - {penalty}!");
                                _money -= penalty;
                                Console.WriteLine("Недовольный клиент покидает автосервис!");
                                isWork = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Такой детали нет на складе или на складе нет необходимых деталей...");
                            Console.WriteLine("Недовольный клиент покидает автосервис!");
                            isWork = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неккоректный ввод...");
                    }
                }
                else
                {
                    Console.WriteLine("Диагностика не выявила неисправностей в машине клиента. Клиент уезжает.");
                    isWork = false;
                }

                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    class CarDetail
    {
        public CarDetail(string name, int price, bool status)
        {
            Name = name;
            Price = price;
            Status = status;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }
        public bool Status { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Деталь - {Name} . Цена - {Price} . Состояние - {ShowStatus()}.");
        }

        public void ChangeStatus()
        {
            if (Status == false)
            {
                Status = true;
            }
        }

        private string ShowStatus()
        {
            string serviceable = "Исправен";
            string defective = "Поврежден";

            if (Status)
                return serviceable;
            else
                return defective;
        }
    }

    class Car
    {
        private List<CarDetail> _details = new List<CarDetail>();

        public Car()
        {
            int serviceable = 1;
            int defective = 0;
            _details.Add(new CarDetail("Двигатель", 100000, UserUtils.GenerateRandomStatus(serviceable, defective)));
            _details.Add(new CarDetail("Коробка передач", 120000, UserUtils.GenerateRandomStatus(serviceable, defective)));
            _details.Add(new CarDetail("Трансмиссия", 80000, UserUtils.GenerateRandomStatus(serviceable, defective)));
            _details.Add(new CarDetail("Аккамулятор", 40000, UserUtils.GenerateRandomStatus(serviceable, defective)));
            _details.Add(new CarDetail("Подвеска", 50000, UserUtils.GenerateRandomStatus(serviceable, defective)));
        }

        public void ShowTechicalState()
        {
            foreach (CarDetail detail in _details)
            {
                detail.ShowInfo();
            }
        }

        public bool ReplaceDetail(CarDetail detail)
        {
            for (int i = 0; i < _details.Count; i++)
            {
                if (_details[i].Name == detail.Name && _details[i].Status != detail.Status)
                {
                    _details[i].ChangeStatus();
                    return true;
                }
            }

            return false;
        }

        public bool IsFullServisiable()
        {
            for (int i = 0; i < _details.Count; i++)
            {
                if (_details[i].Status == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
