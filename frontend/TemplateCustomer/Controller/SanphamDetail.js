
app.controller('SanphamDetail', function ($scope, $routeParams, $location) {
    const apiSPCTUrl = "https://localhost:7196/api/Sanphamchitiets";
    const apiSPUrl = "https://localhost:7196/api/Sanphams";
    const apiSize = "https://localhost:7196/api/size";
    const apiChatlieu = "https://localhost:7196/api/ChatLieu";
    const apiMau = "https://localhost:7196/api/color";
    const apiThuonghieu = "https://localhost:7196/api/thuonghieu";
    const apiDanhGia = "https://localhost:7196/api/Danhgia/GetByIdSP/";
    const apiHinhAnh = "https://localhost:7196/api/Hinhanh/DanhGia/";
    const apigioHang = "https://localhost:7196/api/Giohang/giohangkhachhang";
    const apigioHangChiTiet = "https://localhost:7196/api/Giohangchitiet";
    const apiKhachhang = "https://localhost:7196/api/Khachhangs";
    const sanPhamId = $routeParams.id;
    let dataspct = []; // Sửa Set thành mảng
    let datasanpham = [];

    async function checksoluongsanpham(sanPhamId){
        try {
            if (!sanPhamId) {
                console.error("❌ ID sản phẩm không hợp lệ");
                return [];
            }

            const response = await fetch(`${apiSPUrl}/${sanPhamId}`);
            
            if (!response.ok) {
                throw new Error(`❌ Lỗi API sản phẩm chi tiết: ${response.status}`);
            }
    
            const data = await response.json();
            if(data.soluong <= 0 || data.trangthai == 1){
                Swal.fire({
                    title: "Lỗi",  
                    text: "Sản Phẩm Đã Hết Hàng",
                    icon: "error",
                    confirmButtonText: "OK"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $scope.$apply(() => {
                            $location.path('/'); // Chuyển hướng đến trang "Giỏ hàng"
                        });
                    }
                    $scope.isLoading = false; // Kết thúc tải (nếu cần)
                });  
            }
            
            if(data.trangthai == 2){
                Swal.fire({
                    title: "Lỗi",  
                    text: "Sản Phẩm Tạm Ngưng Bán",
                    icon: "error",
                    confirmButtonText: "OK"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $scope.$apply(() => {
                            $location.path('/'); // Chuyển hướng đến trang "Giỏ hàng"
                        });
                    }
                    $scope.isLoading = false; // Kết thúc tải (nếu cần)
                });  
            }
    
        } catch (error) {
            console.error("❌ Lỗi khi lấy sản phẩm chi tiết:", error);
            return [];
        }
    }

    async function fetchSanPhamChiTiet(sanPhamId) {
        try {
            if (!sanPhamId) {
                console.error("❌ ID sản phẩm không hợp lệ");
                return [];
            }

            const response = await fetch(`${apiSPUrl}/GetALLSanPham/${sanPhamId}`);
            
            if (!response.ok) {
                throw new Error(`❌ Lỗi API sản phẩm chi tiết: ${response.status}`);
            }
    
            const data = await response.json();
    
            if (!data || (Array.isArray(data) && data.length === 0)) {
                console.warn("⚠️ Không có dữ liệu sản phẩm chi tiết.");
                return [];
            }
    
            console.log("✅ Dữ liệu sản phẩm chi tiết:", data);
    
            // Đảm bảo `datasanpham` chỉ nhận dữ liệu hợp lệ
            if (Array.isArray(data)) {
                datasanpham.push(...data);
            } else {
                datasanpham.push(data);
            }
    
            console.log("📌 Danh sách sản phẩm sau khi cập nhật:", datasanpham);
    
            return Array.isArray(data) ? data : [data];
    
        } catch (error) {
            console.error("❌ Lỗi khi lấy sản phẩm chi tiết:", error);
            return [];
        }
    }    

    async function fetchDataSanPhamChiTiet() {
        try {
            await checksoluongsanpham(sanPhamId);
            const danhSachSPCT = await fetchSanPhamChiTiet(sanPhamId);
            if (!danhSachSPCT.length) return;
    
            const container = document.querySelector("#anhsanpham .thumbnail-container");
            const mainImage = document.querySelector("#main-product-image");
    
            let allImages = [];
            let colorIds = new Set();
            let chatlieuIds = new Set();
            let sizeIds = new Set();
            let allGiaSale = [];
    
            // Lặp qua tất cả sản phẩm chi tiết
            for (const spct of danhSachSPCT) {
                var thuonghieu = await fetchThuonghieu(spct.idThuongHieu);
                document.querySelector(".product-title").textContent = spct.tensp;
                document.querySelector("#product-detail").textContent = spct.mota;
                document.querySelector("#product-status").textContent = convertStatus(spct.trangThai);
                document.querySelector("#product-category").textContent = thuonghieu.tenthuonghieu;
    
                // Thu thập giá của sản phẩm chi tiết
                for (const anhspct of spct.sanphamchitiets) {
                    if(anhspct.soluong <= 0){
                        continue;
                    }
                    if (anhspct.giaSaleSanPhamChiTiet != null) {
                        allGiaSale.push(anhspct.giaSaleSanPhamChiTiet);
                    } else {
                        allGiaSale.push(anhspct.giaBanSanPhamChiTiet);
                    }
    
                    // Xử lý hình ảnh
                    const response = await fetch(`${apiSPCTUrl}/GetImageById/${anhspct.id}`);
                    if (!response.ok) continue;
    
                    const blob = await response.blob();
                    const imageUrl = URL.createObjectURL(blob);
                    allImages.push({ url: imageUrl, id: anhspct.id });
    
                    // Lấy tất cả color, chất liệu, size
                    colorIds.add(anhspct.idMau);
                    chatlieuIds.add(anhspct.idChatLieu);
                    sizeIds.add(anhspct.idSize);
                }
    
                // Thêm tất cả sản phẩm chi tiết vào danh sách chung
                dataspct.push(...spct.sanphamchitiets);
            }
    
            if (!allImages.length) return;
    
            // Sắp xếp giá từ thấp đến cao
            allGiaSale.sort((a, b) => a - b);
    
            // Hiển thị giá từ thấp đến cao
            if (allGiaSale.length > 0) {
                document.querySelector("#price-current").textContent = `${Math.round(allGiaSale[0]).toLocaleString("vi-VN")}VNĐ - ${Math.round(allGiaSale[allGiaSale.length - 1]).toLocaleString("vi-VN")}VNĐ`;
                document.querySelector("#price-original").style.display = "none";
            } else {
                document.querySelector("#price-current").textContent = "Chưa có giá";
            }
    
            // Hiển thị ảnh vào thumbnail-container
            container.innerHTML = allImages.map((img, index) =>
                `<img src="${img.url}" class="thumbnail-img ${index === 0 ? 'active' : ''}" 
                data-img-url="${img.url}" data-id-spct="${img.id}" 
                alt="Thumbnail ${index + 1}">`
            ).join('');
    
            // Hiển thị ảnh đầu tiên vào position-relative
            mainImage.src = allImages[0].url;
    
            // Thêm sự kiện click vào từng thumbnail
            document.querySelectorAll(".thumbnail-img").forEach(thumbnail => {
                thumbnail.addEventListener("click", function () {
                    changeMainImage(this.dataset.imgUrl, this);
                });
            });
    
            await fetchAndDisplayColors([...colorIds]);
            await fetchAndDisplayChatlieus([...chatlieuIds]);
            await fetchAndDisplaySize([...sizeIds]);
    
            console.log("Dữ liệu sản phẩm chi tiết:", dataspct); // Debug kiểm tra dữ liệu lấy ra
    
            await hienThiDanhGia();
            for (const spct of datasanpham) {
                LoadSanPhamTuongTu(spct.idThuongHieu, spct.id);
            }
        } catch (error) {
            console.error("Lỗi khi lấy ảnh sản phẩm chi tiết:", error);
        }
    }    
    

    let currentPage = 1;
    const reviewsPerPage = 5;
    let danhGiaList = [];
    let danhGiaDaLoc = []; // Lưu danh sách đã lọc

    async function hienThiDanhGia() {
        const reviewsContainer = document.getElementById("reviews");
        const filterContainer = document.getElementById("filterContainer");
        const avgRatingElement = document.getElementById("avgRating");
        const starDisplayElement = document.getElementById("starDisplay");
        const paginationContainer = document.getElementById("pagination");
    
        reviewsContainer.innerHTML = "";
        filterContainer.innerHTML = "";
        paginationContainer.innerHTML = "";
    
        let danhGiaCounts = {};
        let tongSao = 0;
        let tongDanhGia = 0;
        danhGiaList = [];
    
        try {
            let danhGiaData = await fetchDanhGia(sanPhamId);
    
            // Nếu không có dữ liệu, hiển thị thông báo
            if (!danhGiaData || danhGiaData.length === 0) {
                console.warn(`Không có đánh giá cho sản phẩm ID: ${sanPhamId}`);
                reviewsContainer.innerHTML = `
                    <div class="text-center text-muted mt-3">
                        <p>Sản phẩm chưa có đánh giá</p>
                    </div>`;
                avgRatingElement.textContent = "0";
                starDisplayElement.innerHTML = generateStarHTML(0);
                return;
            }
    
            // Đảm bảo danhGiaData là một mảng
            if (!Array.isArray(danhGiaData)) {
                danhGiaData = [danhGiaData];
            }
    
            // Lấy danh sách hình ảnh của tất cả đánh giá đồng thời
            const danhGiaIds = danhGiaData.map(dg => dg.id);
            const hinhAnhPromises = danhGiaIds.map(id => fetchHinhAnhDanhGia(id));
            const hinhAnhResults = await Promise.all(hinhAnhPromises);
    
            danhGiaData.forEach((danhGia, index) => {
                danhGiaCounts[danhGia.sosao] = (danhGiaCounts[danhGia.sosao] || 0) + 1;
                tongSao += danhGia.sosao;
                tongDanhGia++;
    
                danhGiaList.push({
                    idkh: danhGia.idkh,
                    sosao: danhGia.sosao,
                    noidungdanhgia: danhGia.noidungdanhgia,
                    hinhAnhList: hinhAnhResults[index] || []
                });
            });
    
        } catch (error) {
            console.error(`Lỗi khi lấy đánh giá cho sản phẩm ID ${sanPhamId}:`, error);
            reviewsContainer.innerHTML = `
                <div class="text-center text-danger mt-3">
                    <p>Không thể tải đánh giá. Vui lòng thử lại sau.</p>
                </div>`;
            return;
        }
    
        let avgRating = tongDanhGia > 0 ? (tongSao / tongDanhGia).toFixed(1) : 0;
        avgRatingElement.textContent = `${avgRating}`;
    
        // Hiển thị sao với nửa sao
        starDisplayElement.innerHTML = generateStarHTML(avgRating);
    
        hienThiBoLoc();
        hienThiTrangDanhGia();
        hienThiPhanTrang();
    }    
    
    // Hàm tạo HTML hiển thị sao (hỗ trợ nửa sao)
    function generateStarHTML(rating) {
        const fullStars = Math.floor(rating); // Số sao đầy
        const halfStar = rating % 1 >= 0.5 ? 1 : 0; // Kiểm tra có nửa sao không
        const emptyStars = 5 - fullStars - halfStar; // Số sao trống
    
        let starHTML = "";
    
        for (let i = 0; i < fullStars; i++) {
            starHTML += '<i class="bi bi-star-fill text-warning"></i>'; // ⭐
        }
    
        if (halfStar) {
            starHTML += '<i class="bi bi-star-half text-warning"></i>'; // ⭐½
        }
    
        for (let i = 0; i < emptyStars; i++) {
            starHTML += '<i class="bi bi-star text-muted"></i>'; // ☆
        }
    
        return starHTML;
    }
     

    function hienThiBoLoc() {
        const filterContainer = document.getElementById("filterContainer");
        filterContainer.innerHTML = "";
    
        let danhGiaCounts = {};
        danhGiaList.forEach(danhGia => {
            danhGiaCounts[danhGia.sosao] = (danhGiaCounts[danhGia.sosao] || 0) + 1;
        });
    
        let filterHTML = `<button class="filter-btn active" data-filter="all">Tất Cả (${danhGiaList.length})</button>`;
        for (let i = 5; i >= 1; i--) {
            if (danhGiaCounts[i]) {
                filterHTML += `<button class="filter-btn" data-filter="${i}">${i} Sao (${danhGiaCounts[i]})</button>`;
            }
        }
    
        filterContainer.innerHTML = filterHTML;
    
        document.querySelectorAll(".filter-btn").forEach(button => {
            button.addEventListener("click", function () {
                document.querySelectorAll(".filter-btn").forEach(btn => btn.classList.remove("active"));
                this.classList.add("active");
    
                const filterValue = this.dataset.filter;
                danhGiaDaLoc = (filterValue === "all") ? danhGiaList : danhGiaList.filter(dg => dg.sosao == filterValue);
    
                currentPage = 1; // Reset về trang 1 sau khi lọc
                hienThiTrangDanhGia();
                hienThiPhanTrang();
            });
        });
    
        danhGiaDaLoc = danhGiaList; // Mặc định hiển thị tất cả
    }
    
    // Hiển thị đánh giá theo trang dựa trên danhGiaDaLoc
    async function hienThiTrangDanhGia() {
        const reviewsContainer = document.getElementById("reviews");
        reviewsContainer.innerHTML = "";
    
        let startIndex = (currentPage - 1) * reviewsPerPage;
        let endIndex = startIndex + reviewsPerPage;
        let danhGiaHienTai = danhGiaDaLoc.slice(startIndex, endIndex);
    
        for (const danhGia of danhGiaHienTai) {
            try {
                // Gọi API lấy thông tin khách hàng
                const response = await fetch(`https://localhost:7196/api/khachhangs/${danhGia.idkh}`);
                
                if (!response.ok) {
                    console.error(`Lỗi khi lấy thông tin khách hàng: ${response.status}`);
                    continue; // Bỏ qua đánh giá này nếu không lấy được dữ liệu khách hàng
                }
    
                const data = await response.json();
                const tenKH = data.ten || "Khách hàng ẩn danh"; // Nếu không có tên, hiển thị mặc định
    
                const reviewDiv = document.createElement("div");
                reviewDiv.classList.add("review");
                reviewDiv.dataset.rating = danhGia.sosao;
    
                // Tạo số sao đầy đủ và sao trống
                const starHTML = generateStarHTML(danhGia.sosao);
    
                reviewDiv.innerHTML = `
                    <div class="review-header">
                        <span class="review-user"><strong>${tenKH}</strong></span>
                        <span class="review-rating">${starHTML} (${danhGia.sosao}/5)</span>
                    </div>
                    <p class="review-content">${danhGia.noidungdanhgia}</p>
                    <div class="review-images">
                        ${danhGia.hinhAnhList.map((url) => `<img src="${url}" class="review-img img-thumbnail" alt="Ảnh đánh giá" data-bs-toggle="modal" data-bs-target="#imageModal">`).join("")}
                    </div>
                `;
                // Thêm sự kiện mở modal cho từng ảnh
            reviewDiv.querySelectorAll(".review-img").forEach(img => {
                img.addEventListener("click", function () {
                    document.getElementById("modalImg").src = this.src;
                });
            });

                reviewsContainer.appendChild(reviewDiv);
            } catch (error) {
                console.error(`Lỗi khi hiển thị đánh giá:`, error);
            }
        }
    }    
    
    
    // Hiển thị phân trang dựa trên danhGiaDaLoc
    function hienThiPhanTrang() {
        const paginationContainer = document.getElementById("pagination");
        paginationContainer.innerHTML = "";
    
        let totalPages = Math.ceil(danhGiaDaLoc.length / reviewsPerPage);
    
        if (totalPages > 1) {
            const prevButton = document.createElement("button");
            prevButton.textContent = "« Trang trước";
            prevButton.disabled = currentPage <= 1;
            prevButton.addEventListener("click", () => {
                if (currentPage > 1) {
                    currentPage--;
                    hienThiTrangDanhGia();
                    hienThiPhanTrang();
                }
            });
    
            paginationContainer.appendChild(prevButton);
    
            for (let i = 1; i <= totalPages; i++) {
                const pageButton = document.createElement("button");
                pageButton.textContent = i;
                pageButton.classList.add("page-button");
                if (i === currentPage) {
                    pageButton.classList.add("active");
                }
                pageButton.addEventListener("click", () => {
                    currentPage = i;
                    hienThiTrangDanhGia();
                    hienThiPhanTrang();
                });
    
                paginationContainer.appendChild(pageButton);
            }
    
            const nextButton = document.createElement("button");
            nextButton.textContent = "Trang sau »";
            nextButton.disabled = currentPage >= totalPages;
            nextButton.addEventListener("click", () => {
                if (currentPage < totalPages) {
                    currentPage++;
                    hienThiTrangDanhGia();
                    hienThiPhanTrang();
                }
            });
    
            paginationContainer.appendChild(nextButton);
        }
    }
    
    // Hàm thay đổi ảnh chính khi click vào thumbnail
    function changeMainImage(imageUrl, clickedThumbnail) {
        const mainImage = document.querySelector("#main-product-image");
        if (mainImage) {
            mainImage.src = imageUrl;
        }

        // Loại bỏ class "active" khỏi tất cả ảnh thumbnail
        document.querySelectorAll(".thumbnail-img").forEach(img => img.classList.remove("active"));

        // Thêm class "active" cho ảnh đang chọn
        clickedThumbnail.classList.add("active");
    }

    // Đảm bảo khai báo các biến trước khi sử dụng
    let selectedColorId = null;
    let selectedSizeId = null;
    let selectedChatlieuId = null;

    function updateSizeOptions(validSizeIds) {
        document.querySelectorAll(".size-option").forEach(button => {
            const sizeId = parseInt(button.getAttribute("data-size"), 10);

            if (validSizeIds.includes(sizeId)) {
                button.disabled = false;
                button.style.opacity = "1";
                button.style.pointerEvents = "auto";
            } else {
                button.disabled = true;
                button.style.opacity = "0.5";
                button.style.pointerEvents = "none";
                button.classList.remove("active");
            }
        });

        if (selectedSizeId && !validSizeIds.includes(parseInt(selectedSizeId, 10))) {
            selectedSizeId = null;
        }
    }


    function updateChatlieuOptions(validChatlieuIds) {
        document.querySelectorAll(".chatlieu-option").forEach(button => {
            const chatlieuId = parseInt(button.getAttribute("data-chatlieu"), 10);

            if (validChatlieuIds.includes(chatlieuId)) {
                button.disabled = false;
                button.style.opacity = "1";
                button.style.pointerEvents = "auto";
            } else {
                button.disabled = true;
                button.style.opacity = "0.5";
                button.style.pointerEvents = "none";
                button.classList.remove("active");
            }
        });

        if (selectedChatlieuId && !validChatlieuIds.includes(parseInt(selectedChatlieuId, 10))) {
            selectedChatlieuId = null;
        }
    }

    function filterSanPhamChiTiet() {
        let filteredSPCTs = dataspct;

        // Chỉ lọc nếu giá trị được chọn không phải null hoặc rỗng
        if (selectedColorId !== null && selectedColorId !== "") {
            filteredSPCTs = filteredSPCTs.filter(sp => sp.idMau == selectedColorId);
        }
        if (selectedSizeId !== null && selectedSizeId !== "") {
            filteredSPCTs = filteredSPCTs.filter(sp => sp.idSize == selectedSizeId);
        }
        if (selectedChatlieuId !== null && selectedChatlieuId !== "") {
            filteredSPCTs = filteredSPCTs.filter(sp => sp.idChatLieu == selectedChatlieuId);
        }

        // Kiểm tra nếu tất cả đều không chọn thì hiển thị toàn bộ sản phẩm
        if (!selectedColorId && !selectedSizeId && !selectedChatlieuId) {
            filteredSPCTs = dataspct;
        }

        // Lấy danh sách thuộc tính hợp lệ từ kết quả lọc
        const validSizeIds = new Set(filteredSPCTs.map(sp => sp.idSize));
        const validChatlieuIds = new Set(filteredSPCTs.map(sp => sp.idChatLieu));
        const validColorIds = new Set(filteredSPCTs.map(sp => sp.idMau));

        // Cập nhật giao diện với các tùy chọn hợp lệ
        updateSizeOptions([...validSizeIds]);
        updateChatlieuOptions([...validChatlieuIds]);
        updateColorOptions([...validColorIds]);

        // Chỉ cập nhật hình ảnh khi chọn đủ 3 thuộc tính
        if (selectedColorId && selectedSizeId && selectedChatlieuId) {
            checkAndDisplayMainImage(filteredSPCTs);
            updateQuantity();
        }
    }


    function updateColorOptions(validColorIds) {
        document.querySelectorAll(".color-option").forEach(button => {
            const colorId = parseInt(button.getAttribute("data-color"), 10);

            if (validColorIds.includes(colorId)) {
                button.disabled = false;
                button.style.opacity = "1";
                button.style.pointerEvents = "auto";
            } else {
                button.disabled = true;
                button.style.opacity = "0.5";
                button.style.pointerEvents = "none";
                button.classList.remove("active");
            }
        });

        if (selectedColorId && !validColorIds.includes(parseInt(selectedColorId, 10))) {
            selectedColorId = null;
        }
    }

    // 🟢 Xử lý chọn màu, chỉ được chọn một màu duy nhất
    function selectColor(selectedButton) {
        document.querySelectorAll(".color-option").forEach(button => {
            button.classList.remove("active");
        });

        selectedButton.classList.add("active");

        selectedColorId = selectedButton.getAttribute("data-color");

        // Gọi hàm lọc sản phẩm chi tiết
        filterSanPhamChiTiet();
    }

    function selectChatlieu(selectedButton) {
        document.querySelectorAll(".chatlieu-option").forEach(button => {
            button.classList.remove("active");
        });

        selectedButton.classList.add("active");
        selectedChatlieuId = selectedButton.getAttribute("data-chatlieu");

        filterSanPhamChiTiet();
    }

    // 🟢 Hàm hiển thị màu sắc từ API lên giao diện
    async function fetchAndDisplayColors(colorIds) {
        try {
            const colorContainer = document.querySelector(".color-options");
            colorContainer.innerHTML = ""; // Xóa màu cũ

            for (const idMau of colorIds) {
                const response = await fetch(`${apiMau}/${idMau}`);
                if (!response.ok) continue;

                const colorData = await response.json();
                const colorCode = colorData.mamau;

                // Tạo nút màu với dấu tích ✔
                const colorButton = document.createElement("button");
                colorButton.classList.add("btn", "btn-outline-secondary", "me-2", "color-option");
                colorButton.style.backgroundColor = colorCode;
                colorButton.style.color = "#fff";
                colorButton.style.padding = "18px";
                colorButton.setAttribute("data-color", idMau);

                // Gán sự kiện click
                colorButton.addEventListener("click", function () {
                    selectColor(this);
                });

                // Thêm vào giao diện
                colorContainer.appendChild(colorButton);
            }
        } catch (error) {
            console.error("Lỗi khi lấy danh sách màu:", error);
        }
    }
    // Lấy danh sách đánh giá từ API

    async function fetchDanhGia(idspct) {
        try {
            const response = await fetch(`${apiDanhGia}${idspct}`);
            
            if (!response.ok) {
                if (response.status === 404) {
                    console.warn(`Không tìm thấy dữ liệu đánh giá cho id: ${idspct}`);
                    return [];
                }
                throw new Error(`Lỗi API đánh giá: ${response.status}`);
            }
    
            // Kiểm tra nếu phản hồi rỗng
            const text = await response.text();
            if (!text.trim()) {
                console.warn(`API trả về rỗng cho id: ${idspct}`);
                return [];
            }
    
            // Phân tích JSON nếu có nội dung hợp lệ
            return JSON.parse(text);
        } catch (error) {
            console.error("Lỗi khi lấy dữ liệu đánh giá:", error);
            return [];
        }
    }     

    // Lấy hình ảnh đánh giá từ API
    async function fetchHinhAnhDanhGia(danhGiaId) {
        try {
            const response = await fetch(`${apiHinhAnh}${danhGiaId}`);
            if (!response.ok) return [];

            const hinhAnhData = await response.json();
            return hinhAnhData.map(img => img.url); // Trả về danh sách URL ảnh
        } catch (error) {
            console.error("Lỗi khi lấy hình ảnh đánh giá:", error);
            return [];
        }
    }

    async function fetchAndDisplayChatlieus(chatlieuIds) {
        try {
            const chatlieuContainer = document.querySelector(".chatlieu-options");
            chatlieuContainer.innerHTML = ""; // Xóa nội dung cũ

            let chatlieuButtons = [];

            for (const idchatlieu of chatlieuIds) {
                const response = await fetch(`${apiChatlieu}/${idchatlieu}`);
                if (!response.ok) continue;

                const chatlieuData = await response.json();
                const chatlieuName = chatlieuData.tenchatlieu;

                chatlieuButtons.push(`
                    <div class="chatlieu-option" data-chatlieu="${idchatlieu}" 
                        style="display: inline-flex; align-items: center; justify-content: center; 
                               margin-right: 10px; cursor: pointer;
                               background-color: #fff;
                               padding: 0 12px;">
                        ${chatlieuName}
                    </div>
                `);
                
            }

            chatlieuContainer.innerHTML = chatlieuButtons.join('');

            // Gán sự kiện click vào từng nút chất liệu sau khi thêm vào DOM
            document.querySelectorAll(".chatlieu-option").forEach(button => {
                button.addEventListener("click", function () {
                    selectChatlieu(this); // Gọi hàm đúng cách
                });
            });

        } catch (error) {
            console.error("Lỗi khi lấy danh sách chất liệu:", error);
        }
    }

    function checkAndDisplayMainImage(filteredSPCTs) {
        if (!filteredSPCTs.length) return;

        const firstImageId = filteredSPCTs[0].id;
        const selectedImage = document.querySelector(`.thumbnail-img[data-id-spct="${firstImageId}"]`);

        if (selectedImage) {
            changeMainImage(selectedImage.dataset.imgUrl, selectedImage);
        }
    }


    function selectSize(selectedButton) {
        document.querySelectorAll(".size-option").forEach(button => {
            button.classList.remove("active");
        });

        selectedButton.classList.add("active");
        selectedSizeId = selectedButton.getAttribute("data-size");

        filterSanPhamChiTiet();
    }

    function selectChatlieu(selectedButton) {
        document.querySelectorAll(".chatlieu-option").forEach(button => {
            button.classList.remove("active");
        });

        selectedButton.classList.add("active");
        selectedChatlieuId = selectedButton.getAttribute("data-chatlieu");

        filterSanPhamChiTiet();
    }


    async function fetchAndDisplaySize(sizeIds) {
        try {
            const sizeContainer = document.querySelector(".size-options");
            sizeContainer.innerHTML = ""; // Xóa nội dung cũ

            let sizeButtons = [];

            for (const idsize of sizeIds) {
                const response = await fetch(`${apiSize}/${idsize}`);
                if (!response.ok) continue;

                const sizeData = await response.json();
                const sizeName = sizeData.sosize;

                sizeButtons.push(`
                    <div class="size-option" data-size="${idsize}" 
                        style="style:width: 45px; height: 40px; display: inline-flex; align-items: center; justify-content: center;margin-right: 10px; cursor: pointer;  border: 1px solid #dee2e6; background-color: #fff; transition: all 0.3s ease; ">
                        ${sizeName}
                    </div>
                `);
            }

            sizeContainer.innerHTML = sizeButtons.join('');

            // ✅ Gán sự kiện click đúng cách vào từng `.size-option`
            document.querySelectorAll(".size-option").forEach(button => {
                button.addEventListener("click", function () {
                    selectSize(this); // Gọi hàm đúng
                });
            });

        } catch (error) {
            console.error("Lỗi khi lấy danh sách size:", error);
        }
    }

    async function fetchThuonghieu(idth) {
        try {
            if (!idth) {
                console.error("id thương hiệu không hợp lệ");
                return;
            }

            const response = await fetch(`${apiThuonghieu}/${idth}`);
            if (!response.ok) {
                throw new Error(`Lỗi API thương hiệu: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Lỗi khi lấy thông tin thương hiệu:", error);
        }
    }

    function convertStatus(statusCode) {
        return statusCode === 0 ? "Còn hàng" : (statusCode === 1 ? "Hết hàng" : "Không xác định");
    }    

    function resetSelections() {
        // Xóa các thuộc tính đã chọn
        selectedColorId = null;
        selectedSizeId = null;
        selectedChatlieuId = null;
    
        // Xóa trạng thái active của các button
        document.querySelectorAll(".color-option, .size-option, .chatlieu-option").forEach(button => {
            button.classList.remove("active");
            button.disabled = false;
            button.style.opacity = "1";
            button.style.pointerEvents = "auto";
        });
    
        let allGiaSale = [];
    
        for (const spct of datasanpham) {
            document.querySelector("#product-detail").textContent = spct.mota;
            document.querySelector("#product-status").textContent = convertStatus(spct.trangThai);
    
            for (const spctItem of spct.sanphamchitiets) {
                if (spctItem.giaSaleSanPhamChiTiet != null) {
                    allGiaSale.push(spctItem.giaSaleSanPhamChiTiet);
                } else {
                    allGiaSale.push(spctItem.giaBanSanPhamChiTiet);
                }
            }
        }
    
        // Sắp xếp giá từ thấp đến cao
        allGiaSale.sort((a, b) => a - b);
        document.querySelector("#price-original").style.display = "none";

        // Hiển thị giá theo khoảng từ thấp nhất đến cao nhất
        if (allGiaSale.length > 0) {
            document.querySelector("#price-current").textContent = `${Math.round(allGiaSale[0]).toLocaleString("vi-VN")}VNĐ - ${Math.round(allGiaSale[allGiaSale.length - 1]).toLocaleString("vi-VN")}VNĐ`;
        } else {
            document.querySelector("#price-current").textContent = "Chưa có giá";
        }
    
        updateQuantity();
        console.log("Đã đặt lại các lựa chọn về mặc định.");
    }
    

    document.getElementById("increase-quantity").addEventListener("click", function () {
        if (!selectedColorId || !selectedSizeId || !selectedChatlieuId)
        {
            alert("Vui lòng chọn đủ thuộc tính");
            return;
        };

        let selectedProduct = dataspct.find(sp =>
            sp.idMau == selectedColorId &&
            sp.idSize == selectedSizeId &&
            sp.idChatLieu == selectedChatlieuId
        );

        if (!selectedProduct) return;

        let quantityInput = document.getElementById("quantity");
        let currentQuantity = parseInt(quantityInput.value, 10);
        let maxQuantity = selectedProduct.soluong; // Giới hạn số lượng có thể mua

        if (currentQuantity < maxQuantity) {
            quantityInput.value = currentQuantity + 1;
        } else {
            alert("Bạn chỉ có thể mua tối đa " + maxQuantity + " sản phẩm.");
        }
    });

    document.getElementById("decrease-quantity").addEventListener("click", function () {
        let quantityInput = document.getElementById("quantity");
        let currentQuantity = parseInt(quantityInput.value, 10);

        if (currentQuantity > 1) {
            quantityInput.value = currentQuantity - 1;
        }
    });

    // Kiểm tra điều kiện khi đủ 3 thuộc tính
    function updateQuantity() {
        let quantityInput = document.getElementById("quantity");
        let priceCurrent = document.querySelector("#price-current");
        let priceOriginal = document.querySelector("#price-original");
        let productStatus = document.querySelector("#product-status");

        if (selectedColorId && selectedSizeId && selectedChatlieuId) {
            let selectedProduct = dataspct.find(sp =>
                sp.idMau == selectedColorId &&
                sp.idSize == selectedSizeId &&
                sp.idChatLieu == selectedChatlieuId
            );
    
            if (selectedProduct) {
                let maxQuantity = selectedProduct.soLuong;
                quantityInput.value = 1; // Đặt lại số lượng về 1
                quantityInput.setAttribute("max", maxQuantity); // Giới hạn số lượng
                productStatus.textContent = convertStatus(selectedProduct.trangThai);
    
                // Lấy giá tại thời điểm hiện tại và giá sale
                let giaThoiDiemHienTai = selectedProduct.giathoidiemhientai;
                let giaSaleSanPhamChiTiet = selectedProduct.giaSaleSanPhamChiTiet;
    
                // Kiểm tra giá để hiển thị đúng format
                if (giaThoiDiemHienTai > giaSaleSanPhamChiTiet) {
                    priceCurrent.textContent = `${Math.round(giaSaleSanPhamChiTiet).toLocaleString("vi-VN")}VNĐ`;
                    priceOriginal.textContent = `${Math.round(giaThoiDiemHienTai).toLocaleString("vi-VN")}VNĐ`;
                    priceOriginal.style.display = ""
                } else {
                    priceCurrent.textContent = `${Math.round(giaThoiDiemHienTai).toLocaleString("vi-VN")}VNĐ`;
                    document.querySelector("#price-original").style.display = "none"; // Ẩn giá gốc nếu 2 giá bằng nhau
                }
            }
        } else {
            quantityInput.value = 1;
        }
    }    

    $scope.muaSanphamNgay = function() {
        let inputQuantity = parseInt(document.querySelector("#quantity").value, 10);
    
        if (!selectedColorId || !selectedSizeId || !selectedChatlieuId) {
            Swal.fire("Lỗi", "Vui lòng chọn đầy đủ Màu sắc, Kích thước và Chất liệu.", "error");
            return;
        }
    
        let selectedSPCT = dataspct.find(spct =>
            spct.idMau == selectedColorId &&
            spct.idSize == selectedSizeId &&
            spct.idChatLieu == selectedChatlieuId
        );
    
        if (!selectedSPCT) {
            Swal.fire("Lỗi", "Không tìm thấy sản phẩm phù hợp.", "error");
            return;
        }
    
        if (selectedSPCT.trangThai == 1) {
            Swal.fire("Lỗi", "Sản phẩm đã hết hàng không thể mua được.", "error");
            return;
        }
    
        if (inputQuantity > selectedSPCT.soluong) {
            Swal.fire("Lỗi", `Bạn chỉ có thể mua tối đa ${selectedSPCT.soluong} sản phẩm.`, "error");
            return;
        }
    
        // Lưu vào sessionStorage (hoặc localStorage nếu muốn giữ lâu hơn)
        sessionStorage.setItem("selectedSPCTId", selectedSPCT.id);
        sessionStorage.setItem("inputQuantity", inputQuantity);
    
        // Chuyển hướng sang trang khác kèm theo ID sản phẩm và số lượng
        console.log("Chuyển hướng đến:", `/hoadon/${selectedSPCT.id}?quantity=${inputQuantity}`);
        $location.path(`/hoadon/${selectedSPCT.id}`).search({ quantity: inputQuantity });
    }       

    function GetByidKH() {
        // Lấy dữ liệu từ localStorage
        const userInfoString = localStorage.getItem("userInfo");
        let userId = 0; // Giá trị mặc định nếu không có thông tin khách hàng

        // Kiểm tra nếu dữ liệu tồn tại
        if (userInfoString) {
            try {
                // Chuyển đổi chuỗi JSON thành đối tượng
                const userInfo = JSON.parse(userInfoString);

                // Kiểm tra và lấy giá trị id từ userInfo
                userId = userInfo?.id || 0;
            } catch (error) {
                console.error("Lỗi khi phân tích dữ liệu userInfo:", error);
            }
        } else {
            console.warn("Dữ liệu userInfo không tồn tại trong localStorage.");
        }

        return userId;
    }

    // Hàm lấy id giỏ hàng
    async function fetchGioHangId(idkh) {
        try {
            const response = await fetch(`${apigioHang}/${idkh}`);
            const data =  await response.json();
            console.log(data);
            return data;
        } catch (error) {
            console.error("Lỗi khi lấy id giỏ hàng:", error);
            return null;
        }
    }

    async function AddGHCT() {
        try {
            const idkh = GetByidKH();
            const idgh = await fetchGioHangId(idkh);
            if (!idgh) {
                Swal.fire("Lỗi", "Không tìm thấy giỏ hàng của bạn.", "error");
                return;
            }
    
            // Lấy số lượng sản phẩm người dùng nhập
            let inputQuantity = parseInt(document.querySelector("#quantity").value, 10);
            if (isNaN(inputQuantity) || inputQuantity <= 0) {
                Swal.fire("Lỗi", "Vui lòng nhập số lượng hợp lệ.", "error");
                return;
            }
    
            // Kiểm tra người dùng đã chọn đủ thuộc tính chưa
            if (!selectedColorId || !selectedSizeId || !selectedChatlieuId) {
                Swal.fire("Lỗi", "Vui lòng chọn đầy đủ Màu sắc, Kích thước và Chất liệu.", "error");
                return;
            }
    
            // Lọc sản phẩm chi tiết dựa trên thuộc tính đã chọn
            let selectedSPCT = dataspct.find(spct =>
                spct.idMau == selectedColorId &&
                spct.idSize == selectedSizeId &&
                spct.idChatLieu == selectedChatlieuId
            );
    
            if (!selectedSPCT) {
                Swal.fire("Lỗi", "Không tìm thấy sản phẩm phù hợp.", "error");
                return;
            }
    
            // Kiểm tra trạng thái sản phẩm và số lượng tồn kho
            if (selectedSPCT.trangThai == 1) {
                Swal.fire("Lỗi", "Sản phẩm đã hết hàng không thể mua được.", "error");
                return;
            }
    
            if (inputQuantity > selectedSPCT.soluong) {
                Swal.fire("Lỗi", `Bạn chỉ có thể mua tối đa ${selectedSPCT.soluong} sản phẩm.`, "error");
                return;
            }
    
            // Chuẩn bị dữ liệu để thêm vào giỏ hàng
            const newGioHangCT = {
                id: 0,
                idgh: idgh.id,
                idspct: selectedSPCT.id,
                soluong: inputQuantity,
            };
    
            // Gửi yêu cầu thêm vào giỏ hàng
            const response = await fetch(apigioHangChiTiet, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newGioHangCT)
            });
    
            // Kiểm tra phản hồi API
            if (!response.ok) {
                const errorData = await response.json();
                Swal.fire("Lỗi", errorData.message || "Không thể thêm sản phẩm vào giỏ hàng.", "error");
                return;
            }
    
            Swal.fire({
                title: "Thành công",
                text: "Sản phẩm đã được thêm vào giỏ hàng.",
                icon: "success",
                confirmButtonText: "OK"
            }).then((result) => {
                if (result.isConfirmed) {
                    $scope.$apply(() => {
                        $location.path('/giohang'); // Chuyển hướng đến trang "Giỏ hàng"
                    });
                }
                $scope.isLoading = false; // Kết thúc tải (nếu cần)
            });            
    
        } catch (error) {
            console.error("Lỗi khi thêm vào giỏ hàng:", error);
            Swal.fire("Lỗi", "Không thể thêm sản phẩm vào giỏ hàng.", "error");
        }
    }     
    
    async function LoadSanPhamTuongTu(idThuongHieu, idSanPhamHienTai) {
        if (!idThuongHieu) {
            console.warn("idThuongHieu không tồn tại.");
            return;
        }
    
        try {
            const response = await fetch(`https://localhost:7196/api/Sanphams/GetALLSanPhamByThuongHieu/${idThuongHieu}`);
            if (!response.ok) {
                throw new Error("Không thể lấy dữ liệu sản phẩm tương tự.");
            }
    
            let data = await response.json();
    
            // Lọc bỏ sản phẩm hiện tại khỏi danh sách
            const filteredProducts = data.filter(product => product.id !== idSanPhamHienTai);
    
            // Lấy danh sách sản phẩm tương tự ngẫu nhiên
            const sanPhamsTuongTu = randomizeProducts(filteredProducts, 4);
    
            // Hiển thị sản phẩm vào giao diện
            hienThiSanPhamTuongTu(sanPhamsTuongTu);
    
            console.log("Danh sách sản phẩm tương tự:", sanPhamsTuongTu);
        } catch (error) {
            console.error("Lỗi khi tải sản phẩm tương tự:", error);
        }
    }
    
    async function hienThiSanPhamTuongTu(datasanPhams) {
        let sanPhams = [];
    
        for (const data of datasanPhams) {
            try {
                const spctList = await fetchSanPhamChiTiet(data.id); // Lấy danh sách sản phẩm chi tiết
    
                if (spctList.length > 0) {
                    for (const spctGroup of spctList) { // Duyệt từng nhóm sản phẩm chi tiết
                        const spct = spctGroup.sanphamchitiets?.[0]; // Chỉ lấy phần tử đầu tiên
    
                        if (spct) { // Nếu có sản phẩm chi tiết
    
                            // Kiểm tra giá hợp lệ trước khi tính phần trăm giảm
                            let giaban = parseFloat(data.giaban) || 0;
                            let giasale = parseFloat(data.giasale) || 0;
                            let phanTramGiam = 0;
    
                            if (giaban > 0 && giasale > 0 && giasale < giaban) {
                                phanTramGiam = Math.round(((giaban - giasale) / giaban) * 100);
                            }
    
                            sanPhams.push({
                                id: spctGroup.id,
                                tensp: data.tensp,
                                giaban,
                                giasale,
                                anh: `https://localhost:7196/picture/${spct.urlHinhanh}`,
                                phanTramGiam
                            });
                        }
                    }
                }
            } catch (error) {
                console.error(`Lỗi khi lấy chi tiết sản phẩm ${data.id}:`, error);
            }
        }
    
        const container = document.querySelector(".related-products .row");
        if (!container) return;
    
        container.innerHTML = sanPhams.map(sp => `
            <div class="col-md-3 col-6 mb-4">
                <a class="card h-100 position-relative" href="#!SanpDetail/${sp.id}" style="cursor: pointer; text-decoration: none;">
                    ${sp.phanTramGiam > 0 ? `
                        <div class="position-absolute top-0 end-0 bg-warning text-dark p-2 m-2 small">
                            ${sp.phanTramGiam}% GIẢM
                        </div>` 
                    : ""}
                    <img src="${sp.anh}" class="card-img-top product-image" alt="${sp.tensp}">
                    <div class="card-body">
                        <h5 class="card-title">${sp.tensp}</h5>
                        <p class="card-text">
                            <span class="text-danger fw-bold">${Math.round(sp.giasale).toLocaleString("vi-VN")} VNĐ</span>
                            ${sp.phanTramGiam > 0 ? 
                                `<small class="text-muted text-decoration-line-through ms-2">${Math.round(sp.giaban).toLocaleString("vi-VN")} VNĐ</small>` 
                            : ""}
                        </p>
                    </div>
                </a>
            </div>
        `).join('');
    }
          

    // Hàm chọn ngẫu nhiên sản phẩm
    function randomizeProducts(products, maxItems) {
        if (products.length > maxItems) {
            return products.sort(() => 0.5 - Math.random()).slice(0, maxItems);
        }
        return products;
    }    

    // Gán sự kiện cho nút "Xóa tất cả"
    document.getElementById("btnResetSelections").addEventListener("click", resetSelections);
    document.getElementById("addtocart").addEventListener("click", AddGHCT);

    document.getElementById("toggleReviews").addEventListener("click", function () {
        const reviews = document.getElementById("reviews");
        if (reviews.style.display === "none" || reviews.style.display === "") {
            reviews.style.display = "block";
            this.textContent = "Thu gọn đánh giá ▲";
        } else {
            reviews.style.display = "none";
            this.textContent = "Xem đánh giá ▼";
        }
    });
    

    // Gọi API khi trang tải
    fetchDataSanPhamChiTiet();
})