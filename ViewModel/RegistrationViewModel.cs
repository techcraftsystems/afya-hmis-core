using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.ViewModel
{
    public class RegistrationRegisterViewModel
    {
        public RegistrationRegisterViewModel()
        {
        }
    }

    public class RegistrationNewEditViewModel
    {
        public string DateOfBirth { get; set; }
        public Patient Patient { get; set; }
        public IEnumerable<SelectListItem> Gender { get; set; }
        public IEnumerable<SelectListItem> IdType { get; set; }

        public RegistrationNewEditViewModel() {
            DateOfBirth = "";
            Patient = new Patient();
            Gender = new List<SelectListItem> {
                new SelectListItem {Value = "f", Text = "Female" },
                new SelectListItem {Value = "m", Text = "Male" },
                new SelectListItem {Value = "o", Text = "Others" },
            };
            IdType = new List<SelectListItem>();
        }
    }

    public class RegistrationIntakeViewModel
    {
        public Patient Patient { get; set; }

        public RegistrationIntakeViewModel()
        {
            Patient = new Patient();
        }
    }
}
