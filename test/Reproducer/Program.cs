using Eventuous.Sut.Domain;
using NodaTime;

var stayPeriod = new StayPeriod(new LocalDate(2022, 01, 01), new LocalDate(2022, 01, 10));

var booking = new Booking();
booking.BookRoom("1", stayPeriod, 100, "some-guest");
// Next line throws, since RecordPayment calls 'EnsureExists' while the 'Aggregate.Original' property still indicates a non existing aggregate
booking.RecordPayment("payment-1234", 50, DateTimeOffset.Now);



// Workaround with 'Load()'

var booking2 = new Booking();
booking2.BookRoom("1", stayPeriod, 100, "some-guest");

// Backfeeding the changes to the aggregate works, but feels wrong
booking2.Load(booking2.Changes);

booking2.RecordPayment("payment-1234", 50, DateTimeOffset.Now);
