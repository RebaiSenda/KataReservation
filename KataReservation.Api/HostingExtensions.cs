using KataReservation.Api.Middlewares;
using KataReservation.Dal.Repositories;
using KataReservation.Domain.Interfaces.Repositories;
using KataReservation.Domain.Interfaces.Services;
using KataReservation.Domain.Services;
using Scalar.AspNetCore;

namespace KataReservation.Api;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
        builder.Services.AddAuthentication()
           .AddJwtBearer(options =>
           {
               options.Authority = "https://localhost:5001";
               options.TokenValidationParameters.ValidateAudience = false;
           });

        //builder.Services.AddAuthorization();
        //builder.Services.AddAuthorizationBuilder()
        //    .AddPolicy("KataReservationApiPolicy", policy =>
        //    {
        //        policy.RequireClaim("scope", "KataReservation.Api");
        //    });
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddScoped<IPersonService, PersonService>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ErrorLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}