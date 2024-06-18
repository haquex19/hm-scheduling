using System.Net;
using FluentAssertions;
using Hm.Scheduling.Api.Controllers;
using Hm.Scheduling.Core.Commands;
using Hm.Scheduling.Core.Entities;
using Hm.Scheduling.Core.Exceptions;
using Hm.Scheduling.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hm.Scheduling.Tests;

[TestClass]
public class CreateAvailabilitiesTests
{
    [TestMethod]
    public async Task Create_GivenTimeRange_CreatesSuccessfully()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        var response = await controller.CreateAsync(request);
        response
            .Result.Should()
            .BeOfType<CreatedAtActionResult>();

        var value = (AppointmentAvailabilityModel)((CreatedAtActionResult)response.Result!).Value!;
        value
            .StartTime.Should()
            .Be(request.StartTime);

        value
            .EndTime.Should()
            .Be(request.EndTime);
    }

    [TestMethod]
    public async Task Create_GivenOverlappingTime_ThatEncapsulatesExisting_ThrowsUnprocessableEntity()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 8, 30, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 18, 30, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.UnprocessableEntity &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "OverlappingAvailabilitiesFound");
    }

    [TestMethod]
    public async Task Create_GivenOverlappingTime_ThatIsContainedWithinExisting_ThrowsUnprocessableEntity()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 11, 30, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 14, 30, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.UnprocessableEntity &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "OverlappingAvailabilitiesFound");
    }

    [TestMethod]
    public async Task Create_GivenOverlappingTime_ThatHasMatchingStartTime_ThrowsUnprocessableEntity()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 18, 0, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.UnprocessableEntity &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "OverlappingAvailabilitiesFound");
    }

    [TestMethod]
    public async Task Create_GivenOverlappingTime_ThatHasMatchingEndTime_ThrowsUnprocessableEntity()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.UnprocessableEntity &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "OverlappingAvailabilitiesFound");
    }

    [TestMethod]
    public async Task Create_GivenMultipleTimes_WithoutOverlapping_CreatesSuccessfully()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        await controller.CreateAsync(request);

        request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 2, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 2, 17, 30, 0, TimeSpan.Zero)
        };

        var response = await controller.CreateAsync(request);
        response
            .Result.Should()
            .BeOfType<CreatedAtActionResult>();
    }

    [TestMethod]
    public async Task Create_GivenTimeIsNotMultipleOf15_ThrowsBadRequestException()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 25, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 17, 30, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.BadRequest &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "MultipleOf15");
    }

    [TestMethod]
    public async Task Create_GivenInvalidDateRange_ThrowsBadRequestException()
    {
        await using var provider = await Utilities.GetDefaultServiceProviderAsync();
        await using var scope = provider.CreateAsyncScope();

        var controller = new AvailabilitiesController(scope.ServiceProvider.GetRequiredService<IMediator>());
        var user = await Utilities.CreateUserAsync(scope, UserType.Provider);

        var request = new CreateAppointmentAvailabilityRequest
        {
            ProviderId = user.Id,
            StartTime = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero),
            EndTime = new DateTimeOffset(2024, 1, 1, 8, 30, 0, TimeSpan.Zero)
        };

        var act = async () => await controller.CreateAsync(request);
        await act
            .Should()
            .ThrowAsync<RequestException>()
            .Where(
                x => x.StatusCode == HttpStatusCode.BadRequest &&
                     x.RequestErrors.Count == 1 &&
                     x.RequestErrors[0].Code == "RangeInvalid");
    }
}
