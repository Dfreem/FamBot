using System;
using System.IO;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace FamBot.Data.Services;

public class CalendarService
{
    public Calendar GetCalendar { get; set; }

    public CalendarService()
    {
        using (var sr = new StreamReader("Data/freemand@my.lanecc.edu.ics"))
        {
            var calendarStream = sr.ReadToEnd();
            GetCalendar = Calendar.Load(calendarStream);
        }
    }
    public List<CalendarEvent> GetCalendarEventsByDateAsync(IDateTime toGet)
    {
        
        return GetCalendar.Events.Where(e => e.OccursOn(toGet)).ToList();
    }

    public async Task<string> AddReminderAsync(Todo newReminder)
    {
        GetCalendar.AddChild(newReminder);
        CalendarSerializer serial = new();
        string serializedCalendar = serial.SerializeToString(newReminder);
        await File.AppendAllTextAsync("./freemand@my.lanecc.edu", serializedCalendar);
        return await Task.FromResult(serializedCalendar);
    }

    public Task<string> GetThisWeek()
    {
        string todoString = string.Empty;
        IDateTime end = new CalDateTime(DateTime.Now.AddDays(7));
        var todos = GetCalendar
            .Todos.AsParallel()
            .Where(td => td.DtStart.LessThanOrEqual(end));
        todos.ForAll(t => todoString += t.Description);
        return Task.FromResult(todoString);
    }
}

//BEGIN:VCALENDAR
//VERSION:2.0
//PRODID:-//Apple Inc.//macOS 12.6.5//EN
//CALSCALE:GREGORIAN
//BEGIN:VEVENT
//TRANSP:TRANSPARENT
//DTEND; VALUE=DATE:20220620
//X-APPLE-TRAVEL-ADVISORY-BEHAVIOR:AUTOMATIC
//UID:a593943d-9d63-316f-8573-18a4c6f55e47
//DTSTAMP; VALUE=DATE:19760401T000000Z
//SEQUENCE:0
//CLASS:PUBLIC
//X-APPLE-EWS-BUSYSTATUS:FREE
//X-APPLE-UNIVERSAL-ID:48f51f4a-8688-b05c-5acb-148c3a969369
//CATEGORIES:Holidays
//SUMMARY; LANGUAGE=en:Juneteenth
//LAST-MODIFIED:20230622T115833Z
//DTSTART; VALUE=DATE:20220619
//CREATED:20230621T202504Z
//RRULE:FREQ=YEARLY;COUNT=6
//BEGIN:VALARM
//X-WR-ALARMUID:8253FFFE-0052-4CDC-8994-CC6F8E6E4FA0
//UID:8253FFFE-0052-4CDC-8994-CC6F8E6E4FA0
//TRIGGER:-PT15H
//ATTACH; VALUE=URI:Chord
//X-APPLE-LOCAL-DEFAULT-ALARM:TRUE
//ACTION:AUDIO
//X-APPLE-DEFAULT-ALARM:TRUE
//END:VALARM
//END:VEVENT
//END:VCALENDAR
