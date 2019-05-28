using System;

namespace MedCRM.Data
{
    public class Notification
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string NotificationText { get; set; }
        public DateTime SendTime { get; set; }
    }
}