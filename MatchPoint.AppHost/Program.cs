var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MatchPoint_ClubService>("matchpoint-clubservice");

builder.Build().Run();
