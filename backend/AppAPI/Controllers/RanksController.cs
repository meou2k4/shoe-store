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

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RanksController : ControllerBase
    {
        private readonly IRankService _service;
        public RanksController(IRankService service)
        {
            _service = service;

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _service.GetRankByIdAsync(id);
            if (x == null) return NotFound();

            return Ok(new
            {
                x.Tenrank,
                x.MaxMoney,
                x.MinMoney,
                Trangthai = x.Trangthai == 0 ? "Đang hoạt động" : "Dừng hoạt động"
            });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RankDTO rankDTO)
        {


            try
            {
                await _service.AddRankDTOAsync(rankDTO);
                return CreatedAtAction(nameof(GetById), new { id = rankDTO.Tenrank }, rankDTO); // Trả về trạng thái 201 Created
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message }); // Trả về thông điệp lỗi chung
            }
        }

        // Cập nhật nhà cung cấp theo ID
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RankDTO rankDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Trả về lỗi nếu model không hợp lệ

            try
            {
                await _service.UpdateRankAsync(id, rankDTO);
                return NoContent(); // Trả về trạng thái 204 No Content nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message }); // Trả về thông điệp lỗi chung
            }
        }

        // Xóa nhà cung cấp theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteRankAsync(id);
                return NoContent(); // Trả về trạng thái 204 No Content nếu xóa thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message }); // Trả về thông điệp lỗi chung
            }
        }

        // Lấy tất cả nhà cung cấp
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllRanksAsync();
            return Ok(result);
        }
    }
}
