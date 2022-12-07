using BlazorComponentHeap.Shared.Models.Datepicker;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorComponentHeap.Components.Calendar.CalendarDays;

public partial class BCHCalendarDays
{
    [Parameter] public string Id { get; set; } = string.Empty;
    
    [Parameter]
    public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;

    [Parameter]
    public int DefaultMonth { get; set; }

    [Parameter]
    public int DefaultYear { get; set; }

    [Parameter]
    public bool IsDateRange { get; set; }

    [Parameter]
    public EventCallback<bool> IsShowMonth { get; set; }

    [Parameter]
    public EventCallback<DateTime> OnDateSelected { get; set; }

    [Parameter]
    public EventCallback OnFocusOut { get; set; }

    [Parameter]
    public DateTime SelectedDate { get; set; }

    [Parameter]
    public EventCallback<DateTime> SelectedStartDay { get; set; }

    [Parameter]
    public DateTime StartDay { get; set; } = DateTime.MinValue;

    [Parameter]
    public DateTime EndDay { get; set; } = DateTime.MinValue;

    [Parameter]
    public EventCallback OnCloseDate { get; set; }

    [Parameter]
    public EventCallback<DateRange> ValuesChanged { get; set; }

    [Parameter]
    public DateRange Values
    {
        get => _values;
        set
        {
            if (_values == value) return;
            _values = value;

            ValuesChanged.InvokeAsync(value);
        }
    }

    private List<string> _weekDays = new();
    private List<WeekDay> _days = new();
    private int _rowsCount = 0;
    private CultureInfo culture = null!;
    private int _month = DateTime.Now.Month;
    private int _year = DateTime.Now.Year;
    private DateRange _values = new();

    private DateTime _startDay;
    private DateTime _endDay;

    protected override void OnInitialized()
    {
        culture = new CultureInfo(Culture);

        _startDay = StartDay;
        _endDay = EndDay;

        if (SelectedDate.Month > _month)
        {
            _month = SelectedDate.Month;
            _year = SelectedDate.Year;
        }
        else
        {
            _month = DefaultMonth == 0 ? DateTime.Now.Month : DefaultMonth;
            _year = DefaultYear == 0 ? DateTime.Now.Year : DefaultYear;
        }

        SetWeekDayNamesToCalendars();
        UpdateCalendar(_year, _month);
    }

    private void UpdateCalendar(int year, int month)
    {
        var countDayOfMounth = DateTime.DaysInMonth(year, month);

        var countDayInPreviusMonth = DateTime.DaysInMonth(GetPreviousYear(year, month), GetPreviousMonth(month));

        var firstDayInMonth = (int)new DateTime(year, month, 1).DayOfWeek;
        var lastDayInMonth = (int)new DateTime(year, month, countDayOfMounth).DayOfWeek;
        int numberOfEmptyDays = 0;

        firstDayInMonth = firstDayInMonth == 0 ? 7 : firstDayInMonth;
        numberOfEmptyDays = firstDayInMonth - 1;

        _days.Clear();

        if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
        {
            numberOfEmptyDays += 1;
        }

        if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Saturday)
        {
            numberOfEmptyDays += 2;
        }

        for (int i = 0; i < numberOfEmptyDays; i++)
        {
            _days.Add(
                new WeekDay
                {
                    Date = new DateTime(GetPreviousYear(year, month), GetPreviousMonth(month), countDayInPreviusMonth--),
                    IsOtherDay = true
                }
            );
        }

        _days = Enumerable.Reverse(_days).ToList();

        for (int i = 0; i < countDayOfMounth; i++)
        {
            var day = i + 1;
            _days.Add(
                new WeekDay
                {
                    Date = new DateTime(year, month, day),
                    IsOtherDay = false
                }
            );
        }

        if (lastDayInMonth != 7 && lastDayInMonth != 0)
        {
            for (int i = 0; i < (7 - lastDayInMonth); i++)
            {
                var newDay = i + 1;
                _days.Add(
                    new WeekDay
                    {
                        Date = new DateTime(GetNextYear(year, month), GetNextMonth(month), newDay),
                        IsOtherDay = true
                    }
                );
            }
        }

