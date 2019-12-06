using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;

namespace AfyaHMIS.Service
{
    public interface IPatientService
    {
        public List<Patient> SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "");
    }

    public class PatientService : IPatientService
    {
        public PatientService(){}

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
    }
}
