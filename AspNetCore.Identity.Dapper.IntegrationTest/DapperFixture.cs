using Aspire.Hosting.Testing;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Identity.Dapper.IntegrationTest;

[SetUpFixture]
public class DapperFixture
{
    private DistributedApplication _host;
    
    private string? _postgresConnectionString;
    
    [OneTimeSetUp]  
    public async Task GlobalSetup()
    {
        var appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspNetCore_Identity_Dapper_AppHost>();
        _host = await appHost.BuildAsync();
        await _host.StartAsync();
        _postgresConnectionString = await _host.GetConnectionStringAsync("postgres");
    }
    
    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await _host.StopAsync();
    }
}