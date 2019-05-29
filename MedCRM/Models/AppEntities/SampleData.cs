using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedCRM.Data;

namespace MedCRM.Models.AppEntities
{
    public class SampleData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.Patients.Any())
            {
                context.Patients.AddRange(
                    new Patient
                    {
                        Firstname = "Vasya",
                        Lastname = "Pupkin",
                        PhoneNumber = "+77778226291"
                    });
                context.SaveChanges();
            }
        }
    }
}
