app.controller('saleController', function () {
    // Toggle switch
    document.querySelectorAll('.toggle-switch input').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const status = this.checked ? 'Kích hoạt' : 'Tắt';
            console.log(`Sale đã được ${status}`);
        });
    });

    // Thêm chi tiết sale
    document.getElementById("btnAddSaleDetail").addEventListener("click", function () {
        const tableBody = document.getElementById("saleDetailBody");
        const newRow = document.createElement("tr");

        newRow.innerHTML = `
        <td>
            <select class="form-select">
                <option value="">Chọn sản phẩm</option>
                <option value="sp1">Áo thun nam</option>
                <option value="sp2">Quần jean</option>
                <option value="sp3">Giày thể thao</option>
            </select>
        </td>
        <td>
            <select class="form-select">
                <option value="1">%</option>
                <option value="0">VNĐ</option>
            </select>
        </td>
        <td><input type="number" class="form-control" placeholder="Số lượng"></td>
        <td><input type="number" class="form-control" placeholder="Giá trị giảm"></td>
        <td class="text-center">
            <button class="btn btn-sm btn-danger btnRemoveRow">
                <i class="fas fa-trash"></i>
            </button>
        </td>
    `;

        tableBody.appendChild(newRow);

        // Xóa hàng khi nhấn nút "Xóa"
        newRow.querySelector(".btnRemoveRow").addEventListener("click", function () {
            newRow.remove();
        });
    });

    // Thêm chi tiết sale trong modal chỉnh sửa
    document.getElementById("btnAddSaleDetailupdate").addEventListener("click", function () {
        const tableBody = document.getElementById("editSaleDetailBody");
        const newRow = document.createElement("tr");

        newRow.innerHTML = `
        <td>
            <select class="form-select">
                <option value="">Chọn sản phẩm</option>
                <option value="sp1">Áo thun nam</option>
                <option value="sp2">Quần jean</option>
                <option value="sp3">Giày thể thao</option>
            </select>
        </td>
        <td>
            <select class="form-select">
                <option value="1">%</option>
                <option value="0">VNĐ</option>
            </select>
        </td>
        <td><input type="number" class="form-control" placeholder="Số lượng"></td>
        <td><input type="number" class="form-control" placeholder="Giá trị giảm"></td>
        <td class="text-center">
            <button class="btn btn-sm btn-danger btnRemoveRow">
                <i class="fas fa-trash"></i>
            </button>
        </td>
        <td class="text-center">
        </td>
    `;

        tableBody.appendChild(newRow);

        // Xóa hàng khi nhấn nút "Xóa"
        newRow.querySelector(".btnRemoveRow").addEventListener("click", function () {
            newRow.remove();
        });
    });

    // Xử lý xóa chi tiết sale
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('btnRemoveRow')) {
            e.target.closest('tr').remove();
        }
    });
});