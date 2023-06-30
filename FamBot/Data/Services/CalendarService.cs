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
    public Calendar GetCalendar { get; set; }
    string _fp;

    public CalendarService(string filePath = "Data/icalexport.ics")
    {
        _fp = filePath;
        if (File.Exists(filePath))
        {
            var calString = File.ReadAllText(filePath);
            GetCalendar = Calendar.Load(calString) ?? new Calendar();
            Log.Logger.Information(GetCalendar.ToString()!);
        }
        else
        {
            GetCalendar = new Calendar();
            File.WriteAllText( filePath, GetCalendar.ToString());
        }
    }

    /// <summary>
    /// Get all the events in the calendar for the specified date
    /// </summary>
    /// <param name="toGet">the date to get all the events for</param>
    /// <returns>a string containing all the serialized events</returns>
    public async Task<string> GetCalendarEventsByDateAsync(DateTime toGet)
    {
        IDateTime realDate = new CalDateTime(toGet);
        string calendarString = string.Empty;
        CalendarSerializer serial = new();

        var events = GetCalendar.Events.Where(e => e.OccursOn(realDate)).ToList();
        if (events is not null)
        {
            calendarString = serial.SerializeToString(events);
        }
        return await Task.FromResult(calendarString);
    }

    /// <summary>
    /// Get all the todos stored in the calendar.
    /// </summary>
    /// <returns>a string of serialized <see cref="Todo"/></returns>
    public async Task<string> GetAllTodos()
    {
        CalendarSerializer serial = new();

        var allTodos = serial.SerializeToString(GetCalendar.Todos);
        return await Task.FromResult(allTodos);
    }

    public string? GetTodo(string uid)
    {
        CalendarSerializer serial = new();
        foreach (var todo in GetCalendar.Todos)
        {
            if (todo.Uid == uid)
            {
                return serial.SerializeToString(todo);
            }
        }
        return null;

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
            Summary = todoName + desc,
            
        });
        CalendarSerializer serial = new();
        string serializedCalendar = serial.SerializeToString(GetCalendar);
        await File.WriteAllTextAsync(_fp, serializedCalendar);
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

    public async Task<bool> RemoveEventAsync(CalendarEvent toRemove)
    {
        bool wasRemoved = GetCalendar.Events.Remove(toRemove);
        await Task.CompletedTask;
        return wasRemoved;
    }
    public async Task<bool> RemoveTodoAsync(Todo toRemove)
    {
        bool wasRemoved = GetCalendar.Todos.Remove(toRemove);
        await Task.CompletedTask;
        return wasRemoved;
    }

    /// <summary>
    /// returns the entire calendar as a serialzed, human readable string.
    /// </summary>
    /// <returns>the calendar as a string</returns>
    public override string ToString()
    {
        CalendarSerializer serializer = new();

        return serializer.SerializeToString(GetCalendar);
    }
}