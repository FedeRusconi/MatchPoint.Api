var builder = DistributedApplication.CreateBuilder(args);

var accessControlService = builder.AddProject<Projects.MatchPoint_AccessControlService>("matchpoint-accesscontrolservice");
builder.AddProject<Projects.MatchPoint_ClubService>("matchpoint-clubservice").WithReference(accessControlService);

builder.Build().Run();
