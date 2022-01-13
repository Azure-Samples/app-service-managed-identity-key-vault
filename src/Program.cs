// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;
using System.Text;

// check PORT env var
string? p = Environment.GetEnvironmentVariable("PORT");

if (string.IsNullOrWhiteSpace(p))
{
    p = "8080";
    Environment.SetEnvironmentVariable("PORT", p);
}

// pass --urls to builder
if (args == null)
{
    args = new string[] { "--urls", $"http://*:{p}/" };
}
else
{
    // append if not passed
    if (!args.Contains("--urls"))
    {
        args = args.Append("--urls").Append($"http://*:{p}/").ToArray();
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// handle default doc
app.Use(async (context, next) =>
{
    string path = context.Request.Path.HasValue ? context.Request.Path.Value.ToLowerInvariant() : string.Empty;

    // matches / /index.* /default.*
    if (path == "/" || path.StartsWith("/index.") || path.StartsWith("/default."))
    {
        // return the message
        context.Response.ContentType = "text/plain";
        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Under construction ...")).ConfigureAwait(false);
    }
    else
    {
        // not a match, so call next handler
        await next().ConfigureAwait(false);
    }
});

//app.UseSwagger();
//app.UseSwaggerUI();

app.MapControllers();

app.Run();
