﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Employee_Schedule.Models;

namespace Employee_Schedule.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        public ActionResult Index()
        {
            return View();
        }

        //Method to pull Events from database
        public JsonResult GetEvents()
        {
            Employee_Schedule_DatabaseEntities db = new Employee_Schedule_DatabaseEntities();

            db.Configuration.ProxyCreationEnabled = false;
            var events = db.Events.ToList();
             //Tempory holder for resrouces
            List<resourceModel> resList = new List<resourceModel>();


            foreach(var evnt in events)
            {
                //Finds Employee Linked to Event
                Employee tempEmp = new Employee();
                tempEmp = db.Employees.Where(c => c.EmployeeID == evnt.EmployeeID).FirstOrDefault();

                //Saves All information about event to temporary result model
                resourceModel res = new resourceModel();

                res.title = tempEmp.FirstName + " " + tempEmp.LastName;
                res.EventID = evnt.EventID;
                res.Start = evnt.Start;
                res.IsFullDay = evnt.IsFullDay;
                res.ThemeColor = evnt.ThemeColor;
                res.EmployeeID = tempEmp.EmployeeID;
                

                //Adding text to event description depending on how long employee works that day
                if (res.IsFullDay)
                {
                    res.Description = tempEmp.Occupation + ", works full time today";
                }
                else
                {
                    res.End = evnt.End;
                    TimeSpan? timeDif = res.End- res.Start;
                    res.Description = tempEmp.Occupation + ", works " + timeDif.Value.TotalHours + " hours today.";
                }
                resList.Add(res);
            }

                return new JsonResult { Data = resList, JsonRequestBehavior = JsonRequestBehavior.AllowGet}; 
        }

        //Method to pull resources (employees) from database.
        public JsonResult GetResources()
        {
            Employee_Schedule_DatabaseEntities db = new Employee_Schedule_DatabaseEntities();

            db.Configuration.ProxyCreationEnabled = false;
            var resources = db.Employees.ToList();

            //Tempory holder for resrouces
            List<resourceModel> resList = new List<resourceModel>();


            foreach(var emp in resources)
            {
                resourceModel res = new resourceModel();
                res.title = emp.FirstName + " " + emp.LastName;
                res.id = emp.EmployeeID;
                res.groupId = emp.Occupation;

                resList.Add(res);

            }

                return new JsonResult { Data = resList, JsonRequestBehavior = JsonRequestBehavior.AllowGet};

            
        }

        //Method to Save New or edited event
        [HttpPost]
        public JsonResult SaveEvent(Event evnt)
        {
            Employee_Schedule_DatabaseEntities db = new Employee_Schedule_DatabaseEntities();
            db.Configuration.ProxyCreationEnabled = false;
            var status = false;

            //If the event ID is bigger than zero its a existing event.
            if (evnt.EventID > 0)
            {
                //Grabs event with given ID from the database
                var oldEvent = db.Events.Where(a => a.EventID == evnt.EventID).FirstOrDefault();

                if (oldEvent != null)
                {
                    //Replaces fields that has been updated
                    oldEvent.EventID = evnt.EventID;
                    oldEvent.EmployeeID = evnt.EmployeeID;
                    oldEvent.Start = evnt.Start;
                    oldEvent.End = evnt.End;
                    oldEvent.IsFullDay = evnt.IsFullDay;
                    oldEvent.Description = evnt.Description; 
                }
            }
            //If a new event is added, it just adds the new event to DB
            else
            {

                db.Events.Add(evnt);
            }

            db.SaveChanges();
            status = true;

            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int evntID)
        {
            Employee_Schedule_DatabaseEntities db = new Employee_Schedule_DatabaseEntities();
            db.Configuration.ProxyCreationEnabled = false;
            var status = false;

            //Finds event by ID which should be deleted
            var evnt = db.Events.Where(a => a.EventID == evntID).FirstOrDefault();

            if (evnt != null)
            {
                //Removes event from DB
                db.Events.Remove(evnt);
                db.SaveChanges();
                status = true;

            }

            return new JsonResult{ Data = new {status=status}};
            
        }
}


    }
