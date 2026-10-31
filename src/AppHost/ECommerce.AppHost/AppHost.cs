var builder = DistributedApplication.CreateBuilder(args);

// 1. SQL Server
var sql = builder.AddSqlServer("sql")
    .WithDataVolume();

var database = sql.AddDatabase("Ecommerce");

// 2. RabbitMQ
var rabbitmqUsername = builder.AddParameter(
    "rabbitmq-username",
    secret: true);

var rabbitmqPassword = builder.AddParameter(
    "rabbitmq-password",
    secret: true);

var messaging = builder.AddRabbitMQ(
        "messaging",
        rabbitmqUsername,
        rabbitmqPassword)
    .WithManagementPlugin()
    .WithDataVolume()
    .WithEnvironment(
        "RABBITMQ_SERVER_ADDITIONAL_ERL_ARGS",
        "-rabbit loopback_users []");

// 3. PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithEnvironment(
        "POSTGRES_PASSWORD",
        "postgres")
    .WithDataVolume()
    .WithEndpoint(
        port: 5432,
        targetPort: 5432,
        name: "tcp");

var inventory = postgres.AddDatabase("inventories");
var notification = postgres.AddDatabase("notifications");
var localization = postgres.AddDatabase("localizations");
var currency = postgres.AddDatabase("currencies");
var shipping = postgres.AddDatabase("shippings");

// 4. MongoDB
var mongo = builder.AddMongoDB("mongo")
    .WithDataVolume();

var catalog = mongo.AddDatabase("Catalog");
var cart = mongo.AddDatabase("Cart");
var order = mongo.AddDatabase("Order");
var coupon = mongo.AddDatabase("Coupon");

// 5. API
var api = builder.AddProject<Projects.Ecommerce_API>(
        "ecommerce-api")
    .WithReference(database)
    .WithReference(catalog)
    .WithReference(cart)
    .WithReference(order)
    .WithReference(inventory)
    .WithReference(notification)
    .WithReference(coupon)
    .WithReference(messaging)
    .WithReference(localization)
    .WithReference(shipping)
    .WithReference(currency)
    .WaitFor(messaging);

// 6. Admin
builder.AddProject<Projects.Ecommerce_Admin>(
        "ecommerce-admin")
    .WithReference(api)
    .WaitFor(api);

// 7. Storefront
builder.AddProject<Projects.Ecommerce_Storefront>(
        "ecommerce-storefront")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();