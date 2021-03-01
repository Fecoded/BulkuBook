﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if(id == null)
            {
                //this is to create coverType
                return View(coverType);
            }
            //this is to edit
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = _unitOfWork.SP_CAll.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                {
                    _unitOfWork.SP_CAll.Execute(SD.Proc_CoverType_Create, parameter);
                   
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_CAll.Execute(SD.Proc_CoverType_Update, parameter);
                }
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }

            return View(coverType);
        }

  

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.SP_CAll.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = result });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var coverType = _unitOfWork.SP_CAll.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter) ;
           
            if (coverType == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.SP_CAll.Execute(SD.Proc_CoverType_Delete, parameter);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Cover Type was deleted " });
        }



        #endregion
    }
}
