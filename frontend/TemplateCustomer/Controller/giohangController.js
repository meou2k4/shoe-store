app.controller("GiohangCtrl", function ($document, $rootScope, $scope, $compile, $location, $timeout) {
    const apiUrls = {
        gioHang: "https://localhost:7196/api/Giohang/giohangkhachhang",
        gioHangChiTiet: "https://localhost:7196/api/Giohangchitiet/giohangchitietbygiohang",
        sanPhamChiTiet: "https://localhost:7196/api/Sanphamchitiets",
        sanPham: "https://localhost:7196/api/Sanphams",
        Size: "https://localhost:7196/api/size",
        Chatlieu: "https://localhost:7196/api/ChatLieu",
        Mau: "https://localhost:7196/api/color",
        saleChiTiet: "https://localhost:7196/api/Salechitiets/SanPhamCT",
        giohangchitietbyspctandgh: "https://localhost:7196/api/Giohangchitiet/idghctbygiohangangspct"
    };

    let danhSachSanPham = [];

    // Hàm lấy thông tin khách hàng từ localStorage
    function GetByidKH() {
        const userInfoString = localStorage.getItem("userInfo");

        if (userInfoString === null) {
            $location.path(`/login`);
            return null; // Ngăn chặn việc tiếp tục chạy code
        }

        try {
            const userInfo = JSON.parse(userInfoString);
            return userInfo?.id || null;
        } catch (error) {
            console.error("Lỗi khi phân tích dữ liệu userInfo:", error);
            return null;
        }
    }


    // Hàm lấy id giỏ hàng
    async function fetchGioHangId(idkh) {
        try {
            const response = await fetch(`${apiUrls.gioHang}/${idkh}`);
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy id giỏ hàng:", error);
            return null;
        }
    }

    // Hàm lấy danh sách chi tiết giỏ hàng
    async function fetchGioHangChiTiet(gioHangId) {
        try {
            const response = await fetch(`${apiUrls.gioHangChiTiet}/${gioHangId}`);
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy danh sách chi tiết giỏ hàng:", error);
            return [];
        }
    }

    // Hàm gọi API để lấy sản phẩm chi tiết theo idspct
    async function fetchSanPhamChitiet(sanPhamCTId) {
        try {
            // Kiểm tra nếu idspct có giá trị hợp lệ
            if (!sanPhamCTId) {
                console.error("idspct không hợp lệ");
                return null;
            }

            // Gọi API với idspct
            const response = await fetch(`${apiUrls.sanPhamChiTiet}/${sanPhamCTId}`);

            if (!response.ok) {
                throw new Error(`Lỗi API: ${response.status}`);
            }

            const data = await response.json();

            // Nếu API trả về một đối tượng, chuyển đổi nó thành mảng
            return Array.isArray(data) ? data : [data];
        } catch (error) {
            console.error("Lỗi khi lấy sản phẩm chi tiết:", error);
            return null; // Trả về null nếu có lỗi
        }
    }

    // Hàm lấy danh sách Size
    async function fetchSizes(idsize) {
        try {
            const response = await fetch(apiUrls.Size);
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy danh sách Size:", error);
            return [];
        }
    }

    // Hàm lấy danh sách Chất liệu
    async function fetchChatLieu(idchatlieu) {
        try {
            const response = await fetch(apiUrls.Chatlieu);
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy danh sách Chất liệu:", error);
            return [];
        }
    }

    // Hàm lấy danh sách Màu sắc
    async function fetchMauSac(idcolor) {
        try {
            const response = await fetch(apiUrls.Mau);
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy danh sách Màu sắc:", error);
            return [];
        }
    }

    async function layTenSanPham(id) {
        try {
            const response = await fetch(`${apiUrls.sanPham}/${id}`);
            if (!response.ok) throw new Error("Lỗi khi lấy dữ liệu sản phẩm!");

            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Lỗi lấy sản phẩm:", error);
            return "N/A"; // Tránh lỗi nếu API thất bại
        }
    }

    async function fetchSaleChiTietBySPCTId(spctId) {
        try {
            // Gọi API giảm giá chi tiết
            const response = await fetch(`${apiUrls.saleChiTiet}/${spctId}`);
            if (!response.ok) {
                if (response.status === 404) {
                    console.warn("Không tìm thấy dữ liệu giảm giá chi tiết");
                    return null; // Không tìm thấy, trả về null
                }
                throw new Error(`Lỗi API giảm giá: ${response.status}`);
            }

            return await response.json(); // Trả về dữ liệu JSON nếu thành công
        } catch (error) {
            console.error("Lỗi khi lấy dữ liệu giảm giá chi tiết:", error);
            return null; // Trả về null nếu có lỗi
        }
    }

    async function renderGioHang() {
        const idkh = GetByidKH();
        if (!idkh) return;

        const idgh = await fetchGioHangId(idkh);
        if (!idgh) return;

        const gioHangChiTiet = await fetchGioHangChiTiet(idgh.id);
        if (!gioHangChiTiet) return;

        for (const item of gioHangChiTiet) {
            const sanPhamChiTiet = await fetchSanPhamChitiet(item.idspct);
            if (!sanPhamChiTiet) continue;

            sanPhamChiTiet.soluonggiohang = item.soluong;
            sanPhamChiTiet.idgh = item.id;

            // Lấy thông tin size, chất liệu, màu
            sanPhamChiTiet.size = await fetchSizes(sanPhamChiTiet.sizeId);
            sanPhamChiTiet.chatLieu = await fetchChatLieu(sanPhamChiTiet.chatLieuId);
            sanPhamChiTiet.mau = await fetchMauSac(sanPhamChiTiet.mauId);

            danhSachSanPham.push(sanPhamChiTiet);
        }

        hienThiGioHang(danhSachSanPham);
    }

    async function hienThiGioHang(sanPhamChitiets) {
        const tbody = document.querySelector("tbody");
        tbody.innerHTML = ""; // Xóa nội dung cũ
    
        console.log(sanPhamChitiets);
    
        if (!sanPhamChitiets || sanPhamChitiets.length === 0 || sanPhamChitiets.every(sp => sp.length === 0)) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="10" class="text-center text-muted">
                        <strong>Giỏ hàng chưa có sản phẩm</strong>
                    </td>
                </tr>
            `;
            return;
        }
    
        let tongTien = 0;
    
        for (const sp of sanPhamChitiets) {
            for (const data of sp) {
                let tenSanPham = "N/A";
                let anhSanPham = `https://localhost:7196/picture/${data.urlHinhanh}` || "";
                let mau = "N/A";
                let size = "N/A";
                let chatLieu = "N/A";
                let thanhTien = 0;
    
                // Lấy giá gốc
                let giaGoc = data.giathoidiemhientai || 0;
                let giaSauGiam = giaGoc;
    
                // Kiểm tra giảm giá
                const saleInfo = await fetchSaleChiTietBySPCTId(data.id);
                if (saleInfo && saleInfo.giatrigiam) {
                    if (saleInfo.donvi === 0) {
                        giaSauGiam = giaGoc - saleInfo.giatrigiam; // Trừ trực tiếp
                    } else if (saleInfo.donvi === 1) {
                        giaSauGiam = giaGoc - (giaGoc * saleInfo.giatrigiam / 100); // Giảm theo %
                    }
                }
    
                if (data.idsp) {
                    datasp = await layTenSanPham(data.idsp);
                    tenSanPham = datasp.tenSanpham;
                    trangthaisp = datasp.trangthai
                }
    
                let isDisabled = data.trangthai === 1 || data.soluong <= 0; // Hết hàng
                let isDisabled1 = trangthaisp === 2; // Tạm ngừng bán
                let isProductUnavailable = isDisabled || isDisabled1; // Kết hợp cả 2 trạng thái

                if (Array.isArray(sp.mau) && sp.mau.length > 0) {
                    const mauTimThay = sp.mau.find(m => m.id === data.idMau);
                    mau = mauTimThay ? mauTimThay.tenmau : "N/A";
                }
    
                if (Array.isArray(sp.size) && sp.size.length > 0) {
                    const sizeTimThay = sp.size.find(s => s.id === data.idSize);
                    size = sizeTimThay ? sizeTimThay.sosize : "N/A";
                }
    
                if (Array.isArray(sp.chatLieu) && sp.chatLieu.length > 0) {
                    const chatLieuTimThay = sp.chatLieu.find(cl => cl.id === data.idChatLieu);
                    chatLieu = chatLieuTimThay ? chatLieuTimThay.tenchatlieu : "N/A";
                }
    
                if (giaSauGiam && sp.soluonggiohang) {
                    thanhTien = giaSauGiam * sp.soluonggiohang;
                }
    
                tongTien += thanhTien;
    
                const row = document.createElement("tr");
                row.className = `product-item ${isProductUnavailable ? "disabled-product" : ""}`;
                
                // Xác định thông báo overlay
                let overlayMessage = "";
                if (isDisabled) {
                    overlayMessage = '<div class="overlay">Sản phẩm đã hết</div>';
                } else if (isDisabled1) {
                    overlayMessage = '<div class="overlay">Sản phẩm tạm ngừng bán</div>';
                }

                row.innerHTML = `
                    <td class="align-middle">
                        <input type="checkbox" class="product-checkbox" id="checkbox${data.id}" data-id="${data.id}" ${isProductUnavailable ? "disabled" : ""}>
                    </td>
                    <td class="align-middle position-relative">
                        <img src="${anhSanPham}" alt="Ảnh sản phẩm" class="rounded" width="100px">
                        ${overlayMessage}
                    </td>
                    <td class="align-middle" style="width: 20%;"><strong>${tenSanPham}</strong></td>
                    <td class="align-middle">${mau}</td>
                    <td class="align-middle">${size}</td>
                    <td class="align-middle">${chatLieu}</td>
                    <td class="align-middle giathoidiemhientai">
                        <span class="gia-goc ${saleInfo ? 'text-muted text-decoration-line-through' : ''}">
                            ${Math.floor(giaGoc).toLocaleString('vi-VN')} VNĐ
                        </span>
                        ${saleInfo ? `<br><span class="gia-giam">${Math.floor(giaSauGiam).toLocaleString('vi-VN')} VNĐ</span>` : ""}
                    </td>
                    <td class="align-middle">
                        <div class="d-flex justify-content-center align-items-center">
                            <button type="button" class="btn btn-sm btn-outline-dark" ng-click="capNhatSoLuong($event, false, ${data.id})" ${isProductUnavailable ? "disabled" : ""}>-</button>
                            <span class="px-3 quantity-display">${sp.soluonggiohang || 0}</span>
                            <button type="button" class="btn btn-sm btn-outline-dark" ng-click="capNhatSoLuong($event, true, ${data.id})" ${isProductUnavailable ? "disabled" : ""}>+</button>
                        </div>
                    </td>
                    <td class="align-middle thanhTien">${Math.floor(thanhTien).toLocaleString('vi-VN')} VNĐ</td>
                    <td class="align-middle">
                        <a class="text-danger" ng-click="deleteProduct('${data.id}')">
                            <i class="bi bi-trash3-fill"></i>
                        </a>
                    </td>
                `;
                const compiledElement = $compile(row)($scope);
                tbody.appendChild(compiledElement[0]);
            }
        }
    
        // Cập nhật lại checkbox tổng sau khi render xong
        initializeCheckboxEvents();
    }

    $scope.capNhatSoLuong = async function (event, isIncrease, productId) {
        let dataspct = 0;
        const product = await fetchSanPhamChitiet(productId);
        if (!product) {
            console.error(`Sản phẩm với ID ${productId} không tồn tại trong danh sách.`);
            return;
        }

        let quantityDisplay = event.target.closest(".product-item").querySelector(".quantity-display");
        let currentQuantity = parseInt(quantityDisplay.textContent);

        for (const sp of product) {
            const maxQuantity = sp.soluong;

            if (isIncrease) {
                if (currentQuantity < maxQuantity) {
                    currentQuantity++;
                } else {
                    alert("Số lượng sản phẩm đã đạt giới hạn tối đa.");
                    return;
                }
            } else {
                if (currentQuantity > 1) {
                    currentQuantity--;
                }
                else if (currentQuantity = 1) {
                    alert("Số lượng sản phẩm đã đạt giới hạn tối đa.");
                    return;
                }
            }

            quantityDisplay.textContent = currentQuantity;
            dataspct = sp.id;
        }
        try {
            await updateCartQuantity(dataspct, currentQuantity)
            updateTotals(event);
        } catch (error) {
            console.error("Lỗi khi cập nhật số lượng:", error);
        }
    }

    async function updateCartQuantity(productId, quantity) {
        try {
            // Lấy ID khách hàng
            const idkh = GetByidKH();
            if (!idkh) {
                throw new Error("Không thể lấy ID khách hàng.");
            }

            // Lấy ID giỏ hàng
            const idgh = await fetchGioHangId(idkh);
            if (!idgh || !idgh.id) {
                throw new Error("Không thể lấy ID giỏ hàng.");
            }

            // Lấy ID giỏ hàng chi tiết dựa trên sản phẩm chi tiết và giỏ hàng
            const idgiohangct = await fetchgiohangchitietbyspctandgh(idgh.id, productId);
            if (!idgiohangct || !idgiohangct.id) {
                throw new Error("Không thể lấy ID giỏ hàng chi tiết.");
            }

            // Gửi yêu cầu PUT để cập nhật số lượng sản phẩm
            const response = await fetch(`https://localhost:7196/api/Giohangchitiet/${idgiohangct.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    id: idgiohangct.id,
                    idgh: idgh.id,
                    idspct: productId,
                    soluong: quantity,
                }),
            });

            // Kiểm tra phản hồi từ API
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Lỗi API: ${response.status}. Nội dung: ${errorText}`);
            }

            const data = await response.json();
        } catch (error) {
            console.error("Cập nhật giỏ hàng thất bại", error);
            alert("Cập nhật giỏ hàng thất bại. Vui lòng thử lại sau.");
        }
    }

    async function fetchgiohangchitietbyspctandgh(idgh, idspct) {
        try {
            const response = await fetch(`https://localhost:7196/api/Giohangchitiet/idghctbygiohangangspct/${idgh}/${idspct}`);
            if (!response.ok) {
                if (response.status === 404) {
                    console.warn("Không tìm thấy dữ liệu giỏ hàng chi tiết");
                    return null;
                }
                throw new Error(`Lỗi API giỏ hàng chi tiết: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy dữ liệu giảm giá chi tiết:", error);
            return null;
        }
    }

    function updateTotals(event) {
        let productItem = event.target.closest(".product-item");
        if (!productItem) {
            console.error("Không tìm thấy phần tử .product-item");
            return;
        }

        let quantityDisplay = productItem.querySelector(".quantity-display");
        let giaGiamDisplay = productItem.querySelector(".gia-giam"); // Giá đã giảm
        let giaGocDisplay = productItem.querySelector(".gia-goc");   // Giá gốc
        let thanhTienDisplay = productItem.querySelector(".thanhTien");

        if (!quantityDisplay || !giaGocDisplay || !thanhTienDisplay) {
            console.error("Thiếu phần tử quantity-display, gia-goc hoặc thanhTien.");
            return;
        }

        let quantity = parseInt(quantityDisplay.textContent) || 0;

        // Kiểm tra xem có giảm giá không, nếu không thì lấy giá gốc
        let unitPrice = giaGiamDisplay
            ? parseInt(giaGiamDisplay.textContent.replace(/\D/g, '')) || 0
            : parseInt(giaGocDisplay.textContent.replace(/\D/g, '')) || 0;

        // Tính thành tiền
        let thanhTien = quantity * unitPrice;
        thanhTienDisplay.textContent = `${thanhTien.toLocaleString('vi-VN')} VNĐ`;

        laydulieucheckbox();
    }


    // Hàm cập nhật tổng tiền dựa trên các sản phẩm đã chọn
    function laydulieucheckbox() {
        let totalPrice = 0;

        document.querySelectorAll(".product-item").forEach(item => {
            const checkbox = item.querySelector("input[type='checkbox']");
            if (checkbox.checked) {
                const price = parseInt(item.querySelector(".thanhTien").textContent.replace(' VNĐ', '').replace(/\./g, '')) || 0;
                totalPrice += price;
            }
        });

        document.querySelector("#tongThanhToan").textContent = `${totalPrice.toLocaleString('vi-VN')} VNĐ`;
    }

    // Xử lý sự kiện checkbox thay đổi
    // Hàm kiểm tra và cập nhật trạng thái checkbox tổng
    function updateSelectAllCheckbox() {
        const allCheckboxes = document.querySelectorAll('.product-checkbox:not(:disabled)');
        const checkedCheckboxes = document.querySelectorAll('.product-checkbox:not(:disabled):checked');
        const selectAllCheckbox = document.getElementById('selectAllCheckbox');

        // Nếu tất cả checkbox đều được chọn thì bật checkbox tổng
        if (checkedCheckboxes.length === allCheckboxes.length && allCheckboxes.length > 0) {
            selectAllCheckbox.checked = true;
        }
        // Nếu có ít nhất 1 checkbox không được chọn thì tắt checkbox tổng
        else {
            selectAllCheckbox.checked = false;
        }
    }

    // Xử lý sự kiện checkbox thay đổi
    function initializeCheckboxEvents() {
        document.querySelectorAll(".product-checkbox").forEach(checkbox => {
            checkbox.addEventListener("change", function () {
                laydulieucheckbox(); // Cập nhật tổng tiền khi chọn/bỏ chọn sản phẩm
                updateSelectAllCheckbox(); // Cập nhật trạng thái checkbox tổng
            });
        });
    }

    // Lắng nghe sự kiện khi checkbox "chọn tất cả" được click
    document.getElementById("selectAllCheckbox").addEventListener("click", function () {
        const isChecked = this.checked;
        document.querySelectorAll(".product-checkbox:not(:disabled)").forEach(checkbox => {
            checkbox.checked = isChecked;
        });

        laydulieucheckbox();
    });

    // Gọi hàm này sau khi render giỏ hàng xong
    function initializeCheckboxAfterRender() {
        initializeCheckboxEvents();
        updateSelectAllCheckbox();
    }

    // Trong hàm hienThiGioHang, thay dòng:
    // initializeCheckboxEvents();
    // bằng:
    initializeCheckboxAfterRender();

    // Hàm xử lý khi bấm nút xóa
    $scope.deleteProduct = async function (idspct) {
        Swal.fire({
            title: "Bạn có chắc chắn muốn xóa sản phẩm này?",
            text: "Hành động này không thể hoàn tác!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    const idkh = GetByidKH();
                    if (!idkh) throw new Error("Không thể lấy ID khách hàng.");

                    const idgh = await fetchGioHangId(idkh);
                    if (!idgh || !idgh.id) throw new Error("Không thể lấy ID giỏ hàng.");

                    const idgiohangct = await fetchgiohangchitietbyspctandgh(idgh.id, idspct);
                    if (!idgiohangct) throw new Error("Không thể lấy ID giỏ hàng chi tiết.");

                    const result = await deleteGioHangChiTiet(idgiohangct.id);
                    if (result) {
                        Swal.fire({
                            title: "Xóa thành công!",
                            text: "Sản phẩm đã được xóa khỏi giỏ hàng.",
                            icon: "success",
                            confirmButtonText: "OK"
                        }).then(() => {
                            location.reload(); // Load lại trang
                        });
                    } else {
                        Swal.fire({
                            title: "Lỗi!",
                            text: "Xóa sản phẩm thất bại, vui lòng thử lại.",
                            icon: "error",
                            confirmButtonText: "OK"
                        });
                    }
                } catch (error) {
                    Swal.fire({
                        title: "Lỗi!",
                        text: error.message,
                        icon: "error",
                        confirmButtonText: "OK"
                    });
                }
            }
        });
    };

    async function deleteGioHangChiTiet(idghct) {
        try {
            const response = await fetch(`https://localhost:7196/api/Giohangchitiet/${idghct}`, {
                method: 'DELETE'
            });

            // Kiểm tra trạng thái phản hồi
            if (response.ok) {
                return true; // Trả về true nếu xoá thành công
            } else {
                console.error(`Lỗi API: ${response.status}`);
                return false; // Trả về false nếu xoá thất bại
            }
        } catch (error) {
            console.error("Lỗi khi xóa chi tiết giỏ hàng:", error);
            return false; // Trả về false nếu có lỗi
        }
    }

    // Hàm gọi API để lấy sản phẩm chi tiết theo idspct
    async function fetchSanPhamChitiet2(sanPhamCTId) {
        try {
            if (!sanPhamCTId) {
                console.error("idspct không hợp lệ");
                return null;
            }

            const response = await fetch(`${apiUrls.sanPhamChiTiet}/${sanPhamCTId}`);
            if (!response.ok) {
                throw new Error(`Lỗi API: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error("Lỗi khi lấy sản phẩm chi tiết:", error);
            return null;
        }
    }

    $scope.changePage = async function () {
        try {
            // Lấy tất cả các checkbox đã chọn
            const selectedCheckboxes = document.querySelectorAll('.product-checkbox:checked');

            if (selectedCheckboxes.length === 0) {
                alert("Vui lòng chọn ít nhất một sản phẩm trong giỏ hàng.");
                return false;
            }
            // Lấy ID khách hàng
            const idkh = GetByidKH();
            if (!idkh) {
                alert("Không thể xác định khách hàng. Vui lòng đăng nhập lại.");
                return false;
            }

            // Lấy ID giỏ hàng
            const idgh = await fetchGioHangId(idkh);
            if (!idgh || typeof idgh !== 'object' || !idgh.id) {
                alert("Không thể lấy thông tin giỏ hàng. Vui lòng thử lại.");
                return false;
            }

            // Lấy danh sách chi tiết giỏ hàng
            const gioHangChiTiet = await fetchGioHangChiTiet(idgh.id);
            if (!Array.isArray(gioHangChiTiet) || gioHangChiTiet.length === 0) {
                alert("Giỏ hàng trống hoặc lỗi dữ liệu. Vui lòng thử lại.");
                return false;
            }

            const selectedIds = [];
            const productsWithIssues = []; // Danh sách sản phẩm có vấn đề về số lượng

            // Kiểm tra từng sản phẩm được chọn
            for (const checkbox of selectedCheckboxes) {
                const productId = checkbox.getAttribute('data-id');
                const gioHangItem = gioHangChiTiet.find(item => item.idspct === parseInt(productId));

                if (!gioHangItem) {
                    console.error(`Không tìm thấy sản phẩm với ID ${productId} trong giỏ hàng.`);
                    continue;
                }

                const cartQuantity = gioHangItem.soluong;

                // Gọi API lấy thông tin sản phẩm chi tiết
                const productDetail = await fetchSanPhamChitiet2(productId);
                if (!productDetail) {
                    console.error(`Không thể lấy thông tin sản phẩm chi tiết cho ID ${productId}.`);
                    continue;
                }

                // Kiểm tra số lượng
                if (cartQuantity > productDetail.soluong) {
                    const sanPhamData = await fetchSanPhamById(productDetail.idsp);
                    const tenSanPham = sanPhamData ? sanPhamData.tensp : `ID: ${productDetail.idsp}`;
                    productsWithIssues.push(productDetail.tenSanPham || tenSanPham);
                } else {
                    selectedIds.push(productId); // Chỉ thêm sản phẩm hợp lệ
                }
            }

            // Nếu có sản phẩm vượt quá số lượng
            if (productsWithIssues.length > 0) {
                Swal.fire({
                    title: 'Cảnh báo',
                    icon: 'warning',
                    html: `Các sản phẩm sau đây có số lượng trong giỏ hàng lớn hơn số lượng khả dụng:<br>- ${productsWithIssues.join('<br>- ')}`,
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#3085d6'
                });
                return false;
            }

            // Chuyển hướng sang trang "Hóa đơn giỏ hàng"
            $timeout(() => {
                $scope.$apply(() => {
                    $location.path(`/hoadongiohang/${selectedIds.join(',')}`); // Chuyển hướng với danh sách ID hợp lệ
                });
                $scope.isLoading = false; // Kết thúc tải (nếu cần)
            }, 1000);

        } catch (error) {
            console.error("Lỗi khi kiểm tra số lượng sản phẩm:", error);
            alert("Có lỗi xảy ra khi kiểm tra số lượng sản phẩm. Vui lòng thử lại sau.");
        }
    };

    renderGioHang();
});