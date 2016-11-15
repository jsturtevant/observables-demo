using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OberservableDemo
{
    public class Program
    {
        private const int delay = 3000;

        public static void Main(string[] args)
        {
            Console.WriteLine("###########################");
            Console.WriteLine("Welcome to observables Demo");
            Console.WriteLine();

            var keeprunning = true;
            while (keeprunning)
            {
                Console.WriteLine();
                Console.WriteLine("###########################");
                Console.WriteLine("Choose Demo");
                Console.WriteLine("a - akavache");
                Console.WriteLine("l - long list");
                Console.WriteLine("x - exit");
                var value = Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();

                switch (value.Key.ToString().ToLower())
                {
                    case "a":
                        AkavacheDemo();
                        break;
                    case "l":
                        LongListDemo();
                        break;
                    case "x":
                        keeprunning = false;
                        break;
                    default:
                        Console.WriteLine("invalid choose another");
                        break;
                }
            }
        }

        private static async void LongListDemo()
        {
            var subject = new Subject<List<Dog>>();

            Console.WriteLine("Subscribe to list");
            ProcessResults(subject);

            Console.WriteLine("First update");
            var dos1 = await Dogs1();
            subject.OnNext(dos1);

            Console.WriteLine("Second update");
            var dos2 = await Dogs2();
            subject.OnNext(dos2);

            Console.WriteLine("Send Exception");
            Thread.Sleep(2000);
            subject.OnError(new Exception());

            Console.WriteLine("Complete Transaction");
            Thread.Sleep(2000);
            subject.OnCompleted();

            Console.WriteLine("Demo done.  Press key to continue");
            Console.ReadLine();
        }

        private static void AkavacheDemo()
        {
            var cache = BlobCache.InMemory;

            Console.WriteLine("###########################");
            Console.WriteLine("call with no cache:");
            var result = cache.GetAndFetchLatest<List<Dog>>("dogs", Dogs1,
                   null, DateTimeOffset.Now.AddHours(3));

          
            ProcessResults(result);
            Console.WriteLine("Press key to make another call");
            Console.ReadLine();

            Console.WriteLine("###########################");
            Console.WriteLine("call with cache:");
            var result2 = cache.GetAndFetchLatest<List<Dog>>("dogs", Dogs2,
                  null, DateTimeOffset.Now.AddHours(3));

            ProcessResults(result2);
            Console.WriteLine("Press key to make another call");
            Console.ReadLine();


            Console.WriteLine("###########################");
            Console.WriteLine("call with exception:");
            var result3 = cache.GetAndFetchLatest<List<Dog>>("dogs", DogsThrow,
                  null, DateTimeOffset.Now.AddHours(3));

            ProcessResults(result3);
            Console.WriteLine("Press key to make another call");
            Console.ReadLine();

            Console.WriteLine("Demo done. Press key to continue");
            Console.ReadLine();
        }

        private static void ProcessResults(IObservable<List<Dog>> result)
        {

            result.Subscribe(onNext: dogs =>
            {
                Console.WriteLine("Dogs received: ");
                foreach (var dog in dogs)
                {
                    Console.WriteLine($"\t {dog.Name}");
                }
            }, onError: exception =>
            {
                Console.WriteLine($"We were unable to Connect.  Will try again later");
            }, onCompleted: () =>
            {
                Console.WriteLine("Completed.");
            });
        }

        private static Task<List<Dog>> Dogs1()
        {
            //this would be a call to api
       
            Thread.Sleep(3000);

            return Task.FromResult(new List<Dog>()
            {
                new Dog() { Name = "Dog1" },
                new Dog() { Name = "Dog2" },
                new Dog() { Name = "Dog3" },
            });
        }

        private static Task<List<Dog>> Dogs2()
        {
            Thread.Sleep(3000);
            //this would be a call to api
            return Task.FromResult(new List<Dog>()
            {
                new Dog() { Name = "Dog4" },
                new Dog() { Name = "Dog5" },
                new Dog() { Name = "Dog6" },
            });
        }

        private static Task<List<Dog>> DogsThrow()
        {
            Thread.Sleep(delay);
            throw new Exception("error connecting");
        }
    }

    public class Dog
    {
        public string Name { get; set; }
    }
}
