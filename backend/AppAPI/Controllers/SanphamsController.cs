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
using AppData.IService_Admin;
using AppData.Dto_Admin;
using SanphamDTO = AppData.Dto_Admin.SanphamDTO;
using AppData.ViewModel;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanphamsController : ControllerBase
    {
        private readonly ISanPhamService _service;
		private readonly ISanPhamservice _service1;

		public SanphamsController(ISanPhamService service, ISanPhamservice service1)
        {
            _service = service;
            _service1 = service1;

        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sanpham = await _service.GetByIdAsync(id);
            return sanpham != null ? Ok(sanpham) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppData.DTO.SanphamDTO sanphamDto)
        {
            await _service.AddAsync(sanphamDto);
            return CreatedAtAction(nameof(GetById), new { id = sanphamDto.Id }, sanphamDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AppData.DTO.SanphamDTO sanphamDto)
        {
            await _service.UpdateAsync(id, sanphamDto);
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> UpdateStatusToCancelled(int id)
        {
            try
            {
                await _service.UpdateStatusToCancelled(id);
                return NoContent(); // Thành công mà không cần trả về nội dung
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/update-status-load")]
        public async Task<IActionResult> UpdateStatusload(int id)
        {
            try
            {
                await _service.UpdateStatusLoad(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _service.SearchByNameAsync(name));

        [HttpGet("GetALLSanPham")]
        public async Task<IActionResult> GetAllSanphams()
        {
            try
            {
                var sanphamViewModels = await _service.GetAllSanphamViewModels();
                return Ok(sanphamViewModels);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ chung nếu có lỗi khác
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetALLSanPham/{id}")]
        public async Task<IActionResult> GetAllSanphamsByIdSP(int id)
        {
            try
            {
                var sanphamViewModels = await _service.GetAllSanphamViewModelsByIdSP(id);
                return Ok(sanphamViewModels);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetALLSanPhamGiamGia")]
        public async Task<IActionResult> GetAllSanphamsGiamGia()
        {
            try
            {
                var sanphamViewModels = await _service.GetAllSanphamGiamGiaViewModels();
                return Ok(sanphamViewModels);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ chung nếu có lỗi khác
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetALLSanPhamByThuongHieu/{id}")]
        public async Task<IActionResult> GetAllSanphamsByThuongHieu(int id)
        {
            try
            {
                var sanphamViewModels = await _service.GetAllSanphamByThuongHieu(id);
                return Ok(sanphamViewModels);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SanPhamChiTiet/search")]
        public async Task<IActionResult> SearchSanphams(
            
            [FromQuery] decimal? giaMin = null,
            [FromQuery] decimal? giaMax = null,
            [FromQuery] List<int> idThuongHieu = null,
            [FromQuery] List<int> idSize = null,
            [FromQuery] bool? coSale = null)
        {
            try
            {
                
                var sanphams = await _service.GetSanphamByThuocTinh(giaMin, giaMax, idThuongHieu, idSize, coSale);
                if (sanphams == null || !sanphams.Any())
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm nào thỏa mãn tiêu chí." });
                }

                return Ok(sanphams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }
		[HttpGet("Admin")]
		public async Task<IActionResult> GetAllAdmin() => Ok(await _service1.GetAllAsync());

		[HttpGet("{id}/Admin")]
		public async Task<IActionResult> GetByIdAdmin(int id)
		{
			var sanpham = await _service1.GetByIdAsync(id);
			return sanpham != null ? Ok(sanpham) : NotFound();
		}
		[HttpGet("active-products-with-attributes/Admin")]
		public async Task<IActionResult> GetActiveProductsWithAttributes(int id)
		{
			var products = await _service1.GetAllActiveProductsWithAttributesAsync(id);
			return Ok(products);
		}
		//[HttpPost("Admin")]
		//public async Task<IActionResult> Add1(AppData.Dto_Admin.SanphamDTO sanphamDto)
		//{
		//	// Kiểm tra tính hợp lệ của model
		//	if (!ModelState.IsValid)
		//	{
		//		var errorMessages = ModelState.Values
		//						   .SelectMany(v => v.Errors)
		//						   .Select(e => e.ErrorMessage)
		//						   .ToList();

		//		return BadRequest(errorMessages);
		//	}

		//	await _service1.AddAsync(sanphamDto);
		//	return CreatedAtAction(nameof(GetById), new { id = sanphamDto.Tensp }, sanphamDto);
		//}

		[HttpGet("details/Admin")]
		public async Task<ActionResult<IEnumerable<SanphamDetailDto>>> GetSanphamDetails()
		{
			var sanphamDetails = await _service1.GetSanphamDetailsAsync();
			return Ok(sanphamDetails);
		}
		[HttpPut("{id}/add-soluong/Admin")]
		public async Task<IActionResult> AddSoluong(int id, [FromBody] AddSoluongDto addSoluongDto)
		{
			if (!ModelState.IsValid)
			{
				var errorMessage = ModelState.Values
				 .SelectMany(v => v.Errors)
				 .Select(e => e.ErrorMessage)
				 .FirstOrDefault();

				// Trả về BadRequest với thông báo lỗi đơn
				return BadRequest(errorMessage);
			}

			var result = await _service1.AddSoluongAsync(id, addSoluongDto.SoluongThem);
			if (!result)
			{
				return NotFound(new { message = $"Sản phẩm với ID {id} không được tìm thấy." });
			}

			return NoContent(); // HTTP 204 No Content
		}
		//[HttpPut("{id}/Admin")]
		//public async Task<IActionResult> UpdateSanpham(int id, [FromBody] SanphamDTO sanphamDto)
		//{
		//	if (sanphamDto == null)
		//	{
		//		return BadRequest("Dữ liệu sản phẩm không được null.");
		//	}

		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	try
		//	{
		//		var isUpdated = await _service1.UpdateAsync(id, sanphamDto);
		//		if (!isUpdated)
		//		{
		//			return NotFound($"Không tìm thấy sản phẩm với ID = {id}.");
		//		}

		//		return Ok(new { message = "Cập nhật sản phẩm thành công." });
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log lỗi nếu cần
		//		return StatusCode(StatusCodes.Status500InternalServerError,
		//			$"Đã xảy ra lỗi trong quá trình cập nhật sản phẩm: {ex.Message}");
		//	}
		//}
		[HttpPut("{id}/cancel/Admin")]
		public async Task<IActionResult> UpdateStatusToCancelledAdmin(int id)
		{
			try
			{
				await _service1.UpdateStatusToCancelled(id);
				return NoContent(); // Thành công mà không cần trả về nội dung
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPut("{id}/update-status-load/Admin")]
		public async Task<IActionResult> UpdateStatusloadAdmin(int id)
		{
			try
			{
				await _service1.UpdateStatusLoad(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
		
		[HttpDelete("{id}/Admin")]
		public async Task<IActionResult> DeleteAdmin(int id)
		{
			await _service1.DeleteAsync(id);
			return NoContent();
		}

		[HttpGet("search/Admin")]
		public async Task<IActionResult> SearchByNameAdmin(string name) => Ok(await _service1.SearchByNameAsync(name));
		[HttpGet("searchhd/Admin")]
		public async Task<IActionResult> SearchByNameHd(string name) => Ok(await _service1.SearchByNameHdAsync(name));
		[HttpGet("tong-trangthai-0-1")]
		public async Task<IActionResult> TongTrangThai01()
		{
			int tong = await _service1.GetTongTrangThai0or1Async();
			return Ok(tong);       
		}
	}

}

