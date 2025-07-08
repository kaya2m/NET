using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.Application.DTOs.User
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int AdminUsers { get; set; }
        public int ResidentUsers { get; set; }
        public int MaintenanceUsers { get; set; }
        public int AccountantUsers { get; set; }
        public int SecurityUsers { get; set; }
        public int TodayLogins { get; set; }
        public int WeekLogins { get; set; }
        public int MonthLogins { get; set; }
        public DateTime? LastLogin { get; set; }
        public int LockedUsers { get; set; }
        public int UnconfirmedEmails { get; set; }
    }
}
