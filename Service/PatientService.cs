using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Persons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface IPatientService {
        public List<Patient> SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "");
        public List<SelectListItem> GetPatientIdentificationTypes();

        public string GetPatientUuid(Patient patient);

        public Person SavePerson(Person person);
        public PersonAddress SavePersonAddress(PersonAddress address);
        public Patient SavePatient(Patient patient);
        public PatientIdentifier SavePatientIdentifier(PatientIdentifier identifier);
    }

    public class PatientService : IPatientService {
        private List<SelectListItem> GetIEnumerable(string query) {
            List<SelectListItem> ienumarable = new List<SelectListItem>();
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect(query);
            if (dr.HasRows)
            {
                while (dr.Read()) {
                    ienumarable.Add(new SelectListItem
                    {
                        Value = dr[0].ToString(),
                        Text = dr[1].ToString()
                    });
                }
            }

            return ienumarable;
        }

        public List<Patient> SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "")
        {
            SqlServerConnection conn = new SqlServerConnection();
            List<Patient> search = new List<Patient>();
            string query = "WHERE pt_idnt>0";
            double ageInt;

            if (!string.IsNullOrEmpty(names))
                query += conn.GetQueryString(names, "ps_name", "", true, false);
            if (!string.IsNullOrEmpty(identifier))
                query += conn.GetQueryString(identifier, "pt_identifier+'-'+pi_identifier", "", true, false);
            if (!string.IsNullOrEmpty(phone))
                query += conn.GetQueryString(phone, "pa_telephone+'-'+pa_location+'-'+pa_email", "", true, false);
            if (!string.IsNullOrEmpty(age) && Double.TryParse(age, out ageInt) && Double.Parse(age) > 0)
                query += " AND ps_dob BETWEEN DATEADD(YEAR, 0-" + (ageInt + 1) + ", GETDATE()) AND DATEADD(YEAR, 0-" + (ageInt - 1) + ", GETDATE())";
            if (!string.IsNullOrEmpty(gender))
                query += " AND ps_gender='" + gender + "'";
            if (!string.IsNullOrEmpty(visit))
                query += " AND last_visit>=DATEADD(MONTH, 0-" + Int32.Parse(visit) + ", GETDATE())";

            SqlDataReader dr = conn.SqlServerConnect("SELECT pt_idnt, pt_uuid, pt_identifier, pt_notes, pt_added_by, pt_added_on, pt_visit, pst_idnt, pst_status, pi_idnt, pi_identifier, pi_notes, pit_idnt, pit_type, ps_idnt, ps_name, ps_gender, ps_dob, ps_estimate, ps_added_on, ps_added_by, ps_notes, pa_idnt, pa_default, pa_telephone, pa_email, pa_location, pa_added_on, pa_added_by, pa_notes FROM vPatient WHERE pt_idnt IN (SELECT DISTINCT TOP(100)pt_idnt FROM vPatientSearch " + query + ") ORDER BY ps_name, pt_identifier");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Patient patient = new Patient {
                        Id = Convert.ToInt64(dr[0]),
                        Uuid = dr[1].ToString(),
                        Identifier = dr[2].ToString(),
                        Notes = dr[3].ToString(),
                        AddedBy = new Users { Id = Convert.ToInt64(dr[4]) },
                        AddedOn = Convert.ToDateTime(dr[5]),
                        LastVisit = Convert.ToDateTime(dr[6]).ToString("dd/MM/yyyy"),
                        Status = new PatientStatus {
                            Id = Convert.ToInt64(dr[7]),
                            Name = dr[8].ToString()
                        },
                        PI = new PatientIdentifier {
                            Id = Convert.ToInt64(dr[9]),
                            Identifier = dr[10].ToString(),
                            Notes = dr[11].ToString(),
                            Type = new PatientIdentifierType {
                                Id = Convert.ToInt64(dr[12]),
                                Name = dr[13].ToString()
                            }
                        },
                        Person = new Person {
                            Id = Convert.ToInt64(dr[14]),
                            Name = dr[15].ToString(),
                            Gender = dr[16].ToString(),
                            DateOfBirth = Convert.ToDateTime(dr[17]),
                            Estimate = Convert.ToBoolean(dr[18]),
                            AddedOn = Convert.ToDateTime(dr[19]),
                            AddedBy = new Users { Id = Convert.ToInt64(dr[20]) },
                            Notes = dr[21].ToString(),
                            Address = new PersonAddress
                            {
                                Id = Convert.ToInt64(dr[22]),
                                Default = Convert.ToBoolean(dr[23]),
                                Telephone = dr[24].ToString(),
                                Email = dr[25].ToString(),
                                Location = dr[26].ToString(),
                                AddedOn = Convert.ToDateTime(dr[27]),
                                AddedBy = new Users { Id = Convert.ToInt64(dr[28]) },
                                Notes = dr[29].ToString()
                            }
                        }
                    };

                    patient.GetAge();
                    search.Add(patient);
                }
            }

            return search;
        }

        public string GetPatientUuid(Patient patient) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pt_uuid FROM Patient WHERE pt_idnt=" + patient.Id);
            if (dr.Read()) {
                return dr[0].ToString();
            }

            return "";
        }

        public List<SelectListItem> GetPatientIdentificationTypes() {
            return GetIEnumerable("SELECT pit_idnt, pit_type FROM PatientIdentifierType ORDER BY pit_order, pit_idnt");
        }

        public string GetPatientIdentifier(Patient patient) {
            string prefix = patient.Person.Name.Replace(" ", "").Substring(0, 3).ToUpper() + patient.Person.Gender.Substring(0,1).ToUpper();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ISNULL(MAX(SUBSTRING(pt_identifier, 5, 100)),0)+1 FROM Patient WHERE pt_identifier LIKE '" + prefix + "%'");
            if (dr.Read()) {
                return prefix + dr[0].ToString().PadLeft(7, '0');
            }

            return prefix + "0000001";
        }

        /* Section
         * For
         * Data
         * Writers
         */

        public Person SavePerson(Person person) {
            SqlServerConnection conn = new SqlServerConnection();
            person.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + person.Id + ", @name NVARCHAR(250)='" + person.Name.ToUpper() + "', @gender NVARCHAR(1)='" + person.Gender + "', @dob DATE='" + person.DateOfBirth + "', @actor INT=" + person.AddedBy.Id + ", @estimate BIT='" + person.Estimate + "', @notes NVARCHAR(MAX)='" + person.Notes + "'; IF NOT EXISTS (SELECT ps_idnt FROM Person WHERE ps_idnt=@idnt) BEGIN INSERT INTO Person(ps_name, ps_gender, ps_dob, ps_added_by, ps_estimate, ps_notes) output INSERTED.ps_idnt VALUES (@name, @gender, @dob, @actor, @estimate, @notes) END ELSE BEGIN UPDATE Person SET ps_name=@name, ps_gender=@gender, ps_dob=@dob, ps_estimate=@estimate, ps_notes=@notes output INSERTED.ps_idnt WHERE ps_idnt=@idnt END");

            return person;
        }

        public PersonAddress SavePersonAddress(PersonAddress address) {
            SqlServerConnection conn = new SqlServerConnection();
            address.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + address.Id + ", @default BIT='" + address.Default + "', @person INT=" + address.Person.Id + ", @telephone NVARCHAR(250)='" + address.Telephone + "', @email NVARCHAR(250)='" + address.Email + "', @location NVARCHAR(250)='" + address.Location + "', @user INT=" + address.AddedBy.Id + ", @notes NVARCHAR(250)='" + address.Notes + "'; IF NOT EXISTS (SELECT pa_idnt FROM PersonAddress WHERE pa_idnt=@idnt) BEGIN INSERT INTO PersonAddress(pa_default, pa_person, pa_telephone, pa_email, pa_location, pa_added_by, pa_notes) output INSERTED.pa_idnt VALUES (@default, @person, @telephone, @email, @location, @user, @notes) END ELSE BEGIN UPDATE PersonAddress SET pa_default=@default, pa_person=@person, pa_telephone=@telephone, pa_email=@email, pa_location=@location, pa_notes=@notes output INSERTED.pa_idnt WHERE pa_idnt=@idnt END");

            return address;
        }

        public Patient SavePatient(Patient patient) {
            SqlServerConnection conn = new SqlServerConnection();
            patient.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + patient.Id + ", @identifier NVARCHAR(50)='" + GetPatientIdentifier(patient) + "', @person INT=" + patient.Person.Id + ", @status INT=" + patient.Status.Id + ", @user INT=" + patient.AddedBy.Id + ", @notes NVARCHAR(MAX)='" + patient.Notes + "'; IF NOT EXISTS (SELECT pt_idnt FROM Patient WHERE pt_idnt=@idnt) BEGIN INSERT INTO Patient(pt_identifier, pt_person, pt_status, pt_added_by, pt_notes) output INSERTED.pt_idnt VALUES (@identifier, @person, @status, @user, @notes) END ELSE BEGIN UPDATE Patient SET pt_status=@status, pt_notes=@notes output INSERTED.pt_idnt WHERE pt_idnt=@idnt END");

            return patient;
        }

        public PatientIdentifier SavePatientIdentifier(PatientIdentifier pi) {
            SqlServerConnection conn = new SqlServerConnection();
            pi.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + pi.Id + ", @default BIT='" + pi.Default + "', @patient INT=" + pi.Patient.Id + ", @type INT=" + pi.Type.Id + ", @identifier NVARCHAR(250)='" + pi.Identifier + "', @user INT=" + pi.AddedBy.Id + ", @notes NVARCHAR(MAX)='" + pi.Notes + "'; IF NOT EXISTS (SELECT pi_idnt FROM PatientIdentifier WHERE pi_idnt=@idnt) BEGIN INSERT INTO PatientIdentifier(pi_default, pi_patient, pi_type, pi_identifier, pi_added_by, pi_notes) output INSERTED.pi_idnt VALUES (@default, @patient, @type, @identifier, @user, @notes) END ELSE BEGIN UPDATE PatientIdentifier SET pi_default=@default, pi_type=@type, pi_identifier=@identifier, pi_notes=@notes output INSERTED.pi_idnt WHERE pi_idnt=@idnt END");

            return pi;
        }        
    }
}
