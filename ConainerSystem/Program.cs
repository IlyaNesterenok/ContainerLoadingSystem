using System;
using System.Collections.Generic;

// Interface for notifying hazardous events
public interface IHazardNotifier
{
    void NotifyHazard(string containerNumber);
}

// Exception for overfilling containers
public class OverfillException : Exception
{
    public OverfillException(string message) : base(message)
    {
    }
}

// Container class
public abstract class Container
{
    public string SerialNumber { get; }
    public double MassOfCargo { get; protected set; }
    public double Height { get; }
    public double TareWeight { get; }
    public double Depth { get; }
    public double MaxPayload { get; }

    public Container(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maxPayload)
    {
        SerialNumber = serialNumber;
        MassOfCargo = massOfCargo;
        Height = height;
        TareWeight = tareWeight;
        Depth = depth;
        MaxPayload = maxPayload;
    }

    public abstract void LoadCargo(double cargoMass);

    public abstract void EmptyCargo();
}

// Liquid Container class
public class LiquidContainer : Container, IHazardNotifier
{
    public double Pressure { get; }
    public override void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxPayload)
        {
            throw new OverfillException("Cargo mass exceeds maximum payload.");
        }

        if (cargoMass > 0.5 * MaxPayload)
        {
            throw new OverfillException("Dangerous operation: Liquid container cannot be filled beyond 50% of its capacity.");
        }

        MassOfCargo = cargoMass;
    }

    public override void EmptyCargo()
    {
        MassOfCargo = 0.05 * MaxPayload;
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation detected in liquid container {containerNumber}");
    }

    public LiquidContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maxPayload, double pressure) 
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maxPayload)
    {
        Pressure = pressure;
    }
}

// Gas Container class
public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; }
    public override void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxPayload)
        {
            throw new OverfillException("Cargo mass exceeds maximum payload.");
        }

        if (cargoMass > 0.5 * MaxPayload)
        {
            throw new OverfillException("Dangerous operation: Gas container cannot be filled beyond 50% of its capacity.");
        }

        MassOfCargo = cargoMass;
    }

    public override void EmptyCargo()
    {
        MassOfCargo = 0.05 * MaxPayload;
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation detected in gas container {containerNumber}");
    }

    public GasContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maxPayload, double pressure)
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maxPayload)
    {
        Pressure = pressure;
    }
}

// Refrigerated Container class
public class RefrigeratedContainer : Container
{
    public string ProductType { get; }
    public double Temperature { get; }

    public override void LoadCargo(double cargoMass)
    {
        if (cargoMass > MaxPayload)
        {
            throw new OverfillException("Cargo mass exceeds maximum payload.");
        }

        if (cargoMass > 0.9 * MaxPayload)
        {
            throw new OverfillException("Dangerous operation: Refrigerated container cannot be filled beyond 90% of its capacity.");
        }

        MassOfCargo = cargoMass;
    }

    public override void EmptyCargo()
    {
        MassOfCargo = 0;
    }

    public RefrigeratedContainer(string serialNumber, double massOfCargo, double height, double tareWeight, double depth, double maxPayload, string productType, double temperature)
        : base(serialNumber, massOfCargo, height, tareWeight, depth, maxPayload)
    {
        ProductType = productType;
        Temperature = temperature;
    }
}

// Container ship class
public class ContainerShip
{
    public string Name { get; }
    public double Speed { get; }
    public int MaxContainerNum { get; }
    public double MaxWeight { get; }
    private List<Container> containers;

    public ContainerShip(string name, double speed, int maxContainerNum, double maxWeight)
    {
        Name = name;
        Speed = speed;
        MaxContainerNum = maxContainerNum;
        MaxWeight = maxWeight;
        containers = new List<Container>();
    }

    public void LoadContainer(Container container)
    {
        if (containers.Count < MaxContainerNum && CalculateTotalWeight() + container.MassOfCargo <= MaxWeight)
        {
            containers.Add(container);
        }
        else
        {
            throw new Exception("Cannot load container onto the ship: Ship's capacity exceeded.");
        }
    }

    public void UnloadContainer(Container container)
    {
        if (containers.Contains(container))
        {
            containers.Remove(container);
        }
    }

    public void ReplaceContainer(Container oldContainer, Container newContainer)
    {
        if (containers.Contains(oldContainer))
        {
            containers.Remove(oldContainer);
            containers.Add(newContainer);
        }
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Container Ship: {Name}");
        Console.WriteLine($"Speed: {Speed} knots");
        Console.WriteLine($"Max Container Number: {MaxContainerNum}");
        Console.WriteLine($"Max Weight: {MaxWeight} tons");

        Console.WriteLine("Containers on board:");
        foreach (var container in containers)
        {
            Console.WriteLine($" - {container.SerialNumber}");
        }
    }

    private double CalculateTotalWeight()
    {
        double totalWeight = 0;
        foreach (var container in containers)
        {
            totalWeight += container.MassOfCargo + container.TareWeight;
        }
        return totalWeight;
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Creating containers
            LiquidContainer liquidContainer = new LiquidContainer("KON-L-1", 0, 10, 5, 10, 100, 1.5);
            GasContainer gasContainer = new GasContainer("KON-G-1", 0, 10, 5, 10, 100, 2.0);
            RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer("KON-C-1", 0, 10, 5, 10, 100, "Bananas", 5.0);

            // Loading cargo into containers
            liquidContainer.LoadCargo(50);
            gasContainer.LoadCargo(40);
            refrigeratedContainer.LoadCargo(90);

            // Creating a container ship
            ContainerShip ship = new ContainerShip("Ship 1", 10, 3, 300);

            // Loading containers onto the ship
            ship.LoadContainer(liquidContainer);
            ship.LoadContainer(gasContainer);
            ship.LoadContainer(refrigeratedContainer);

            // Printing ship information
            ship.PrintShipInfo();

            // Unloading a container from the ship
            ship.UnloadContainer(liquidContainer);

            // Printing ship information after unloading
            ship.PrintShipInfo();
        }
        catch (OverfillException ex)
        {
            Console.WriteLine($"Overfill Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}
