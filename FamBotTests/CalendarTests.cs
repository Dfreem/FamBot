using Ical.Net.CalendarComponents;

namespace FamBotTests
{
    public class CalendarTests
    {
        private CalendarService _getCalendar;
        [SetUp]
        public void Setup()
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.Console()
              .WriteTo.File("logs/tests.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();
            _getCalendar = new();
        }

        [Test]
        public void TestCalendarServiceLoadsCalendarOnCreate()
        {
            // Arrange
            string calString = _getCalendar.ToString();

            // Act
            string[] startTag = calString.Split(':', '\n', '\r');
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_getCalendar, Is.Not.Null);
                Assert.That(startTag[0], Is.EqualTo("BEGIN"));
                Assert.That(startTag[1], Is.EqualTo("VCALENDAR"));
            });
        }

        //[Test]
        //public void TestGetTodosReturnsCalendarTodos()
        //{
        //    // Arrange
            
        //    var allTodos = _getCalendar.GetTodos().Result;

        //    // Act

        //    // Assert
        //    Assert.That(allTodos.Count, Is.AtLeast(1));

        //}
    }
}