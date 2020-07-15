using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using CAPServer.Models;
using System.Collections.Generic;
using CAPServer.Context;
using System.Text;
using System.Globalization;
using System.Linq;

namespace CAPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload([FromServices] DataContext context)
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                StringBuilder message = new StringBuilder();
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    List<DeliveredFile> dataList = new List<DeliveredFile>();
                    DeliveredFile data = new DeliveredFile();
                
                    var filesExist = context.DeliveredFile.Where(x => x.FileName == fileName.ToString()).FirstOrDefault();
                    if (filesExist == null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            using (var package = new ExcelPackage(memoryStream))
                            {
                                for (int i = 1; i <= package.Workbook.Worksheets.Count; i++)
                                {
                                    var totalRows = package.Workbook.Worksheets[i].Dimension?.Rows;
                                    var totalCollumns = package.Workbook.Worksheets[i].Dimension?.Columns;
                                    for (int j = 2; j <= totalRows.Value; j++)
                                    {
                                        string _erro = "";
                                        data = new DeliveredFile();
                                        try
                                        {
                                            data.DeliveryDate = Convert.ToDateTime(package.Workbook.Worksheets[1].Cells[j, 1].Value.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            return StatusCode(400, $"Internal server error DeliveryDate {j}: {ex} ");
                                        }

                                        if (data.DeliveryDate <= DateTime.Now)
                                        {
                                            //     return StatusCode(400, $"Internal server error DeliveryDate must be > than today");
                                        }

                                        try
                                        {
                                            string _ProductDescription = "";
                                            _ProductDescription = package.Workbook.Worksheets[1].Cells[j, 2].Value.ToString();
                                            if (_ProductDescription.Length > 50)
                                            {
                                                _erro += "ProductDescription lenght>50 ";
                                            }
                                            else
                                            {
                                                data.ProductDescription = _ProductDescription;
                                            }

                                        }
                                        catch (Exception)
                                        {
                                            _erro += "ProductDescription ";
                                        }
                                        try
                                        {
                                            data.Quantity = Convert.ToInt32(package.Workbook.Worksheets[1].Cells[j, 3].Value.ToString());
                                        }
                                        catch (Exception)
                                        {
                                            _erro += "Quantity ";
                                        }
                                        try
                                        {
                                            double _returnUnit = Convert.ToDouble(package.Workbook.Worksheets[1].Cells[j, 4].Value.ToString());
                                            data.UnitValue = Math.Round(_returnUnit, 2);
                                        }
                                        catch (Exception)
                                        {
                                            _erro += "UnitValue ";
                                        }
                                        try
                                        {
                                            data.TotalValue = data.Quantity * data.UnitValue;
                                        }
                                        catch (Exception)
                                        {
                                            _erro += "TotalValue ";
                                        }
                                        try
                                        {
                                            data.FileName = fileName;
                                        }
                                        catch (Exception)
                                        {
                                            _erro += "FileName ";
                                        }
                                        dataList.Add(data);
                                        if (String.IsNullOrEmpty(_erro) ==false)
                                        {
                                            message.Append("row=" + j + "Error: " + _erro + Environment.NewLine);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message.Append("File already uploaded.");
                        return StatusCode(400, $"Internal server error: {message.ToString()}");
                    };
                    if (message.Length > 0)
                    {
                        return StatusCode(400, $"Internal server error: {message.ToString()}");
                    }
                    else
                    {
                        if (dataList.Count > 0)
                        {
                            context.DeliveredFile.AddRange(dataList);
                            context.SaveChanges();
                        }
                        return Ok(new { Result = dataList });
                    }
                }
                else
                {
                    return StatusCode(400, $"Internal server error: {message.ToString()}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");

            }
        }

    }
}