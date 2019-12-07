using System;
using System.Globalization;
using System.Security.Claims;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Service;
using AfyaHMIS.ViewModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly IPatientService IService;

        [BindProperty]
        public RegistrationNewEditViewModel InputModel { get; set; }

        public RegistrationController(IPatientService iservice) {
            IService = iservice;
        }

        [Route("/registration")]
        public IActionResult Register(RegistrationRegisterViewModel model) {
            return View(model);
        }

        [Route("/registration/new")]
        public IActionResult New(RegistrationNewEditViewModel model, string name = "", string id = "", string phone = "", string age = "", string gender = "") {
            if (!string.IsNullOrEmpty(name))
                model.Patient.Person.Name = name;
            if (!string.IsNullOrEmpty(id))
                model.Patient.PI.Identifier = id;
            if (!string.IsNullOrEmpty(gender))
                model.Patient.Person.Name = gender;
            if (!string.IsNullOrEmpty(phone))
                model.Patient.Person.Address.Telephone = phone;
            if (!string.IsNullOrEmpty(age) && Double.TryParse(age, out double dblAge) && Double.Parse(age) > 0)
                model.DateOfBirth = DateTime.Now.AddYears(0 - Int32.Parse(age)).ToString("dd/MM/yyyy");

            model.IdType = IService.GetPatientIdentificationTypes();

            return View(model);
        }

        [Route("/registration/edit")]
        public IActionResult Edit(string p, RegistrationNewEditViewModel model) {
            model.Patient.Uuid = p;
            return View(model);
        }

        [Route("/registration/intake/")]
        public IActionResult Intake(string p, RegistrationIntakeViewModel model) {
            model.Patient.Uuid = p;
            return View(model);
        }

        public JsonResult SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "") {
            return Json(IService.SearchPatients(names, identifier, phone, age, gender, visit));
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator, Super User, Regional Admin, Agency Admin, Facility Admin")]
        public IActionResult RegisterNewPatient() {
            DateTime dob = DateTime.Now;

            try {
                dob = DateTime.ParseExact(InputModel.DateOfBirth, "MMM dd, yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) {
                System.Diagnostics.Debug.WriteLine("...");
            }

            Users user = new Users { Id = Int64.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) };
            Patient patient = InputModel.Patient;
            patient.Person.DateOfBirth = dob;
            patient.Person.AddedBy = user;
            patient.Person.Address.AddedBy = user;
            patient.AddedBy = user;
            patient.PI.AddedBy = user;
            patient.Save(IService);

            return LocalRedirect("/registration/intake?p=" + patient.GetUuid());
        }

        [AllowAnonymous]
        public string GetDateFromString(string value) {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            try             {
                return DateTime.ParseExact(value, "MMM dd, yyyy", CultureInfo.InvariantCulture).ToString("MMM dd, yyyy");
            }
            catch (Exception) {
                System.Diagnostics.Debug.WriteLine("...");
            }

            if (IsNumber(value)) {
                return DateTime.Now.AddYears(0 - int.Parse(value)).ToString("MMM dd, yyyy");
            }

            if (value.Contains('y')) {
                value = value.Replace("y", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddYears(0 - int.Parse(value)).ToString("MMM dd, yyyy");
                else
                    return "";
            }
            else if (value.Contains('m')) {
                value = value.Replace("m", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddMonths(0 - int.Parse(value)).ToString("MMM dd, yyyy");
                else
                    return "";
            }
            else if (value.Contains('w')) {
                value = value.Replace("w", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddDays(0 - (int.Parse(value) * 7)).ToString("MMM dd, yyyy");
                else
                    return "";
            }
            else if (value.Contains('d')) {
                value = value.Replace("d", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddDays(0 - int.Parse(value)).ToString("MMM dd, yyyy");
                else
                    return "";
            }

            return "";
        }

        public bool IsNumber(string s) {
            bool value = true;
            foreach (char c in s) { 
                value = value && Char.IsDigit(c);
            }

            return value;
        }
    }
}
