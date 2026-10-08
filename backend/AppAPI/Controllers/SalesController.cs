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

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppData.IService.ISaleService _KhachHang_service;
		private readonly AppData.IService_Admin.ISaleService _service;

		public SalesController(AppData.IService.ISaleService service, AppData.IService_Admin.ISaleService service1)
        {
            _KhachHang_service = service;
            _service = service1;
        }
        /*	[HttpGet]
			public async Task<IActionResult> GetAll()
			{
				var sales = await _KhachHang_service.GetAllAsync();
				return Ok(sales.Select(s => new
				{

					s.Ten,
					s.Mota,
					Trangthai = s.Trangthai switch
					{
						0 => "Đang diễn ra",
						1 => "Chuẩn bị diễn ra",
						2 => "Đã diễn ra",
						3 => "Dừng phát hành",
						_ => "Không xác định"
					},
					s.Ngaybatdau,
					s.Ngayketthuc
				}));
			}*/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _KhachHang_service.GetAllWithIdAsync();
            return Ok(sales.Select(s => new
            {
                s.Id, // Thêm Id vào kết quả trả về
                s.Ten,
                s.Mota,
                Trangthai = s.Trangthai switch
                {
                    0 => "Đang diễn ra",
                    1 => "Chuẩn bị diễn ra",
                    2 => "Đã diễn ra",
                    3 => "Dừng phát hành",
                    _ => "Không xác định"
                },
                s.Ngaybatdau,
                s.Ngayketthuc
            }));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _KhachHang_service.GetByIdAsync(id);
            if (sale == null) return NotFound();

            return Ok(new
            {
                sale.Ten,
                sale.Mota,
                Trangthai = sale.Trangthai switch
                {
                    0 => "Đang diễn ra",
                    1 => "Chuẩn bị diễn ra",
                    2 => "Đã diễn ra",
                    3 => "Dừng phát hành",
                    _ => "Không xác định"
                },
                sale.Ngaybatdau,
                sale.Ngayketthuc
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SaleDTO saleDto)
        {
            /*await _KhachHang_service.AddAsync(saleDto);
			//var x = a _KhachHang_service.AddAsync(saleDto);
			return CreatedAtAction(nameof(GetById), new { id = saleDto.Ten }, saleDto);*/
            try
            {


                await _KhachHang_service.AddAsync(saleDto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Chỉ trả ra thông báo lỗi đơn giản
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung (nếu cần), có thể ghi log hoặc xử lý khác
                return StatusCode(500, "Đã xảy ra lỗi, vui lòng thử lại sau.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaleDTO saleDto)
        {
            await _KhachHang_service.UpdateAsync(id, saleDto);
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> UpdateStatusToCancelled(int id)
        {
            try
            {
                await _KhachHang_service.UpdateStatusToCancelled(id);
                return NoContent(); // Thành công mà không cần trả về nội dung
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/update-status")]
        public async Task<IActionResult> UpdateStatusBasedOnDates(int id)
        {
            try
            {
                await _KhachHang_service.UpdateStatusBasedOnDates(id);
                return NoContent();
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
                await _KhachHang_service.UpdateStatusLoad(id);
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
            await _KhachHang_service.DeleteAsync(id);
            return NoContent();
        }
		[HttpGet("Admin")]
		public async Task<IActionResult> GetAllAdmi()
		{
			var sales = await _service.GetAllWithIdAsync();
			return Ok(sales.Select(s => new
			{
				s.Id, // Thêm Id vào kết quả trả về
				s.Ten,
				s.Mota,
				Trangthai = s.Trangthai switch
				{
					0 => "Đang diễn ra",
					1 => "Chuẩn bị diễn ra",
					2 => "Đã diễn ra",
					3 => "Dừng phát hành",
					_ => "Không xác định"
				},
				s.Ngaybatdau,
				s.Ngayketthuc
			}));
		}


		[HttpGet("{id}/Admin")]
		public async Task<IActionResult> GetByIdAdmin(int id)
		{
			var sale = await _service.GetByIdAsync(id);
			if (sale == null) return NotFound();

			return Ok(new
			{
				sale.Ten,
				sale.Mota,
				Trangthai = sale.Trangthai switch
				{
					0 => "Đang diễn ra",
					1 => "Chuẩn bị diễn ra",
					2 => "Đã diễn ra",
					3 => "Dừng phát hành",
					_ => "Không xác định"
				},
				sale.Ngaybatdau,
				sale.Ngayketthuc
			});
		}
		[HttpGet("details/{saleId}/Admin")]
		public async Task<ActionResult<List<SaleDetailDTO>>> GetSaleDetails(int saleId)
		{
			var saleDetails = await _service.GetSaleDetailsAsync(saleId);
			if (saleDetails == null || !saleDetails.Any())
			{
				return NotFound("No sale details found.");
			}
			return Ok(saleDetails);
		}
		[HttpPut("{saleId}/update-prices/Admin")]
		public async Task<IActionResult> UpdateSanphamchitietPrices(int saleId)
		{
			var result = await _service.UpdateSanphamchitietPricesAsync(saleId);

			if (!result)
			{
				return BadRequest(new { message = "Cập nhật giá thất bại. Kiểm tra trạng thái sale hoặc thông tin sản phẩm." });
			}

			return Ok(new { message = "Cập nhật giá sản phẩm chi tiết thành công." });
		}
		[HttpPost("Admin")]
		public async Task<IActionResult> Add([FromBody] SaleDto saleDto)
		{
			try
			{
				var createdSale = await _service.AddAsync(saleDto);

				// Trả về đối tượng sale với id đã được gán
				return CreatedAtAction(nameof(GetById), new { id = createdSale.Id }, createdSale);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Đã xảy ra lỗi, vui lòng thử lại sau.");
			}
		}

		[HttpPut("{id}/Admin")]
		public async Task<IActionResult> Update(int id, [FromBody] SaleDto saleDto)
		{
			await _service.UpdateAsync(id, saleDto);
			return NoContent();
		}

		[HttpPut("{id}/cancel/Admin")]
		public async Task<IActionResult> UpdateStatusToCancelledAdmin(int id)
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

		[HttpPut("{id}/update-status/Admin")]
		public async Task<IActionResult> UpdateStatusBasedOnDatesAdmin(int id)
		{
			try
			{
				await _service.UpdateStatusBasedOnDates(id);
				return NoContent();
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
				await _service.UpdateStatusLoad(id);
				return NoContent();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpDelete("{id}/Admin")]
		public async Task<IActionResult> DeleteSale(int id)
		{
			var result = await _service.DeleteSaleAsync(id);
			if (!result)
			{
				return NotFound(new { message = $"Không tìm thấy Sale với ID = {id}" });
			}

			return NoContent();
		}
		[HttpPost("add-sale-with-details/Admin")]
		public async Task<IActionResult> AddSaleWithDetails([FromBody] CreateSaleDto createSaleDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _service.AddSaleWithDetailsAsync(createSaleDto);

			if (!result)
			{
				return BadRequest(new { Message = "Thêm sale thất bại." });
			}

			return Ok(new { Message = "Thêm sale thành công." });
		}
		[HttpPut("{saleId}/update-sale-with-details/Admin")]
		public async Task<IActionResult> UpdateSaleWithDetails(int saleId, [FromBody] CreateSaleDto updateSaleDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _service.UpdateSaleWithDetailsAsync(saleId, updateSaleDto);

			if (!result)
			{
				return NotFound(new { Message = "Sale không tồn tại." });
			}

			return Ok(new { Message = "Cập nhật sale thành công." });
		}

	}
}
