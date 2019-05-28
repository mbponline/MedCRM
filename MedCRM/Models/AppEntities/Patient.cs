using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedCRM.Data
{
    public class Patient
    {
        public int Id { get; set; }
        public int telegramId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
