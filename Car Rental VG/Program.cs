using Car_Rental.Business.Classes;
using Car_Rental.Common.Classes;
using Car_Rental.Data.Classes;
using Car_Rental.Data.Interfaces;
using Car_Rental_VG;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<BookingProcessor>(); //Ist�llet f�r att skapa ny instans av BookingProcessor hela tiden s� injekterar vi
                                                   //BookingProcessor i den razor sidan vi vill att v�ran asp.net core runtime ska ge oss tillg�ng till BookingProcessor vid beg�ran
builder.Services.AddSingleton<IData, CollectionData>();

await builder.Build().RunAsync();
