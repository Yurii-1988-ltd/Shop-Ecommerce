
global using Ecommerce.Order.Modules.Domain.Repositories;
global using Ecommerce.Domain.Constants;
global using Ecommerce.Order.Modules.Domain.ValueObjects;
global using FluentAssertions;
global using Moq;
global using Xunit;
global using Ecommerce.Application.Abstractions;
global using Ecommerce.Currency.Modules.Application.Abstractions;
global using Ecommerce.Domain.Domain;
global using Ecommerce.Domain.ValueObjects;
global using Ecommerce.Shared.Contracts.IntegrationEvent;
global using Ecommerce.Shared.Contracts.Orders;
global using MassTransit;