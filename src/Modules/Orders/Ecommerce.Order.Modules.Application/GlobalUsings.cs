
global using Ecommerce.Application.CQRS;
global using Ecommerce.Domain.Domain;
global using Ecommerce.Order.Modules.Application.Services;
global using Ecommerce.Order.Modules.Domain.Entities;
global using Ecommerce.Order.Modules.Domain.Errors;
global using Ecommerce.Order.Modules.Domain.Repositories;
global using Ecommerce.Application.Abstractions;
global using Ecommerce.Domain.Constants;
global using Ecommerce.Domain.ValueObjects;
global using Ecommerce.Currency.Modules.Application.Abstractions;
global using Ecommerce.Order.Modules.Domain.ValueObjects;
global using Ecommerce.Shared.Contracts.IntegrationEvent;
global using Ecommerce.Shared.Contracts.Orders;
global using MassTransit;