using DBAccess.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Impl.AdoJobStore;

namespace ERPCronJob.Controllers
{
    public class RequeryController : Controller
    {
        private IRequeryService _requeryService;

        public RequeryController(IRequeryService data)
        {
            _requeryService = data;
        }
        // GET: RequeryController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RequeryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RequeryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequeryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RequeryController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RequeryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RequeryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequeryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
