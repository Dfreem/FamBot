using Ical.Net.CalendarComponents;

namespace FamBotTests
{
    public class CalendarTests
    {
        private CalendarService _getCalendar;
        string fp = "../../../../FamBot/Data/icalexport.ics";

        [SetUp]
        public void Setup()
        {
            _getCalendar = new(fp);

            _getCalendar.AddTodoAsync("test", "test", DayOfWeek.Thursday).Wait();
            File.WriteAllText(fp, _getCalendar.ToString());
        }
        [TearDown]
        public void TearDown()
        {
            File.Delete(fp);
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

        [Test]
        public void TestGetTodosReturnsCalendarTodos()
        {
            // Arrange
            var allTodos = _getCalendar.GetAllTodos().Result;

            // Act
            //var lines = allTodos.Split(new char[] { ':', '\n', 'r' }, StringSplitOptions.TrimEntries);

            // Assert
            Assert.That(allTodos.Count, Is.AtLeast(1));

        }
    }
}