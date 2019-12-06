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
        public RegistrationController(IPatientService service)
        {
            IService = service;
        }

        [Route("/registration")]
        public IActionResult Register(RegistrationViewModel model)
        {
            return View(model);
        }






        public JsonResult SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "") {
            return Json(IService.SearchPatients(names, identifier, phone, age, gender, visit));
        }
    }
}
