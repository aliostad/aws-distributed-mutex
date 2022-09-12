namespace Be.Vlaanderen.Basisregisters.Containers.Testing
{
    using System;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Services;

    public static class Container
    {
        public static object Run(string image, string waitForPort, string waitForProto, TimeSpan? timeout = null) => new Builder()
            .UseContainer()
            .UseImage(image)
            .ExposePort(int.Parse(waitForPort))
            .WaitForPort($"{waitForPort}:{waitForProto}", Convert.ToInt64(timeout?.TotalMilliseconds ?? 30_000))
            .Build()
            .Start();

        // docker-compose --file <yaml file> up
        public static ICompositeService Compose(string fileName, string waitForService, int waitForPort, string waitForProto, TimeSpan? timeout = null) => new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(fileName)
            .RemoveOrphans()
            .WaitForPort(waitForService, $"{waitForPort}:{waitForProto}", Convert.ToInt64(timeout?.TotalMilliseconds ?? 30_000))
            .Build()
            .Start();
    }
}
