﻿namespace BlazorComponentHeap.Components.Models.Scheduler;

public class Appointment
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public object Payload { get; set; } = null!;
    public string BackgroundColor { get; set; } = "";
    public string LineColor { get; set; } = "";
    public float Opacity { get; set; } = 0.4f;
}
