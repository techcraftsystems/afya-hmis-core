﻿using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Patients
{
    public class PatientIdentifier {
        public long Id { get; set; }
        public bool Default { get; set; }
        public Patient Patient { get; set; }
        public PatientIdentifierType Type { get; set; }
        public string Identifier { get; set; }
        public DateTime AddedOn { get; set; }
        public Users AddedBy { get; set; }
        public string Notes { get; set; }

        public PatientIdentifier() {
            Id = 0;
            Default = true;
            Type = new PatientIdentifierType();
            Identifier = "";
            AddedOn = DateTime.Now;
            AddedBy = new Users();
            Notes = "";
        }

        public PatientIdentifier Save(IPatientService IService) {
            return IService.SavePatientIdentifier(this);
        }
    }
}
