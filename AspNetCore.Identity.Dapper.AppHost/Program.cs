var builder = DistributedApplication.CreateBuilder(args);

builder.AddPostgres("postgres");

builder.Build().Run();