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
using AppData.IRepository;
using AppData.ViewModel;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrahangsController : ControllerBase
    {
        private readonly ITraHangService _ser;
        private readonly ITraHangChiTietService _chiTietService;
        private readonly IHoaDonChiTietService _hdctSer;
        private readonly ISanPhamChiTietService _spctSer;
        private readonly ISanPhamService _spSer;
        private readonly IKhachHangService _khSer;
        public TrahangsController(ITraHangService ser, ITraHangChiTietService chiTietService, IHoaDonChiTietService hdctSer, ISanPhamChiTietService spctSer, ISanPhamService spSer, IKhachHangService khSer)
        {
            _ser = ser;
            _chiTietService = chiTietService;
            _hdctSer = hdctSer;
            _spctSer = spctSer;
            _spSer = spSer;
            _khSer = khSer;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _ser.GetAll());
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
                var a = await _ser.GetById(id);
                if (a == null) return BadRequest("Không tồn tại");
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("thongtinn/{id}")]
        public async Task<IActionResult> Get1(int id)
        {
            try
            {
                var a = await _ser.GetById1(id);
                if (a == null) return BadRequest("Không tồn tại");
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(TrahangtaoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    await _ser.Add(dto);
                    // Chỉ trả về Id sau khi tạo thành công
                    return Ok(new { id = dto.Id });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TrahangtaoDTO dto)
        {
            try
            {
                await _ser.Update(id, dto);
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
                await _ser.DeleteById(id);
                return Ok("Xóa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("HuyDon")]
        public async Task<IActionResult> Huydon(int id, int idnv, string? chuthich)
        {
            try
            {
                await _ser.Huydon(id,idnv,chuthich);
                return Ok("Sửa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Xacnhan")]
        public async Task<IActionResult> XacNhan(int id, string hinhthucxuly, int idnv, string? chuthich)
        {
            try
            {
                await _ser.XacNhan(id, hinhthucxuly, idnv, chuthich);
                return Ok("Sửa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateTrangThaiHd/{id}")]
        public async Task<IActionResult> UpdateTrangThaiHd(int id)
        {
            try
            {
                await _ser.UpdateTrangThaiHd(id);
                return Ok("Sửa thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
