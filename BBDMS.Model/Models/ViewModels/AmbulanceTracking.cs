using System;
using System.Collections.Generic;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Model.Models.ViewModels
{
    /// <summary>A single entry of the live ambulance dispatch timeline.</summary>
    public class AmbulanceTimelineStep
    {
        public string Title { get; set; } = string.Empty;
        public string Sub { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-circle";
        public bool Done { get; set; }
        public bool Current { get; set; }
        public bool Cancelled { get; set; }
    }

    /// <summary>
    /// Single source of truth for how an ambulance dispatch request is presented
    /// (status label, badge, ETA wording and timeline steps). Shared by the controller
    /// (server rendered page + JSON polling endpoint) so both always agree.
    /// </summary>
    public static class AmbulanceTracking
    {
        public const string Pending = "Pending";
        public const string Dispatched = "Dispatched";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static string NormalizeStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return Pending;
            var value = status.Trim();
            if (value.Equals("Canceled", StringComparison.OrdinalIgnoreCase)) return Cancelled;
            if (value.Equals(Dispatched, StringComparison.OrdinalIgnoreCase)) return Dispatched;
            if (value.Equals(Completed, StringComparison.OrdinalIgnoreCase)) return Completed;
            if (value.Equals(Cancelled, StringComparison.OrdinalIgnoreCase)) return Cancelled;
            return Pending;
        }

        public static string StatusLabel(string? status)
        {
            switch (NormalizeStatus(status))
            {
                case Dispatched: return "Ambulance Dispatched";
                case Completed: return "Transport Completed";
                case Cancelled: return "Request Cancelled";
                default: return "Pending Dispatch";
            }
        }

        public static string StatusBadge(string status)
        {
            switch (NormalizeStatus(status))
            {
                case Dispatched: return "badge-soft-warning";
                case Completed: return "badge-soft-success";
                case Cancelled: return "badge-soft-secondary";
                default: return "badge-soft-danger";
            }
        }

        public static string StatusIcon(string status)
        {
            switch (NormalizeStatus(status))
            {
                case Dispatched: return "fa-shipping-fast";
                case Completed: return "fa-check-circle";
                case Cancelled: return "fa-ban";
                default: return "fa-clock";
            }
        }

        /// <summary>Human readable ETA / next-action wording for the live panel.</summary>
        public static string EtaText(string status)
        {
            switch (NormalizeStatus(status))
            {
                case Dispatched: return "Crew en route — estimated arrival in about 15 minutes";
                case Completed: return "Patient transported — dispatch closed";
                case Cancelled: return "This dispatch request was cancelled";
                default: return "Awaiting dispatcher confirmation — keep your line free";
            }
        }

        /// <summary>Rough completion of the dispatch journey, used by the tracking progress bar.</summary>
        public static int ProgressPercent(string status)
        {
            switch (NormalizeStatus(status))
            {
                case Dispatched: return 75;
                case Completed: return 100;
                case Cancelled: return 100;
                default: return 25;
            }
        }

        /// <summary>True while the dispatch is still being worked on (used to keep the live poller on).</summary>
        public static bool IsActive(string status)
        {
            var normalized = NormalizeStatus(status);
            return normalized == Pending || normalized == Dispatched;
        }

        /// <summary>
        /// Builds the tracking timeline: request received → dispatcher assigned →
        /// ambulance en route (patient pickup) → transport completed. A cancelled
        /// request collapses into a two step timeline that ends on the cancellation.
        /// </summary>
        public static List<AmbulanceTimelineStep> BuildSteps(AmbulanceRequest? request)
        {
            var steps = new List<AmbulanceTimelineStep>();
            if (request == null) return steps;

            var status = NormalizeStatus(request.Status);
            var baseTime = request.RequestDate == default ? DateTime.Now : request.RequestDate;

            var received = new AmbulanceTimelineStep
            {
                Title = "Request Received",
                Sub = "Your emergency dispatch request has been logged with our control room.",
                Time = baseTime.ToString("hh:mm tt"),
                Icon = "fa-check",
                Done = true
            };
            steps.Add(received);

            if (status == Cancelled)
            {
                steps.Add(new AmbulanceTimelineStep
                {
                    Title = "Dispatch Cancelled",
                    Sub = "This request was cancelled. Submit a new request or call the helpline if you still need an ambulance.",
                    Time = baseTime.AddMinutes(10).ToString("hh:mm tt"),
                    Icon = "fa-ban",
                    Current = true,
                    Cancelled = true
                });
                return steps;
            }

            var assigned = status == Dispatched || status == Completed;
            var enRoute = status == Dispatched || status == Completed;
            var completed = status == Completed;

            steps.Add(new AmbulanceTimelineStep
            {
                Title = "Dispatcher Assigned",
                Sub = "A dispatcher has picked up your case and is contacting the nearest ambulance fleet.",
                Time = assigned ? baseTime.AddMinutes(5).ToString("hh:mm tt") : "Awaiting",
                Icon = assigned ? "fa-check" : "fa-dot-circle",
                Done = assigned,
                Current = !assigned
            });

            steps.Add(new AmbulanceTimelineStep
            {
                Title = "Ambulance En Route",
                Sub = "The ambulance has been dispatched and is on the way to your pickup location.",
                Time = enRoute ? baseTime.AddMinutes(15).ToString("hh:mm tt") : "Pending",
                Icon = completed ? "fa-check" : enRoute ? "fa-dot-circle" : "fa-circle",
                Done = completed,
                Current = enRoute && !completed
            });

            steps.Add(new AmbulanceTimelineStep
            {
                Title = "Patient Transported",
                Sub = "The crew reached the patient and completed the transport to the destination hospital.",
                Time = completed ? baseTime.AddHours(1).ToString("hh:mm tt") : "Pending",
                Icon = completed ? "fa-check" : "fa-circle",
                Done = completed
            });

            return steps;
        }
    }
}
