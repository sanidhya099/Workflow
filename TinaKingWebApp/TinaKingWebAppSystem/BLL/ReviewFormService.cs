using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaKingSystem.DAL;
using TinaKingSystem.Entities;
using TinaKingSystem.ViewModels;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TinaKingSystem.BLL
{
    public class ReviewFormService
    {
        private readonly WFS_2590Context _WFS_2590Context;
        public string SuccessMessage { get; set; }

        internal ReviewFormService(WFS_2590Context wfs_2590Context)
        {
            _WFS_2590Context = wfs_2590Context;
        }

        public WPSView GetByPackageID(int packageID)
        {
            try
            {
                var existWPS = _WFS_2590Context.WPs.FirstOrDefault(x => x.PackageID == packageID);

                if (existWPS == null)
                {
                    return null;
                }

                WPSView ret = new WPSView();
               
                if (ret == null)
                {
                    throw new NullReferenceException();
                }
                ret.WPSID = existWPS.WPSFormID;
                ret.WPSFormID = existWPS.WPSFormID;
                ret.PackageID = (int)existWPS.PackageID;

                ret.WPSNo = existWPS.WPSNo;
                ret.PQRNumber = existWPS.PQRNumber;
                ret.EmployeeID = (int)existWPS.EmployeeID;
                ret.CompanyName = existWPS.CompanyName;
                ret.Date = existWPS.Date;
                ret.fromString(existWPS.Detail);

                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task RegisterWPSView(WPSView wpsView)
        {
            var existingWPS = _WFS_2590Context.WPs.FirstOrDefault(x => (int)x.PackageID == wpsView.PackageID);

            if (existingWPS != null)
            {
                UpdateExistingWPSEx(existingWPS, wpsView);
            }
            else
            {
                var newWPS = getNewWPSEx(wpsView);
                _WFS_2590Context.WPs.Add(newWPS);
            }
            _WFS_2590Context.SaveChanges();
        }


        private WP getNewWPSEx(WPSView wpsView)
        {
            WP ret = new WP
            {
                PackageID = wpsView.PackageID,
                WPSNo = wpsView.WPSNo,
                PQRNumber = wpsView.PQRNumber,
                EmployeeID = wpsView.EmployeeID,
                CompanyName = wpsView.CompanyName,
                Date = (DateTime)wpsView.Date,
                Detail = wpsView.toString()
            };
            return ret;
        }

        private void UpdateExistingWPSEx(WP existingWPS, WPSView wpsView)
        {
            existingWPS.PackageID = wpsView.PackageID;
            existingWPS.WPSNo = wpsView.WPSNo;
            existingWPS.PQRNumber = wpsView.PQRNumber;
            existingWPS.EmployeeID = wpsView.EmployeeID;
            existingWPS.CompanyName = wpsView.CompanyName;
            existingWPS.Date = (DateTime)wpsView.Date;
            existingWPS.Detail = wpsView.toString();
        }
    }
}