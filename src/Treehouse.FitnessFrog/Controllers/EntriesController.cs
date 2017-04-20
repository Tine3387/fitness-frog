using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
                
            };

            ViewBag.ActivitiesSelectListItems = new SelectList(
                Data.Data.Activities, "Id", "Name");

            return View(entry);
        }

        /* Instead of specifying a parameter for each request form field that we need to capture, */
        /* we can simply use a parameter of type entry named Entry. */
        /* It is possible because MVCs model binder will recognize that our parameter is an instance of a class or */
        /* reference type instead of a value type like string, int double, bool, or datetime */
        /* and attempt to bind the incoming form field values to its properties. */
        /* As long as the field names match the classes property names */
        /* the entry object's properties will contain the expected values. */
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            /* If there aren't any "Duration" field validation errors */
            /* then make sure that the duration is greater than "0". */
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }

            /* If the model state is valid, we can save the entry data model instance */
            /* by calling the repositories at entry method. */
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                /* post/redirect/get design pattern to prevent duplicate form submissions */
                return RedirectToAction("Index");
            }

            ViewBag.ActivitiesSelectListItems = new SelectList(
                Data.Data.Activities, "Id", "Name");

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }
    }
}