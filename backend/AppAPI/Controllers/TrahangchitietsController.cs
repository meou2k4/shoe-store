using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppData;
using AppData.Models;
using AppData.DTO;
using AppData.IService;
using AppData.ViewModel;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrahangchitietsController : ControllerBase
    {
        private readonly ITraHangChiTietService _service;
        public TrahangchitietsController(ITraHangChiTietService ser)
        {
            _service = ser;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _service.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var a = await _service.GetById(id);
                if (a == null) return BadRequest("Không tồn tại");
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(TrahangchitietDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    await _service.Add(dto);

                    return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TrahangchitietDTO dto)
        {
            try
            {
                await _service.Update(id, dto);
                return Ok("Sửa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Ok("Xóa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ListSanPhamByIdth/{id}")]
        public async Task<IActionResult> ListSanPhamByIdth(int id)
        {
            try
            {
                return Ok(await _service.ListSanPhamByIdth(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("SanphamByThct")]
        public async Task<IActionResult> SanphamByThct()
        {

            try
            {
                return Ok(await _service.SanphamByThct());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ListSanPhamByIdhd/{id}")]
        public async Task<IActionResult> ListSanPhamByIdhd(int id)
        {

            try
            {
                return Ok(await _service.ListSanPhamByIdhd(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
