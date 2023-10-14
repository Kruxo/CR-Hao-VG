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

builder.Services.AddSingleton<BookingProcessor>(); //Ist�llet f�r att skapa ny instans av BookingProcessor hela tiden s� injektserar vi
                                                   //BookingProcessor i den razor sidan vi vill att v�ran asp.net core runtime ska ge oss tillg�ng till BookingProcessor vid beg�ran

builder.Services.AddSingleton<IData, CollectionData>(); //Detta s�ger att n�r vi beg�r en IData s� ska vi f� en CollectionData, som vi i det h�r fallet beh�ver i BookingProcessor. 

await builder.Build().RunAsync();
