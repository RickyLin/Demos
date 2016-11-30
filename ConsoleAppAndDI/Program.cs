using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace ConsoleAppAndDI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TryOutSingleton();
            Console.WriteLine();

            TryOutTransient();
            Console.WriteLine();

            TryOutScoped();
            Console.WriteLine();
        }

        private static void TryOutSingleton()
        {
            Console.WriteLine($"RUNNING {nameof(TryOutSingleton)}");

            ServiceCollection services = new ServiceCollection(); // 准备好我们的容器
            services.AddSingleton<MyClass>(); //把MyClass注册为 Singleton 生命周期 

            ConcurrentBag<MyClass> bag = GetObjectsFromDI(services); // 调用我们准备好的方法，用若干线程从 IServiceProvider 中获取 MyClass 实例，并加入到集合

            Console.WriteLine($"All items in collection are IDENTICAL: {bag.AreIdentical()}"); // 验证集合中的所有元素是否指向内存中的同一个对象。
        }

        private static void TryOutTransient()
        {
            Console.WriteLine($"RUNNING {nameof(TryOutTransient)}");

            ServiceCollection services = new ServiceCollection(); // 准备好我们的容器
            services.AddTransient<MyClass>(); //把MyClass注册为 Transient 生命周期 

            ConcurrentBag<MyClass> bag = GetObjectsFromDI(services); // 调用我们准备好的方法，用若干线程从 IServiceProvider 中获取 MyClass 实例，并加入到集合

            Console.WriteLine($"All items in collection are DIFFERENT: {bag.AreDifferent()}"); // 验证集合中的所有元素是否各不相同
        }

        private static void TryOutScoped()
        {
            Console.WriteLine($"RUNNING {nameof(TryOutScoped)}");

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<MySingleton>();
            services.AddTransient<MyTransient>();
            services.AddScoped<MyScoped>();

            IServiceProvider sp = services.BuildServiceProvider();

            // 线程1执行
            ConcurrentBag<MySingleton> thread1SingletonBag = new ConcurrentBag<MySingleton>();
            ConcurrentBag<MyTransient> thread1TransientBag = new ConcurrentBag<MyTransient>();
            ConcurrentBag<MyScoped> thread1ScopedBag = new ConcurrentBag<MyScoped>();

            Thread thread1 = new Thread(RunPerThreadWithScopedLifetime);
            thread1.Start(new Tuple<IServiceProvider, ConcurrentBag<MySingleton>, ConcurrentBag<MyTransient>, ConcurrentBag<MyScoped>>(sp, thread1SingletonBag, thread1TransientBag, thread1ScopedBag));

            // 线程2执行
            ConcurrentBag<MySingleton> thread2SingletonBag = new ConcurrentBag<MySingleton>();
            ConcurrentBag<MyTransient> thread2TransientBag = new ConcurrentBag<MyTransient>();
            ConcurrentBag<MyScoped> thread2ScopedBag = new ConcurrentBag<MyScoped>();

            Thread thread2 = new Thread(RunPerThreadWithScopedLifetime);
            thread2.Start(new Tuple<IServiceProvider, ConcurrentBag<MySingleton>, ConcurrentBag<MyTransient>, ConcurrentBag<MyScoped>>(sp, thread2SingletonBag, thread2TransientBag, thread2ScopedBag));

            // 等待执行完毕
            thread1.Join();
            thread2.Join();

            // 验证所有 MySingleton 的实例都指向内存里同一个对象
            IEnumerable<MySingleton> singletons = thread1SingletonBag.Concat(thread2SingletonBag);
            Console.WriteLine($"{singletons.Count()} objects are IDENTICAL? {singletons.AreIdentical()}");

            // 验证所有 MyTransient 的实例都各不相同
            IEnumerable<MyTransient> transients = thread1TransientBag.Concat(thread2TransientBag);
            Console.WriteLine($"{transients.Count()} objects are DIFFERENT? {transients.AreDifferent()}");

            // 对于Scoped生命周期，每个线程集合内的实例应该指向内存里同一个对象，而2个线程集合里的实例应该是不同的。
            Console.WriteLine($"collection of thread 1 has {thread1ScopedBag.Count} objects and they are IDENTICAL: {thread1ScopedBag.AreIdentical()}");
            Console.WriteLine($"collection of thread 2 has {thread2ScopedBag.Count} objects and they are IDENTICAL: {thread2ScopedBag.AreIdentical()}");
            Console.WriteLine($"the first object from thread 1 and the first object from thread 2 are IDENTICAL: {Object.ReferenceEquals(thread1ScopedBag.First(), thread2ScopedBag.First())}");

        }

        private static void RunPerThreadWithScopedLifetime(object threadParam)
        {
            Tuple<IServiceProvider, ConcurrentBag<MySingleton>, ConcurrentBag<MyTransient>, ConcurrentBag<MyScoped>> args
                = threadParam as Tuple<IServiceProvider, ConcurrentBag<MySingleton>, ConcurrentBag<MyTransient>, ConcurrentBag<MyScoped>>;
            IServiceProvider serviceProvider = args.Item1;
            ConcurrentBag<MySingleton> singletonBag = args.Item2;
            ConcurrentBag<MyTransient> transientBag = args.Item3;
            ConcurrentBag<MyScoped> scopedBag = args.Item4;

            IServiceScope scope = serviceProvider.CreateScope(); // 创建Scope
            IServiceProvider scopedServiceProvider = scope.ServiceProvider; // 从 Scope 里得到 IServiceProvider

            for (int i = 0; i < 10; i++)
            {
                // 利用源自 IServiceScope 的 IServiceProvider 获取3种不同生命周期的实例
                singletonBag.Add(scopedServiceProvider.GetRequiredService<MySingleton>());
                transientBag.Add(scopedServiceProvider.GetRequiredService<MyTransient>());
                scopedBag.Add(scopedServiceProvider.GetRequiredService<MyScoped>());
            }
        }

        private static ConcurrentBag<MyClass> GetObjectsFromDI(ServiceCollection services)
        {
            int threadCount = 10;
            IServiceProvider sp = services.BuildServiceProvider();
            ConcurrentBag<MyClass> bag = new ConcurrentBag<MyClass>();
            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(RunPerThread);
                threads[i] = thread;
                thread.Start(new Tuple<IServiceProvider, ConcurrentBag<MyClass>>(sp, bag));
            }

            // 确保所有线程都执行完毕之后再继续
            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            return bag;
        }

        private static void RunPerThread(object threadParam)
        {
            Tuple<IServiceProvider, ConcurrentBag<MyClass>> args = threadParam as Tuple<IServiceProvider, ConcurrentBag<MyClass>>;
            IServiceProvider sp = args.Item1;
            ConcurrentBag<MyClass> bag = args.Item2;
            for (int i = 0; i < 10; i++)
            {
                bag.Add(sp.GetRequiredService<MyClass>());
            }
        }
    }

    public class MyClass { }
    public class MySingleton { }
    public class MyTransient { }
    public class MyScoped { }

    public class ReferenceEqualComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return Object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class IEnumerableExtensions
    {
        public static bool AreIdentical<T>(this IEnumerable<T> bag)
        {
            return bag.Distinct(new ReferenceEqualComparer<T>()).Count() == 1;
        }

        public static bool AreDifferent<T>(this IEnumerable<T> bag)
        {
            return bag.Distinct(new ReferenceEqualComparer<T>()).Count() == bag.Count();
        }
    }
}
