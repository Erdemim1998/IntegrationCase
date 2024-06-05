using Integration.Service;
using System.Threading;

namespace Integration;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        // 1 - Single Server Scenario
        var service = new ItemIntegrationService();

        Task taska = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("a"));
        });

        Task taskb = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("b"));
        });

        Task taskc = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("c"));
        });

        Thread.Sleep(500);

        taska = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("a"));
        });

        taskb = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("b"));
        });

        taskc = new Task(() =>
        {
            ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("c"));
        });

        await Task.Run(() => { taska.Start(); taskb.Start(); taskc.Start(); });
        Thread.Sleep(5000);

        Console.WriteLine("1 - Single Server Scenario");
        Console.WriteLine("Everything recorded:");

        service.GetAllItems().OrderBy(i => i.Id).ToList().ForEach(Console.WriteLine);

        Console.ReadLine();

        // 2 - Distributed System Scenario
        Console.WriteLine("2 - Distributed System Scenario");
        ItemController controller = new ItemController(new Backend.ItemIntegrationContext());
        Console.WriteLine(await controller.CreateItem("a"));
        Console.WriteLine(await controller.CreateItem("b"));
        Console.WriteLine(await controller.CreateItem("c"));

        Console.WriteLine(await controller.CreateItem("a"));
        Console.WriteLine(await controller.CreateItem("b"));
        Console.WriteLine(await controller.CreateItem("c"));

        Console.WriteLine(await controller.CreateItem("1"));
        Console.WriteLine(await controller.CreateItem("2"));
        Console.WriteLine(await controller.CreateItem("3"));
        Console.ReadKey();
    }
}