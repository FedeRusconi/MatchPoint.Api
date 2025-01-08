var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MatchPoint_ClubService>("matchpoint-clubservice");

builder.AddProject<Projects.MatchPoint_AccessControlService>("matchpoint-accesscontrolservice");

builder.Build().Run();
