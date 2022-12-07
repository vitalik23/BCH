using BlazorComponentHeap.Components.Models.Scheduler;
using BlazorComponentHeap.Components.Scheduler.AppointmentItem;
using BlazorComponentHeap.Core.Services.Interfaces;
using BlazorComponentHeap.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorComponentHeap.Components.Scheduler;

public partial class BCHScheduler : IDisposable
{
    [Inject]
    private ISubscriptionService _subscriptionService { get; set; } = null!;

    [Parameter] public string Gap { get; set; } = "4px";
    [Parameter] public string ItemHeight { get; set; } = "90px";
    [Parameter] public DateTime WeekDate { get; set; } = DateTime.Now;
    [Parameter] public List<Appointment> Appointments { get; set; } = new();
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public bool ShowDatePicker { get; set; } = false;
    [Parameter] public bool ShowTodayButton { get; set; } = false;

    private DateTime _currentWeekStart;
    private DateTime _calendarDateTime;
    private int _scrollIndex = 0;

    private Dictionary<string, BCHAppointmentItem> _appointmentTemplates = new();
    private Dictionary<DateTime, Day> _days = new();

    private NumberFormatInfo _numberFormatWithDot = new NumberFormatInfo { NumberDecimalSeparator = "." };

    protected override void OnInitialized()
    {
        _subscriptionService.OnUpdate += StateHasChanged;

        if (!_subscriptionService.IsSubscriptionActivated) return;

        _currentWeekStart = WeekDate.StartOfWeek();
        _calendarDateTime = _currentWeekStart;

        Update();
    }

    public void Dispose()
    {
        _subscriptionService.OnUpdate -= StateHasChanged;
    }

    public void Update()
    {
        foreach (var appointment in Appointments)
        {
            var dateCounter = appointment.Start.Date;

            do
            {
                _days.TryGetValue(dateCounter, out var day);

                if (day is null)
                {
                    day = new Day();
                    _days.Add(dateCounter, day);
                }

                if (!day.Groups.Any(x => x.Appointments.Contains(appointment)))
                {
                    day.Groups.Add(new TimeIntersectionGroup
                    {
                        Start = appointment.Start,
                        End = appointment.End,
                        Appointments = new List<Appointment>
                        {
                            appointment
                        }
                    });
                }

                dateCounter = dateCounter.AddDays(1);
            }
            while (dateCounter <= appointment.End.Date);
        }

        foreach (var keyValue in _days)
        {
            var day = keyValue.Value;

            DetectGroups(day.Groups);

            foreach (var group in day.Groups)
            {
                group.Start = group.Start.AddDays(1);
                group.Appointments = group.Appointments.OrderByDescending(x => x.End - x.Start).ToList();
            }
        }

        StateHasChanged();
    }

    internal void AddAppointmentItem(BCHAppointmentItem item)
    {
        _appointmentTemplates.Add(item.Key, item);
        StateHasChanged();
    }

    private void DetectGroups(List<TimeIntersectionGroup> groups)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];

            for (int j = i + 1; j < groups.Count; j++)
            {
                if (MergeGroups(group, groups[j]))
                {
                    groups.Remove(groups[j]);
                    i = -1;
                    break;
                }
            }
        }
    }

    private bool MergeGroups(TimeIntersectionGroup group1, TimeIntersectionGroup group2)
    {
        if ((group1.Start <= group2.End && group1.Start >= group2.Start) || (group2.Start <= group1.End && group2.Start >= group1.Start))
        {
            group1.Start = group2.Start < group1.Start ? group2.Start : group1.Start;
            group1.End = group2.End > group1.End ? group2.End : group1.End;

            group1.Appointments.AddRange(group2.Appointments);

            return true;
        }

        return false;
    }

    private void OnWeekButtonClicked(bool isRight)
    {
        _scrollIndex += isRight ? 1 : -1;
        _calendarDateTime = _currentWeekStart.AddDays(7 * _scrollIndex).StartOfWeek();

        StateHasChanged();
    }

    private string GetMonthLabel(int num) => num switch
    {
        1 => "Jan",
        2 => "Feb",
        3 => "March",
        4 => "Apr",
        5 => "May",
        6 => "June",
        7 => "July",
        8 => "Aug",
        9 => "Sep",
        10 => "Oct",
        11 => "Nov",
        12 => "Dec",
        _ => throw new ArgumentException("Undefined month")
    };

    private string GetTimeLabel(int hour)
    {
        return $"{(hour < 10 ? "0" : "")}{hour}:00";
    }

    private float GetTimeNumber(DateTime dateTime)
    {
        return dateTime.Hour + (dateTime.Minute / 60.0f) + (dateTime.Second / 60.0f) * 0.1f;
    }

    private void OnDateChanged(DateTime dateTime)
    {
        _calendarDateTime = dateTime;
        _scrollIndex = (_calendarDateTime.StartOfWeek() - _currentWeekStart.StartOfWeek()).Days / 7;

        StateHasChanged();
    }

    private void OnTodayClicked() => OnDateChanged(DateTime.Now);
}
