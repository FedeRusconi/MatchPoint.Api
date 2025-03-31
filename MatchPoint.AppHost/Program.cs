var builder = DistributedApplication.CreateBuilder(args);

var eventBusBroker = builder.AddProject<Projects.MockEventBus_Broker>("mockeventbus-broker");
var accessControlService = builder.AddProject<Projects.MatchPoint_AccessControlService>("matchpoint-accesscontrolservice")
    .WithReference(eventBusBroker);
accessControlService.WithReference(accessControlService);
builder.AddProject<Projects.MatchPoint_ClubService>("matchpoint-clubservice")
    .WithReference(accessControlService);

builder.Build().Run();
