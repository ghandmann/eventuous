using Eventuous.Sut.Domain;
using NodaTime;

var stayPeriod = new StayPeriod(new LocalDate(2022, 01, 01), new LocalDate(2022, 01, 10));

var booking = new Booking();

booking.BookRoom("1", stayPeriod, 100, "some-guest");

booking.RecordPayment("payment-1234", 50, DateTimeOffset.Now);
