
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ScheduleUsers.Helpers;


namespace ScheduleUsers.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        // [Required]

        // additional inherited properties: Email, PhoneNumber, Id, UserName

        /// <summary>
        /// User's First Name.
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// User's Middle Name.
        /// </summary>
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        //[Required]
        /// <summary>
        /// User's Last Name.
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// User's Hire Date.
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// User's Department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// User's Position.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// User's Birthday
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// User's Gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// User's Alternate Email.
        /// </summary>
        public string AlternateEmail { get; set; }

        /// <summary>
        /// User's Work Phone.
        /// </summary>
        public string WorkPhone { get; set; }

        /// <summary>
        /// User's Mobile Phone.
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// User's Address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// User's Hourly Wage.
        /// </summary>
        public decimal HourlyPayRate { get; set; }

        /// <summary>
        /// Distinction if the user is a fulltime employee or not.
        /// </summary>
        public bool Fulltime { get; set; }

        /// <summary>
        /// User's Clocked-in Status.
        /// </summary>
        public string Status { get; set; }


        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<TimeOffEvent> RequestedTimeOff { get; set; }
        public virtual ICollection<WorkTimeEvent> WorkEvents { get; set; }
        [InverseProperty("Sender")]
        public virtual ICollection<Message> SenderMessages { get; set; }
        [InverseProperty("Recipient")]
        public virtual ICollection<Message> RecipientMessages { get; set; }

        public static string getStatus(string userId)
        {
            bool ClockedInStatus;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Checking for clocked in status
                ClockedInStatus = db.WorkTimeEvents.Any(x => x.Id == userId && !x.End.HasValue);
            }
            if (ClockedInStatus)
            {
                return "Clocked In";
            }
            else
            {
                return "Not Clocked In";
            }
        }

        public static string getNextScheduled(string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            DateTime nextScheduled;
            try
            {
                Schedule thisSchedule = db.Schedules.Where(x => x.UserId == userId).First();
                var CurrentSchedule = Calendar.ScheduletoFullCalendar(thisSchedule, DateTime.Now, DateTime.Now.AddMonths(1), true);
                nextScheduled = (DateTime)CurrentSchedule.FirstOrDefault().start;
                if (DateTime.Now < nextScheduled)
                {
                    return nextScheduled.ToString();
                }
                else
                {
                    CurrentSchedule.Remove(CurrentSchedule.FirstOrDefault());
                    nextScheduled = (DateTime)CurrentSchedule.FirstOrDefault().start;
                    return nextScheduled.ToString();
                }
            }
            catch
            {
                return "Not Scheduled";
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ScheduleUsers", throwIfV1Schema: false)
        {
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PayPeriod> PayPeriods { get; set; }
        public DbSet<WorkTimeEvent> WorkTimeEvents { get; set; }
        public DbSet<TimeOffEvent> TimeOffEvents { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<WorkPeriod> WorkPeriods { get; set; }
    }
}