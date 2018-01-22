﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Catalogue.Models.Tables;
using System.Net;
using System.Data.Entity;
using PagedList;
using System.IO;

namespace Catalogue.Controllers.CRUD
{
    public class EmployeeController : Controller
    {
        CatalogueContext db = new CatalogueContext();

        public ActionResult AjaxPositionList(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.Employees.Include(e => e.Department).Include(p => p.Position).OrderBy(i => i.EmployeeFullName).ToPagedList(pageNumber, pageSize));
        }

        // GET: Employee
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(db.Employees.Include(e => e.Department).Include(p => p.Position).OrderBy(i => i.EmployeeFullName).ToPagedList(pageNumber, pageSize));
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = db.Employees.Include(p => p.Position).Include(d => d.Department).SingleOrDefault(e => e.EmployeeId == id);
            return View(employee);
        }

        [HttpGet]
        // GET: Employee/Create
        public ActionResult Create()
        {
            SelectList departmentList = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.DepartmentList = departmentList;
            SelectList positionList = new SelectList(db.Positions, "PositionId", "PositionName");
            ViewBag.PositionList = positionList;
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee collection, HttpPostedFileBase productImg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productImg != null)
                    {
                        var fileName = Path.GetFileName(productImg.FileName);
                        var directoryToSave = Server.MapPath(Url.Content("~/images"));

                        var pathToSave = Path.Combine(directoryToSave, fileName);
                        productImg.SaveAs(pathToSave);
                        collection.EmployeePhoto = fileName;
                    }
                    db.Employees.Add(collection);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(collection);  
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = db.Employees.Find(id);
            if (employee == null)
                return HttpNotFound();

            SelectList departmentList = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.DepartmentList = departmentList;
            SelectList positionList = new SelectList(db.Positions, "PositionId", "PositionName");
            ViewBag.PositionList = positionList;
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Employee collection, HttpPostedFileBase productImg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productImg == null)
                    {
                        var fileName = Path.GetFileName(productImg.FileName);                     
                        //var fileName = Path.GetFileName(db.Employees.Find(id).EmployeePhoto);
                        
                        string path = Path.Combine(Server.MapPath(Url.Content("~/images")), fileName);
                        productImg.SaveAs(path);

                        db.Entry(collection).State = EntityState.Added;
                    }
                    else if (productImg != null)
                    {
                        var fileName = Path.GetFileName(productImg.FileName);

                        var directoryToSave = Server.MapPath(Url.Content("~/images"));

                        var pathToSave = Path.Combine(directoryToSave, fileName);
                        productImg.SaveAs(pathToSave);
                        collection.EmployeePhoto = fileName;
                    }
                }
                db.Entry(collection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
                
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = db.Employees.Include(p => p.Position).Include(d => d.Department).SingleOrDefault(e => e.EmployeeId == id);
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id, Employee collection)
        {
            Employee employee = new Employee();
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                employee = db.Employees.Find(id);
                if (employee == null)
                    return HttpNotFound();
                db.Employees.Remove(employee);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}