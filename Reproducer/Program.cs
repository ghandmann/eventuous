
using System.Text.Json;
using EventStore.Client;
using Eventuous;
using Eventuous.EventStore;
using Eventuous.Sut.Domain;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

// Setup DI container
var collection = new ServiceCollection();
collection.AddSingleton(new EventStoreClient(
    EventStoreClientSettings.Create("esdb://localhost:2113?tls=false")
));
collection.AddSingleton<IEventSerializer>(
    new DefaultEventSerializer(
        new JsonSerializerOptions(JsonSerializerDefaults.Web)
    )
);
collection.AddSingleton<IEventStore, EsdbEventStore>();
collection.AddSingleton<IAggregateStore, AggregateStore>();

var services = collection.BuildServiceProvider();

// Get an aggregate store
var store = services.GetRequiredService<IAggregateStore>();

BookingEvents.MapBookingEvents();

var booking = new Booking();

// Create a booking
var bookingId = new BookingId(Guid.NewGuid().ToString());
var stayOneNight = new StayPeriod(new LocalDate(2021,1, 1), new LocalDate(2021,1,2));
booking.BookRoom(bookingId, "B12", stayOneNight, 100);

// Store the booking
await store.Store(booking, CancellationToken.None);

// just load the booking back from the store.
// typically fails after less than 20 iterations
for(int i = 0; i < 100; i++) {
    var replayedBooking = await store.Load<Booking>(bookingId, CancellationToken.None);
    Console.WriteLine($"Iteration: {i}. replayedBooking.State.Id={replayedBooking.State.Id}");
}