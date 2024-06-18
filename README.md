# Project Info
- Running on .NET 8.
- Using nullable reference types.
- Using newer C# features such as primary constructors and records.

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

# Authentication/Authorization
The `develop` branch does not have any authentication/authorization programmed into it. Please branch over to the `develop-oauth`
branch to get an example of how this might work with authentication/authorization.

- The `develop-oauth` branch uses [identity server](https://duendesoftware.com/products/identityserver) as the identity
provider.
- This branch also demonstrates how this code challenge that doesn't have authentication/authorization can be refactored
to have it.