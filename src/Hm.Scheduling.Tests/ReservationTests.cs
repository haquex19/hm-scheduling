using System.Net;
using FluentAssertions;
using Hm.Scheduling.Api.Controllers;
using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Services;
using Hm.Scheduling.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Hm.Scheduling.Tests;

[TestClass]
public class ReservationTests
{
    [TestMethod]
    public async Task Reservation_GivenSlotAlreadyTaken_ThrowsUnprocessableEntity()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2023, 12, 1, 0, 0, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var controller = new AvailabilitiesController(mediator);
        var providerUser = await Utilities.CreateUserAsync(scope, UserType.Provider);
        var user1 = await Utilities.CreateUserAsync(scope);
        var user2 = await Utilities.CreateUserAsync(scope);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = providerUser.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 10, 15, 0, TimeSpan.Zero)
        };

        var availabilityResponse = await controller.CreateAsync(request);
        var availabilityValue =
            (AppointmentAvailabilityModel)((CreatedAtActionResult)availabilityResponse.Result!).Value!;

        var reservationRequest = new CreateReservationRequest
        {
            Id = availabilityValue.Id,
            Time = new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero),
            UserId = user1.Id
        };

        await controller.CreateReservationAsync(reservationRequest);

        reservationRequest.UserId = user2.Id;
        var act = async () => await controller.CreateReservationAsync(reservationRequest);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.UnprocessableEntity &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "NotAvailable");
    }

    [TestMethod]
    public async Task Reservation_GivenSlotAlreadyTakenButIsNowExpired_SuccessfullyReserves()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2023, 12, 1, 0, 0, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<HmDbContext>();
        var controller = new AvailabilitiesController(mediator);
        var providerUser = await Utilities.CreateUserAsync(scope, UserType.Provider);
        var user1 = await Utilities.CreateUserAsync(scope);
        var user2 = await Utilities.CreateUserAsync(scope);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = providerUser.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 10, 15, 0, TimeSpan.Zero)
        };

        var availabilityResponse = await controller.CreateAsync(request);
        var availabilityValue =
            (AppointmentAvailabilityModel)((CreatedAtActionResult)availabilityResponse.Result!).Value!;

        var reservationRequest = new CreateReservationRequest
        {
            Id = availabilityValue.Id,
            Time = new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero),
            UserId = user1.Id
        };

        await controller.CreateReservationAsync(reservationRequest);
        await context.Reservations.ExecuteUpdateAsync(
            setters => setters.SetProperty(x => x.ExpiresOn, DateTimeOffset.UtcNow.AddMinutes(-1)));

        reservationRequest.UserId = user2.Id;
        var reservationResponse = await controller.CreateReservationAsync(reservationRequest);
        reservationResponse
            .Result.Should()
            .BeOfType<CreatedAtActionResult>();
    }

    [TestMethod]
    public async Task Reservation_GivenRequestNotMadeIn24Hours_ThrowsBadRequest()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2024, 1, 1, 8, 45, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var providerUser = await Utilities.CreateUserAsync(scope, UserType.Provider);
        var user = await Utilities.CreateUserAsync(scope);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = providerUser.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        var availabilityResponse = await controller.CreateAsync(request);
        var availabilityValue =
            (AppointmentAvailabilityModel)((CreatedAtActionResult)availabilityResponse.Result!).Value!;

        var reservationRequest = new CreateReservationRequest
        {
            Id = availabilityValue.Id,
            Time = new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero),
            UserId = user.Id
        };

        var act = async () => await controller.CreateReservationAsync(reservationRequest);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.BadRequest &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "MustRequestWithin24Hours");
    }
}
