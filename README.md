# Projecto

[![Build status](https://ci.appveyor.com/api/projects/status/ikjbo1a07y33jt0o/branch/master?svg=true)](https://ci.appveyor.com/project/huysentruitw/projecto/branch/master)

## Overview

Managed .NET (C#) library for handling CQRS/ES projections while maintaining the event sequence order.

## Get it on [NuGet](https://www.nuget.org/packages/Projecto/)

    PM> Install-Package Projecto

## Concepts / usage

### MessageEnvelope

A message envelope is a user-defined object (must derive from MessageEnvelope) that wraps a message before it is passed to `projector.Project()` which in turn passes the envelope to the `When<>()` handler inside the projection.

It's a convenient way to pass out-of-band data to the handler (f.e. the originating command id, or creation date of the event/message).

### Projection

Inherit from `Projection<TConnection, TMessageEnvelope>` to define a projection, where `TConnection` is the connection type used by the projection and `TMessageEnvelope` is the user-defined message envelope object (see previous topic).

```csharp
public class ExampleProjection : Projection<ApplicationDbContext, MyMessageEnvelope>
{
    public ExampleProjection()
    {
        When<CreatedUserProfileEvent>(async (dataContext, envelope, @event) =>
        {
            dataContext.UserProfiles.Add(...);

            // Data can be saved directly or during collection disposal for batching queries (see `ProjectScope`)
            await dataContext.SaveChangesAsync();
        });
    }
}
```

The above projection is a simple example that does not override the `FetchNextSequenceNumber` and `IncrementNextSequenceNumber` method. If you want to persist the next event sequence number, you'll have to override those methods and fetch/store the sequence number from/to your store.

### Projector

The projector reflects the sequence number of the most out-dated projection.

Use the projector to project one or more events/messages to all registered projections.

The projector ensures that it only handles events/messages in the correct order, starting with event/message with a sequence number equal to the number as returned from `GetNextSequenceNumber()`.

The number returned by the `GetNextSequenceNumber()` method of the projector can be used by the application to request a resend of missing events/messages during startup.

The `ProjectorBuilder` class is used to build a projector instance.

### ProjectorBuilder

```csharp
var disposalCallbacks = new ExampleCollectionDisposalCallbacks();

var projector = new ProjectorBuilder()
    .Register(new ExampleProjection())
    .SetConnectionLifetimeScopeFactory(new ExampleConnectionLifetimeScopeFactory())
    .Build();
```

### ConnectionLifetimeScope

A connection lifetime scope is a scope that is created and disposed inside the call to `projector.Project`. The projector uses the `ConnectionLifetimeScopeFactory` to create a disposable `ConnectionLifetimeScope`. The connection lifetime scope is responsible for resolving connections as needed by one or more projections and has `ScopeEnded` and `ConnectionResolved` events to have more control over the lifetime of the used connection(s).

```csharp
public class ExampleConnectionLifetimeScopeFactory : IConnectionLifetimeScopeFactory
{
    public IConnectionLifetimeScope BeginLifetimeScope()
    {
        return new ExampleConnectionLifetimeScope();
    }
}

public class ExampleConnectionLifetimeScope : IConnectionLifetimeScope
{
    public ExampleProjectScope()
    {
    }

    public void Dispose()
    {
        // Called when the project scope gets disposed
    }

    public object ResolveConnection(Type connectionType)
    {
        if (connectionType == typeof(ApplicationDbContext)) return new ApplicationDbContext();
        throw new Exception($"Can't resolve unknown connection type {connectionType.Name}");
    }
}
```

### Autofac integration

An additional package exists for integration with Autofac. Get it on [NuGet](https://www.nuget.org/packages/Projecto.Autofac/):

    PM> Install-Package Projecto.Autofac
    
Now the configuration of the projector can be simplified to this:

```csharp
autofacContainer.RegisterType<ExampleProjection>().SingleInstance();

autofacContainer.Register(ctx => new ProjectorBuilder<ProjectionMessageEnvelope>()
    .RegisterProjectionsFromAutofac(ctx)
    .UseAutofacConnectionLifetimeScopeFactory(ctx)
    .Build()
).AsSelf().SingleInstance();
```
