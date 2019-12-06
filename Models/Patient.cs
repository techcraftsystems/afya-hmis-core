using System;
namespace AfyaHMIS.Models
{
    public class Patient
    {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Identifier { get; set; }
        public string Age { get; set; }
        public Person Person { get; set; }
        public PatientStatus Status { get; set; }
        public PatientIdentifier PI { get; set; }
        public Users AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string Notes { get; set; }
        public string LastVisit { get; set; } 

        public Patient() {
            Id = 0;
            Uuid = "";
            Age = "";
            Identifier = "";
            Person = new Person();
            Status = new PatientStatus();
            AddedBy = new Users();
            AddedOn = DateTime.Now;
            Notes = "";
            LastVisit = "";
        }

        public string GetAge() {
            Age = Person.GetAge();
            return Age;
        }

        public int GetAgeInYears() {
            return Person.GetAgeInYears();
        }

        public string GetUuid() {
            //if (string.IsNullOrEmpty(Uuid))
                //Uuid = new PatientService().GetPatientUuid(this.Id);
            return Uuid;
        }
    }

    public class PatientStatus
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public PatientStatus() {
            Id = 0;
            Name = "";
        }
    }
}
