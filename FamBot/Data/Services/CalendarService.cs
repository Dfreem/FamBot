using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Serilog;

namespace FamBot.Data.Services;

public class CalendarService
{
    Calendar GetCalendar { get; set; }
    public CalendarService()
    {
        string calPath = Path.GetFullPath("Data/icalexport.ics");
        using (StreamReader fs = new(calPath))
        {
            CalendarSerializer serial = new();
            GetCalendar = Calendar.Load(fs)??new();
            Log.Logger.Information($"Calendar Service ctor, Calendar.Load \n{serial.SerializeToString(GetCalendar)}");
        }
    }
    public async Task<string> GetCalendarEventsByDateAsync(IDateTime toGet)
    {
        string calendarString = string.Empty;

        var events = GetCalendar.Events.Where(e => e.OccursOn(toGet)).ToList();
        if (events.Any())
        {
            events.ForEach(e => { calendarString += $"{e.Name}\n{e.Description}"; });
        }

        return await Task.FromResult(calendarString);
    }

    public async Task<string> GetTodos()
    {
        string reminders = string.Empty;
        var events = GetCalendar.Todos.ToList();
        if (!events.Any())
        {
            reminders = "No events found";
        }
        events.ForEach(e => reminders += e.Description.ToString() + "\n");
        Log.Logger.Information($"From GetAllCalendarEventsAsync in CalendarService:\n{reminders}");
        return await Task.FromResult(reminders);
    }

    public async Task<string> AddTodoAsync(string todoName, string desc, DayOfWeek doOn)
    {
        var stamp = new CalDateTime(DateTime.Now);
        var dt = new CalDateTime(doOn.ToString());
        GetCalendar.AddChild(new Todo
        {
            Name = todoName,
            Description = desc,
            DtStart = dt,
            DtStamp = stamp,
            RecurrenceRules = new List<RecurrencePattern>
            {
                new RecurrencePattern(FrequencyType.None, 1)
            },
            Summary = todoName + desc
        });
        CalendarSerializer serial = new();
        string serializedCalendar = serial.SerializeToString(GetCalendar);
        Log.Logger.Information($"Reminder: {todoName}\n{desc}");
        await File.AppendAllTextAsync("Data/icalexport.ics", serializedCalendar);
        Log.Logger.Information($"From AddReminderAsync in CalendarService:\n{serializedCalendar}");
        return await Task.FromResult(serializedCalendar);

    }

    public async Task<string> GetThisWeek()
    {
        CalendarSerializer serial = new();
        IDateTime start = new CalDateTime(DateTime.Now);
        var filteredTodos = GetCalendar.Todos
            .OrderByDescending(td => td.Created.LessThanOrEqual(start));
        string serializedCalendar = serial
            .SerializeToString(filteredTodos);
        var calResult = await Task.FromResult(serializedCalendar);
        Log.Logger.Information($"From GetThisWeek in CalendarService:\n{serializedCalendar}");
        return calResult;
    }

    public override string ToString()
    {
        CalendarSerializer serializer = new();

        return serializer.SerializeToString(GetCalendar);
    }
}