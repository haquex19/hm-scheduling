using FluentAssertions;
using Hm.Scheduling.Api.Controllers;
using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Models;
using Hm.Scheduling.Core.Queries;
using Hm.Scheduling.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Hm.Scheduling.Tests;

[TestClass]
public class ListAvailableSlotsTests
{
    [TestMethod]
    public async Task ListAvailableSlots_GivenAvailabilityRange_ListsAllSlotsCorrectlyAsync()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2023, 12, 1, 0, 0, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 10, 15, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        var response = await controller.SearchSlotsAsync(new SearchSlotsRequest());
        response.Value!
            .Should()
            .HaveCount(5);

        response.Value![0]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero));

        response.Value![1]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 15, 0, TimeSpan.Zero));

        response.Value![2]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 30, 0, TimeSpan.Zero));

        response.Value![3]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero));

        response.Value![4]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero));
    }

    [TestMethod]
    public async Task ListAvailableSlots_GivenReservedSlot_ListsAllSlotsCorrectlyAsync()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2023, 12, 1, 0, 0, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var availabilitiesController =
            new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var providerUser = await Utilities.CreateUserAsync(scope, UserType.Provider);
        var clientUser = await Utilities.CreateUserAsync(scope);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = providerUser.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 10, 15, 0, TimeSpan.Zero)
        };

        var availabilityResponse = await availabilitiesController.CreateAsync(request);
        var availabilityValue =
            (AppointmentAvailabilityModel)((CreatedAtActionResult)availabilityResponse.Result!).Value!;

        var reservationRequest = new CreateReservationRequest
        {
            Id = availabilityValue.Id,
            Time = new DateTimeOffset(2024, 1, 1, 9, 30, 0, TimeSpan.Zero),
            UserId = clientUser.Id
        };

        await availabilitiesController.CreateReservationAsync(reservationRequest);

        reservationRequest = new CreateReservationRequest
        {
            Id = availabilityValue.Id,
            Time = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero),
            UserId = clientUser.Id
        };

        await availabilitiesController.CreateReservationAsync(reservationRequest);

        var response = await availabilitiesController.SearchSlotsAsync(new SearchSlotsRequest());
        response.Value!
            .Should()
            .HaveCount(3);

        response.Value![0]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero));

        response.Value![1]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 15, 0, TimeSpan.Zero));

        response.Value![2]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero));
    }

    [TestMethod]
    public async Task ListAvailableSlots_GivenTimeHasPassed_ListsAllSlotsCorrectlyAsync()
    {
        var mock = new Mock<IHmClock>();
        mock
            .Setup(x => x.UtcNowOffset())
            .Returns(new DateTimeOffset(2024, 1, 1, 9, 25, 0, TimeSpan.Zero));

        await using var provider = await Utilities.GetDefaultServiceProviderAsync(mock.Object);
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 10, 15, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        var response = await controller.SearchSlotsAsync(new SearchSlotsRequest());
        response.Value!
            .Should()
            .HaveCount(3);

        response.Value![0]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 30, 0, TimeSpan.Zero));

        response.Value![1]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 9, 45, 0, TimeSpan.Zero));

        response.Value![2]
            .StartTime.Should()
            .Be(new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero));
    }
}
