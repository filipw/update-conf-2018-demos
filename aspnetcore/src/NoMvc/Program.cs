using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NoMvc.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace NoMvc
{
    public class Program
    {
        public static async Task Main(string[] args) =>
            await WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(s =>
                {
                    s.AddEmbeddedIdentityServer();
                    s.AddRouting();
                })
                .Configure(app =>
                {
                    app.Map("/identity", id =>
                    {
                        // use embedded identity server to issue tokens
                        id.UseIdentityServer();
                    })
                    .UseAuthentication() // consume the JWT tokens in the API
                    .Use(async (c, next) => // authorize the whole API against the API policy
                    {
                        var allowed = await c.RequestServices.GetRequiredService<IAuthorizationService>().AuthorizeAsync(c.User, "API");
                        if (!allowed.Succeeded)
                        {
                            c.Response.StatusCode = 401;
                            return;
                        }

                        await next();
                    })
                    .UseRouter(r => // define all API endpoints
                    {
                        var contactRepo = new InMemoryContactRepository();

                        r.MapGet("contacts", async (request, response, routeData) =>
                        {
                            var contacts = await contactRepo.GetAll();
                            response.WriteJson(contacts);
                        });

                        r.MapGet("contacts/{id:int}", async (request, response, routeData) =>
                        {
                            var contact = await contactRepo.Get(Convert.ToInt32(routeData.Values["id"]));
                            if (contact == null)
                            {
                                response.StatusCode = 404;
                                return;
                            }

                            response.WriteJson(contact);
                        });

                        r.MapPost("contacts", async (request, response, routeData) =>
                        {
                            var newContact = request.HttpContext.ReadFromJson<Contact>();
                            if (newContact == null) return;

                            await contactRepo.Add(newContact);

                            response.StatusCode = 201;
                            response.WriteJson(newContact);
                        });

                        r.MapPut("contacts/{id:int}", async (request, response, routeData) =>
                        {
                            var updatedContact = request.HttpContext.ReadFromJson<Contact>();
                            if (updatedContact == null) return;

                            updatedContact.ContactId = Convert.ToInt32(routeData.Values["id"]);
                            await contactRepo.Update(updatedContact);

                            response.StatusCode = 204;
                        });

                        r.MapDelete("contacts/{id:int}", async (request, response, routeData) =>
                        {
                            await contactRepo.Delete(Convert.ToInt32(routeData.Values["id"]));
                            response.StatusCode = 204;
                        });
                    });
                })
                .Build().RunAsync();
    }
}