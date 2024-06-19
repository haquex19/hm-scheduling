# Project Info
- Running on .NET 8.
- Using nullable reference types.
- Using newer C# features such as primary constructors and records.
- The `Core` library in the project contains domain level objects and functionality. The `Infrastructure` project contains
code primarily for external services such as postgresql.

# External Service Dependencies
- The project relies on a postgresql server.

# Configuration
- The connection string to the postgresql database instance can be provided via the `ApplicationSettings:ConnectionString`
configuration value in the `appsettings.json` file in the API project.
- A default connection string has been included that points to localhost with a username of `postgres` and password of `Testing123`.
- You can set `ApplicationSettings:FirstTimeDbSetup` to `true`. This will run the entity framework migrations on the database
when you run the API.

# External Library Dependencies
- `Mediator` - I love using the CQRS pattern. I primarily use this library to handle that.
- `AutoMapper` - Using this library to handle mapping between domain objects and HTTP response models.
- `FluentValidation` - Using this library for some quick and easy HTTP request validation.
- `FluentAssertions` - Using this library on unit tests. I feel it makes assertions much easier to read.

# Testing
A test project has been added to the code challenge. This project tests the business requirements and does a "semi" integration
test. What I mean by that is that I am not testing specific individual methods. Instead, I am testing the pipeline as a
whole. The request that goes in a controller should respond with the expected output.

This ensures that all services, from the controller request down to the database operations, are working as expected.

# Impactful Assumptions Made
- Appointments are always in 15 minute intervals. A slot is only available for a multiple of 15. Thus a user will not see
nor cannot reserve a time such as 2:38 PM for example.
- Since it's a code challenge, proper authentication/authorization has not been implemented. Refer to the document further
down to see how I'd do it differently in a real-world scenario.

# Race Condition Handling
- Race conditions were not handled in this code challenge. The code challenge does do a pre-read request to ensure reservations
are available to be made, but it is entirely possible two users made a call to reserve and the read operation occurred for
both users without either having written an entry to the database.
  - We can provide a locking mechanism to prevent this issue.
    - I have used the [RedLock](https://redis.io/docs/latest/develop/use/patterns/distributed-locks/) algorithm to handle
    these situations in the past.
    - It is also possible to use [an application lock](https://www.postgresql.org/docs/current/explicit-locking.html) provided
    by postgres.
  - Both of the methods above achieve distributed locking. Essentially we would lock on the appointment availability id
  and the time of the reservation request. This way we can ensure that two users reserving on the same appointment availability
  and reservation time do not run into a race condition.

# What to do differently
- There are some hard-coded values like an appointment slot length (15 minutes) and the number of hours the user must reserve
ahead of time by. These should instead be stored as configuration/environment variables. This allows us to change these
values without a rebuild of the application.
- I would have implemented a paging mechanism for listing slots. Right now all available slots are returned, this could
post a problem for providers that have scheduled availability for the year.
- In the exception handler, if there is an unexpected exception, I am returning a vague description indicating that an error
has occurred. In a real-world scenario, I would have built an exception tracking pipeline. For example, set up sentry and
configure sentry to send alerts to a slack channel (or email or some other medium) that can be monitored. Additionally, 
configure sentry to create jira tickets (or github issues) automatically for high frequence errors that need to be triaged.

# Authentication/Authorization
The `develop` branch does not have any authentication/authorization programmed into it. Unfortunately, I have ran out of
time (almost 3 hours in) and could not add that in.

However, to add authentication/authorization I would do the following:

- Select an oauth provider. EG: Microsoft Azure, Duende Identity Server, Okta, Auth0, etc.
- Configure the identity provider to use an authorization-code flow to log the user in. This will allow a frontend application
to obtain an access token.
  - Create an API resource that declares a scope.
  - Create a client resource (represents the frontend application such as a mobile app). The client resource will not have
  a secret if it is a public client (such as a mobile app or web app).
  - Allow the client resource access to the api scope. This will allow the client resource access to the API.
- The access token will contain a role indicating whether the user is a provider or a client.
- We can use ASP.NET Identity policies to ensure provider endpoints (such as creating provider available times) are only
available to providers.