        _rowsCount = _days.Count % 7 == 0 ? (_rowsCount = _days.Count / 7) : (Convert.ToInt32(_days.Count / 7) + 1);
        StateHasChanged();
    }

    private void SetWeekDayNamesToCalendars()
    {
        _weekDays = culture.DateTimeFormat.ShortestDayNames.ToList();

        if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
        {
            _weekDays.Add(_weekDays[0]);
            _weekDays.RemoveAt(0);
        }

        if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Saturday)
        {
            _weekDays.Insert(0, _weekDays[6]);
            _weekDays.RemoveAt(7);
        }
    }

    private int GetPreviousMonth(int month)
    {
        return month == 1 ? 12 : month - 1;
    }

    private int GetPreviousYear(int currentYear, int month)
    {
        return month == 1 ? currentYear - 1 : currentYear;
    }

    private int GetNextMonth(int month)
    {
        return month == 12 ? 1 : month + 1;
    }

    private int GetNextYear(int currentYear, int month)
    {
        return month == 12 ? currentYear + 1 : currentYear;
    }

    private async Task NextMonthAsync()
    {
        if (_month == 12)
        {
            _month = 1;
            _year++;
            UpdateCalendar(_year, _month);
            return;
        }
        _month++;
        UpdateCalendar(_year, _month);
        await OnFocusOut.InvokeAsync();

    }

    private async Task PreviousMonthAsync()
    {
        if (_month == 1)
        {
            _month = 12;
            _year--;
            UpdateCalendar(_year, _month);
            return;
        }
        _month--;
        UpdateCalendar(_year, _month);
        await OnFocusOut.InvokeAsync();
    }

    private string GetMonthName(int month)
    {
        return month switch
        {
            1 => culture.DateTimeFormat.GetMonthName(month),
            2 => culture.DateTimeFormat.GetMonthName(month),
            3 => culture.DateTimeFormat.GetMonthName(month),
            4 => culture.DateTimeFormat.GetMonthName(month),
            5 => culture.DateTimeFormat.GetMonthName(month),
            6 => culture.DateTimeFormat.GetMonthName(month),
            7 => culture.DateTimeFormat.GetMonthName(month),
            8 => culture.DateTimeFormat.GetMonthName(month),
            9 => culture.DateTimeFormat.GetMonthName(month),
            10 => culture.DateTimeFormat.GetMonthName(month),
            11 => culture.DateTimeFormat.GetMonthName(month),
            12 => culture.DateTimeFormat.GetMonthName(month),
            _ => ""
        };
    }

    private async Task SelectDateAsync(DateTime date)
    {
        if (IsDateRange)
        {
            await SelectRangeDateAsync(date);
        }

        await OnFocusOut.InvokeAsync();
        await OnDateSelected.InvokeAsync(date);
        StateHasChanged();
    }

    private async Task ApplyDatesAsync()
    {
        Values.Start = _startDay;
        Values.End = _endDay;
        await OnCloseDate.InvokeAsync();
    }

    private async Task SelectRangeDateAsync(DateTime date)
    {

        if (_startDay == date)
        {
            _startDay = DateTime.MinValue;
            _endDay = DateTime.MinValue;
        }

        if (_endDay == date)
        {
            _endDay = DateTime.MinValue;
            _startDay = date;
            await SelectedStartDay.InvokeAsync(date);
        }

        if (_startDay == DateTime.MinValue)
        {
            _startDay = date;
            await SelectedStartDay.InvokeAsync(date);
        }

        if (date > _startDay)
        {
            _endDay = date;
        }

        if (date < _startDay)
        {
            _startDay = date;
            _endDay = DateTime.MinValue;
            await SelectedStartDay.InvokeAsync(date);
        }

        StateHasChanged();
    }

    private async Task OnClickByMonthAsync()
    {
        await IsShowMonth.InvokeAsync(true);
    }

}
