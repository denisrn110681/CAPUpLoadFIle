using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CAPServer.Context;
using CAPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Cors;

namespace CAPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        public IActionResult GetFiles([FromServices] DataContext context)
        {
            try
            {
                var files = context.DeliveredFile.ToList();
                var _countFile =
                from filesGroup in context.DeliveredFile
                group filesGroup by filesGroup.FileName into _count
                select new
                {
                    FileName = _count.Key,
                    Count = _count.Count(),
                };

                foreach (var item in files)
                {
                    foreach (var itemCount in _countFile)
                    {
                        if (item.FileName == itemCount.FileName)
                        {
                            item.TotalValue = itemCount.Count;
                        }
                    }
                }
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

      
        public IActionResult GetFile(Guid id, [FromServices] DataContext context)
        {
            try
            {
                var files = context.DeliveredFile.Where(x => x.Id == id).ToList();
                var _countFile =
               from filesGroup in context.DeliveredFile
               group filesGroup by filesGroup.FileName into _count
               select new
               {
                   FileName = _count.Key,
                   Count = _count.Count(),
               };

                foreach (var itemCount in _countFile)
                {
                    files[0].TotalValue = itemCount.Count;
                }


                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
