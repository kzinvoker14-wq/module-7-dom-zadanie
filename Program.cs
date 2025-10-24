using System;
using System.Collections.Generic;

interface ICommand
{
    void Execute();
    void Undo();
}

// Устройства
class Light
{
    public void On() => Console.WriteLine("[Свет] включен");
    public void Off() => Console.WriteLine("[Свет] выключен");
}

class Door
{
    public void Open() => Console.WriteLine("[Дверь] открыта");
    public void Close() => Console.WriteLine("[Дверь] закрыта");
}

class Thermostat
{
    private int temperature = 22;
    public void Increase() { temperature++; Console.WriteLine("[Термостат] Температура увеличена до " + temperature + "°C"); }
    public void Decrease() { temperature--; Console.WriteLine("[Термостат] Температура уменьшена до " + temperature + "°C"); }
}

// Конкретные команды
class LightOnCommand : ICommand
{
    private Light light;
    public LightOnCommand(Light l) { light = l; }
    public void Execute() => light.On();
    public void Undo() => light.Off();
}

class LightOffCommand : ICommand
{
    private Light light;
    public LightOffCommand(Light l) { light = l; }
    public void Execute() => light.Off();
    public void Undo() => light.On();
}

class DoorOpenCommand : ICommand
{
    private Door door;
    public DoorOpenCommand(Door d) { door = d; }
    public void Execute() => door.Open();
    public void Undo() => door.Close();
}

class DoorCloseCommand : ICommand
{
    private Door door;
    public DoorCloseCommand(Door d) { door = d; }
    public void Execute() => door.Close();
    public void Undo() => door.Open();
}

class TempUpCommand : ICommand
{
    private Thermostat t;
    public TempUpCommand(Thermostat th) { t = th; }
    public void Execute() => t.Increase();
    public void Undo() => t.Decrease();
}

// Invoker
class SmartHome
{
    private Stack<ICommand> history = new Stack<ICommand>();

    public void DoCommand(ICommand command)
    {
        command.Execute();
        history.Push(command);
    }

    public void UndoLast()
    {
        if (history.Count > 0)
        {
            var last = history.Pop();
            Console.WriteLine("Отмена последней команды...");
            last.Undo();
        }
        else
        {
            Console.WriteLine("Нечего отменять...");
        }
    }
}

abstract class Beverage
{
    public void MakeDrink()
    {
        BoilWater();
        Brew();
        PourInCup();
        if (CustomerWantsCondiments())
            AddCondiments();
        Console.WriteLine("Напиток готов!\n");
    }

    void BoilWater() => Console.WriteLine("Кипячю воду...");
    void PourInCup() => Console.WriteLine("Наливаю в кружку...");

    protected abstract void Brew();
    protected abstract void AddCondiments();

    protected virtual bool CustomerWantsCondiments() => true;
}

class Tea : Beverage
{
    protected override void Brew() => Console.WriteLine("Завариваю чайный пакетик...");
    protected override void AddCondiments() => Console.WriteLine("Добавляю лимончик...");
}

class Coffee : Beverage
{
    protected override void Brew() => Console.WriteLine("Готовлю кофе...");
    protected override void AddCondiments() => Console.WriteLine("Добавляю сахар и молоко...");
}

interface IMediator
{
    void SendMessage(string msg, User user);
    void Register(User user);
}

class ChatRoom : IMediator
{
    private List<User> users = new List<User>();

    public void Register(User user)
    {
        users.Add(user);
        user.SetMediator(this);
        Console.WriteLine($"[СИСТЕМА]: {user.Name} вошел в чат.");
    }

    public void SendMessage(string msg, User sender)
    {
        foreach (var u in users)
        {
            if (u != sender)
                u.Receive(msg, sender.Name);
        }
    }
}

abstract class User
{
    protected IMediator mediator;
    public string Name { get; private set; }

    public User(string name) { Name = name; }

    public void SetMediator(IMediator m) => mediator = m;

    public void Send(string msg)
    {
        Console.WriteLine($"{Name} пишет: {msg}");
        mediator.SendMessage(msg, this);
    }

    public abstract void Receive(string msg, string from);
}

class ChatUser : User
{
    public ChatUser(string name) : base(name) { }

    public override void Receive(string msg, string from)
    {
        Console.WriteLine($"{Name} получил сообщение от {from}: {msg}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine(" ПАТТЕРН КОМАНДА ");
        var light = new Light();
        var door = new Door();
        var term = new Thermostat();
        var home = new SmartHome();

        home.DoCommand(new LightOnCommand(light));
        home.DoCommand(new DoorOpenCommand(door));
        home.DoCommand(new TempUpCommand(term));
        home.UndoLast();
        home.DoCommand(new LightOffCommand(light));

        Console.WriteLine("\n ПАТТЕРН ШАБЛОННЫЙ МЕТОД ");
        var tea = new Tea();
        var coffee = new Coffee();

        Console.WriteLine(" Готовим чай ");
        tea.MakeDrink();

        Console.WriteLine(" Готовим кофе ");
        coffee.MakeDrink();

        Console.WriteLine(" ПАТТЕРН ПОСРЕДНИК ");
        var chat = new ChatRoom();

        var u1 = new ChatUser("Илья");
        var u2 = new ChatUser("Маша");
        var u3 = new ChatUser("Ваня");

        chat.Register(u1);
        chat.Register(u2);
        chat.Register(u3);

        u1.Send("Привет всем!");
        u2.Send("Здарова, как дела?");
        u3.Send("Все норм, я делаю лабу по патернам ");

        Console.WriteLine("\n Конец программы ");
    }
}
