using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DeveloperCase.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using System.Globalization;
using Syncfusion.XlsIO;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Threading;

namespace DeveloperCase.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment hostingEnv;
        private readonly ILogger<HomeController> _logger;
        private SalesContext _context;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, SalesContext context)
        {
            _logger = logger;
            this.hostingEnv = env;
            this._context = context;
        }

        public IActionResult Index()
        {
            ViewBag.dateformat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            ViewBag.SearchRange = string.Empty;
            var data = _context.ViewTotalProfit.FromSqlRaw("EXECUTE spSalesTotalProfitsGetAll {0},{1}", DBNull.Value, DBNull.Value).AsEnumerable().FirstOrDefault();
            if (data != null)
            {
                ViewBag.TotalProfit = data.TotalProfit;
            }
            else
            {
                ViewBag.TotalProfit = 0;
            }
            ViewBag.WithMostProfits = _context.ViewSalesProfits.FromSqlRaw("EXECUTE spTopSalesGetAll {0},{1}", DBNull.Value, DBNull.Value).AsEnumerable().ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Index(SearchPanel value)
        {
            ViewBag.dateformat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            if (!ModelState.IsValid)
            {
                ViewBag.SearchRange = string.Empty;
                var _data = _context.ViewTotalProfit.FromSqlRaw("EXECUTE spSalesTotalProfitsGetAll {0},{1}", DBNull.Value, DBNull.Value).AsEnumerable().FirstOrDefault();
                if (_data != null)
                {
                    ViewBag.TotalProfit = _data.TotalProfit;
                }
                else
                {
                    ViewBag.TotalProfit = 0;
                }
                ViewBag.WithMostProfits = _context.ViewSalesProfits.FromSqlRaw("EXECUTE spTopSalesGetAll {0},{1}", DBNull.Value, DBNull.Value).AsEnumerable().ToList();
                return View();
            }


            ViewBag.SearchRange = "Showing Summary From:- " + value.StartDate.ToLongDateString() + " To:- " + value.EndDate.ToLongDateString();
            var data = _context.ViewTotalProfit.FromSqlRaw("EXECUTE spSalesTotalProfitsGetAll {0},{1}", value.StartDate, value.EndDate).AsEnumerable().FirstOrDefault();
            if (data != null)
            {
                ViewBag.TotalProfit = data.TotalProfit; 
            }
            else
            {
                ViewBag.TotalProfit = 0;
            }
            ViewBag.WithMostProfits = _context.ViewSalesProfits.FromSqlRaw("EXECUTE spTopSalesGetAll {0},{1}", value.StartDate, value.EndDate).AsEnumerable().ToList();
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Sales()
        {
            return View();
        }
        public IActionResult BatchUpload()
        {
            return View();
        }
        [AcceptVerbs("Post")]
        public IActionResult SaveFile(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fileSavePath = hostingEnv.WebRootPath + "\\UploadedFiles";
                        string _filename = fileSavePath + $@"\{filename}";
                        if (!System.IO.File.Exists(_filename))
                        {
                            using (FileStream fs = System.IO.File.Create(_filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            _filename = fileSavePath+$@"\{DateTime.Now.Hour+ DateTime.Now.Minute+DateTime.Now.Second+filename}";
                            //System.IO.File.Delete(filename);
                            using (FileStream fs = System.IO.File.Create(_filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                        }
                        ImportSpreadsheetData(_filename);
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }
        [AcceptVerbs("Post")]
        public IActionResult RemoveFile(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = hostingEnv.WebRootPath + "\\UploadedFiles";
                    var fileSavePath = filePath + "\\" + fileName;
                    if (!System.IO.File.Exists(fileSavePath))
                    {
                        System.IO.File.Delete(fileSavePath);
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }
        private void ImportSpreadsheetData(string file)
        {
            Stream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            string format = "dd/MM/yyyy";
            CultureInfo current = new CultureInfo("en-US");
            //string filePath = file;

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            IWorkbook workbook = application.Workbooks.Open(fileStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            if (
                     (worksheet.Range["A1"].Value.Contains("Region")) &&
                     (worksheet.Range["B1"].Value.Contains("Country")) &&
                     (worksheet.Range["C1"].Value.Contains("Item Type")) &&
                     (worksheet.Range["D1"].Value.Contains("Sales Channel")) &&
                     (worksheet.Range["E1"].Value.Contains("Order Priority")) &&
                     (worksheet.Range["F1"].Value.Contains("Order Date")) &&
                     (worksheet.Range["G1"].Value.Contains("Order ID")) &&
                     (worksheet.Range["H1"].Value.Contains("Ship Date")) &&
                     (worksheet.Range["I1"].Value.Contains("Units Sold")) &&
                     (worksheet.Range["J1"].Value.Contains("Unit Price")) &&
                     (worksheet.Range["K1"].Value.Contains("Unit Cost")) &&
                     (worksheet.Range["L1"].Value.Contains("Total Revenue")) &&
                     (worksheet.Range["M1"].Value.Contains("Total Cost")) &&
                     (worksheet.Range["N1"].Value.Contains("Total Profit")))
            {
                List<Sales> _salesList = new List<Sales>();
                for (int row = 2; row <= worksheet.UsedRange.LastRow; row++)
                {
                    Sales _sales = new Sales();
                    if (worksheet.IsRowVisible(row))
                    {
                        for (int column = 1; column <= worksheet.UsedRange.LastColumn; column++)
                        {
                            switch (column)
                            {
                                case 1: // Region
                                    string region = worksheet.Range[row, column].Value.ToString();
                                    _sales.Region = region;
                                    break;
                                case 2: // Country
                                    string country = worksheet.Range[row, column].Value.ToString();
                                    _sales.Country = country;
                                    break;
                                case 3: // ItemType
                                    string itemtype = worksheet.Range[row, column].Value.ToString();
                                    _sales.ItemType = itemtype;
                                    break;
                                case 4: // salesChannel
                                    string salesChannel = worksheet.Range[row, column].Value.ToString();
                                    _sales.SalesChannel = salesChannel;
                                    break;
                                case 5: // OrderPriority
                                    string OrderPriority = worksheet.Range[row, column].Value.ToString();
                                    _sales.OrderPriority = OrderPriority;
                                    break;
                                case 6: // OrderDate
                                    string _OrderDate = worksheet.Range[row, column].Value.ToString();
                                    var _m = _OrderDate.Split('/');
                                    if (Convert.ToInt32(_m[1]) > 12)
                                    {
                                        _OrderDate = _m[1] + "/" + _m[0] + "/" + _m[2];
                                    }
                                    string OrderDate = Convert.ToDateTime(_OrderDate).ToString(format);
                                    DateTime OrderdateTime = new DateTime();
                                    if (DateTime.TryParseExact(OrderDate, format, current, DateTimeStyles.None, out OrderdateTime))
                                    {
                                        _sales.OrderDate = OrderdateTime;
                                    }
                                    break;
                                case 7: // OrderId
                                    double OrderId = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.OrderId = OrderId;
                                    break;
                                case 8: // ShipDate
                                    string xShipDate = worksheet.Range[row, column].Value.ToString();
                                    var m = xShipDate.Split('/');
                                    if(Convert.ToInt32(m[1]) > 12)
                                    {
                                        xShipDate = m[1] + "/" + m[0] + "/" + m[2];
                                    }
                                    string ShipDate = Convert.ToDateTime(xShipDate).ToString(format);
                                    DateTime ShipDateTime = new DateTime();
                                    if (DateTime.TryParseExact(ShipDate, format, current, DateTimeStyles.None, out ShipDateTime))
                                    {
                                        _sales.ShipDate = ShipDateTime;
                                    }
                                    break;
                                case 9: // UnitSold
                                    double UnitSold = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.UnitSold = UnitSold;
                                    break;
                                case 10: // UnitPrice
                                    double UnitPrice = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.UnitPrice = UnitPrice;
                                    break;
                                case 11: // UnitCost
                                    double UnitCost = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.UnitCost = UnitCost;
                                    break;
                                case 12: // TotalRevenue
                                    double TotalRevenue = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.TotalRevenue = TotalRevenue;
                                    break;
                                case 13: // TotalCost
                                    double TotalCost = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.TotalCost = TotalCost;
                                    break;
                                case 14: // TotalProfit
                                    double TotalProfit = Convert.ToDouble(worksheet.Range[row, column].Value.ToString().Trim());
                                    _sales.TotalProfit = TotalProfit;
                                    break;
                            }
                        }

                        _salesList.Add(_sales);
                        
                    }
                }
                if(_salesList.Count > 0)
                {
                    var remainingData = _salesList.Where(
                            o => _context.Sales.Any(x => x.OrderId == o.OrderId) == false
                        );

                    _context.Sales.AddRange(remainingData);
                    _context.SaveChanges();
                    //var exists = _context.Sales.FirstOrDefault(o => o.OrderId == _sales.OrderId);
                    //if(exists== null)
                    //{
                    //    _context.Sales.Add(_sales);

                    //}
                    //else
                    //{
                    //    _context.Entry(exists).CurrentValues.SetValues(_sales);
                    //    _context.Entry(exists).State = EntityState.Modified;
                    //}
                    //_context.SaveChanges();
                }
            }

        }
        public ActionResult SalesDataSource([FromBody] DataManagerRequest dm)
        {
            var _data = _context.Sales.OrderByDescending(o=> o.OrderDate).ToList();
            IEnumerable data = _data;
            int _count = _data.Count;

            DataOperations operation = new DataOperations();
            //Performing filtering operation
            if (dm.Where != null)
            {
                data = operation.PerformFiltering(data, dm.Where, "and");
                var filtered = (IEnumerable<object>)data;
                _count = filtered.Count();
            }
            //Performing search operation
            if (dm.Search != null)
            {
                data = operation.PerformSearching(data, dm.Search);
                var searched = (IEnumerable<object>)data;
                _count = searched.Count();
            }
            //Performing sorting operation
            if (dm.Sorted != null)
                data = operation.PerformSorting(data, dm.Sorted);

            //Performing paging operations
            if (dm.Skip > 0)
                data = operation.PerformSkip(data, dm.Skip);
            if (dm.Take > 0)
                data = operation.PerformTake(data, dm.Take);

            return Json(new { result = data, count = _count });

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
