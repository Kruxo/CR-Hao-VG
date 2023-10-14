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

builder.Services.AddSingleton<BookingProcessor>(); //Istället för att skapa ny instans av BookingProcessor hela tiden så injektserar vi
                                                   //BookingProcessor i den razor sidan vi vill att våran asp.net core runtime ska ge oss tillgång till BookingProcessor vid begäran

builder.Services.AddSingleton<IData, CollectionData>(); //Detta säger att när vi begär en IData så ska vi få en CollectionData, som vi i det här fallet behöver i BookingProcessor. 

await builder.Build().RunAsync();
