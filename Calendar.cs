using ScheduleUsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ScheduleUsers.Helpers
{
    public static class Calendar
    {
        public static string ScheduletoFullCalendar(Schedule schedule, DateTime startDate, DateTime endDate)
        {
            if (schedule != null)
            {
                var eventList = new List<CalendarEvent>();
                // pull the list of workperiods and reorder them by start date ascending
                var sortedWorkPeriods = schedule.WorkPeriods;
                sortedWorkPeriods = sortedWorkPeriods.OrderBy(x => x.StartTime).ToList();
                // parse schedule and create events, adding them to eventList
                for (DateTime runningDate = startDate.Date; runningDate <= endDate.Date; runningDate += TimeSpan.FromDays(1))
                {
                    if (runningDate >= schedule.ScheduleStartDay &&
                        (runningDate <= schedule.ScheduleEndDay ||
                        (schedule.ScheduleEndDay == null &&
                        ((runningDate - schedule.ScheduleStartDay).Days < schedule.WorkPeriods.Count || schedule.Repeating))))
                    {
                        int scheduleIndex = (runningDate - schedule.ScheduleStartDay).Days % schedule.WorkPeriods.Count();
                        int scheduleCycleCount = Convert.ToInt32(Math.Floor(Convert.ToDouble((runningDate.Date - schedule.ScheduleStartDay).Days / schedule.WorkPeriods.Count)));
                        if (sortedWorkPeriods.ElementAt(scheduleIndex).IsDayOff == false)
                        {
                            eventList.Add(new CalendarEvent
                            {
                                id = schedule.Id,
                                title = schedule.User.LastName,
                                description = schedule.Notes,
                                start = sortedWorkPeriods.ElementAt(scheduleIndex).StartTime.Value.AddDays(schedule.WorkPeriods.Count * scheduleCycleCount),
                                end = sortedWorkPeriods.ElementAt(scheduleIndex).EndTime.Value.AddDays(schedule.WorkPeriods.Count * scheduleCycleCount),
                                color = "red"
                            });
                        }
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string scheduletoFullCalendar = serializer.Serialize(eventList);
                // return string to original call
                return scheduletoFullCalendar;
            }
            else
            {
                return null;
            }
        }

		// this overloaded method is used to get the next scheduled datetime the user is scheduled to work
		public static List<CalendarEvent> ScheduletoFullCalendar(Schedule schedule, DateTime startDate, DateTime endDate, bool forNext)
		{
			if (schedule != null)
			{
				var eventList = new List<CalendarEvent>();
				// pull the list of workperiods and reorder them by start date ascending
				var sortedWorkPeriods = schedule.WorkPeriods;
				sortedWorkPeriods = sortedWorkPeriods.OrderBy(x => x.StartTime).ToList();
				// parse schedule and create events, adding them to eventList
				for (DateTime runningDate = startDate; runningDate <= endDate.Date; runningDate += TimeSpan.FromDays(1))
				{
					if (runningDate >= schedule.ScheduleStartDay &&
						(runningDate <= schedule.ScheduleEndDay ||
						(schedule.ScheduleEndDay == null &&
						((runningDate - schedule.ScheduleStartDay).Days < schedule.WorkPeriods.Count || schedule.Repeating))))
					{
						int scheduleIndex = (runningDate - schedule.ScheduleStartDay).Days % schedule.WorkPeriods.Count();
						int scheduleCycleCount = Convert.ToInt32(Math.Floor(Convert.ToDouble((runningDate - schedule.ScheduleStartDay).Days / schedule.WorkPeriods.Count)));
						if (sortedWorkPeriods.ElementAt(scheduleIndex).IsDayOff == false)
						{
							eventList.Add(new CalendarEvent
							{
								id = schedule.Id,
								title = schedule.User.LastName,
								description = schedule.Notes,
								start = sortedWorkPeriods.ElementAt(scheduleIndex).StartTime.Value.AddDays(schedule.WorkPeriods.Count * scheduleCycleCount),
								end = sortedWorkPeriods.ElementAt(scheduleIndex).EndTime.Value.AddDays(schedule.WorkPeriods.Count * scheduleCycleCount),
								color = "red"
							});
						}
					}
				}
				return eventList;
			}
			else
			{
				return null;
			}
		}
	}
}