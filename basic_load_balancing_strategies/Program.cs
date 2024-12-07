
using System;
using System.Collections.Generic;
using System.Linq;



namespace basic_load_balancing_strategies
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Load Balancer Demo:");

            // Simulated servers
            List<Server> servers = new List<Server>
        {
            new Server { Id = 1, Capacity = 100, CurrentLoad = 20, ResponseTime = 50 },
            new Server { Id = 2, Capacity = 150, CurrentLoad = 50, ResponseTime = 30 },
            new Server { Id = 3, Capacity = 200, CurrentLoad = 70, ResponseTime = 20 }
        };

            // Select a load balancing strategy
            Console.WriteLine("Choose a strategy (1-5):");
            Console.WriteLine("1. Round Robin");
            Console.WriteLine("2. Least Connection Method");
            Console.WriteLine("3. Weighted Response Time");
            Console.WriteLine("4. Resource-Based Distribution");
            Console.WriteLine("5. Source IP Hash");

            int choice = int.Parse(Console.ReadLine());

            ILoadBalancer loadBalancer = choice switch
            {
                1 => new RoundRobinLoadBalancer(servers),
                2 => new LeastConnectionLoadBalancer(servers),
                3 => new WeightedResponseTimeLoadBalancer(servers),
                4 => new ResourceBasedLoadBalancer(servers),
                5 => new SourceIpHashLoadBalancer(servers),
                _ => throw new Exception("Invalid choice")
            };

            // Simulate requests
            for (int i = 0; i < 10; i++)
            {
                string clientIp = "192.168.1." + (i + 1); // Simulated client IPs
                Server selectedServer = loadBalancer.GetServer(clientIp);
                Console.WriteLine($"Client {clientIp} directed to Server {selectedServer.Id}");
            }
        }
    }


    // Server class to simulate server properties
    class Server
    {
        public int Id { get; set; }
        public int Capacity { get; set; } // Total capacity
        public int CurrentLoad { get; set; } // Current usage
        public int ResponseTime { get; set; } // Response time in ms
    }

    // Interface for load balancer strategies
    interface ILoadBalancer
    {
        Server GetServer(string clientIp);
    }

    // 1. Round Robin Load Balancer
    class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly List<Server> _servers;
        private int _currentIndex = -1;

        public RoundRobinLoadBalancer(List<Server> servers) => _servers = servers;

        public Server GetServer(string clientIp)
        {
            _currentIndex = (_currentIndex + 1) % _servers.Count;
            return _servers[_currentIndex];
        }
    }

    // 2. Least Connection Load Balancer
    class LeastConnectionLoadBalancer : ILoadBalancer
    {
        private readonly List<Server> _servers;

        public LeastConnectionLoadBalancer(List<Server> servers) => _servers = servers;

        public Server GetServer(string clientIp)
        {
            return _servers.OrderBy(s => s.CurrentLoad).First();
        }
    }

    // 3. Weighted Response Time Load Balancer
    class WeightedResponseTimeLoadBalancer : ILoadBalancer
    {
        private readonly List<Server> _servers;

        public WeightedResponseTimeLoadBalancer(List<Server> servers) => _servers = servers;

        public Server GetServer(string clientIp)
        {
            return _servers.OrderBy(s => s.ResponseTime).First();
        }
    }

    // 4. Resource-Based Load Balancer
    class ResourceBasedLoadBalancer : ILoadBalancer
    {
        private readonly List<Server> _servers;

        public ResourceBasedLoadBalancer(List<Server> servers) => _servers = servers;

        public Server GetServer(string clientIp)
        {
            return _servers.OrderBy(s => s.CurrentLoad * 1.0 / s.Capacity).First();
        }
    }

    // 5. Source IP Hash Load Balancer
    class SourceIpHashLoadBalancer : ILoadBalancer
    {
        private readonly List<Server> _servers;

        public SourceIpHashLoadBalancer(List<Server> servers) => _servers = servers;

        public Server GetServer(string clientIp)
        {
            int hash = clientIp.GetHashCode();
            int index = Math.Abs(hash % _servers.Count);
            return _servers[index];
        }
    }
}
