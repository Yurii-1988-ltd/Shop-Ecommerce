using Ecommerce.Shippings.Modules.Application.Features.GetShipmentTrackingEvents;
using MediatR;

namespace Ecommerce.Shippings.Modules.Presentation.Shippings;

internal sealed class GetShipmentTrackingEventsEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/shippings/{id:guid}/tracking-events", async (
        Guid id,
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            var query = new GetShipmentTrackingEventsQuery(id);

            var result = await sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            var response = result.Value.Select(e => new ShipmentTrackingEventResponse(
     e.Id,
     e.ShipmentId, // Теперь проецируем и идентификатор отправления
     e.Status,
     e.Description,
     e.OccurredAtUtc // Используем обновленное имя свойства
 )).ToList();

            return Results.Ok(response);
        })
    .WithName("GetShipmentTrackingEvents")
    .WithTags("Shippings") // Или Tags.Shippings
    .Produces<IReadOnlyCollection<ShipmentTrackingEventResponse>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .RequireAuthorization();
    }
}