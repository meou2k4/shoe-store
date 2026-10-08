app.controller('voucherController', function ($location, $scope) {
    function GetByidKH() {
        const userInfoString = localStorage.getItem("userInfo");

        if (userInfoString === null) {
            $location.path(`/login`);
            return null;
        }

        try {
            const userInfo = JSON.parse(userInfoString);
            document.getElementById('userName').textContent = userInfo.ten;
            return userInfo?.id || null;
        } catch (error) {
            console.error("Lỗi khi phân tích dữ liệu userInfo:", error);
            return null;
        }
    }

    GetByidKH1();
    
    async function GetByidKH1() {
        try {
            // Kiểm tra và lấy thông tin user từ localStorage
            const userInfoString = localStorage.getItem("userInfo");
            if (!userInfoString) {
                console.error("Không tìm thấy thông tin user trong localStorage");
                return null;
            }
    
            const userInfo = JSON.parse(userInfoString);
            if (!userInfo || !userInfo.id) {
                console.error("Thông tin user không hợp lệ");
                return null;
            }
    
            // Lấy thông tin khách hàng từ API
            const infoResponse = await fetch(`https://localhost:7196/api/khachhangs/${userInfo.id}`);
            if (!infoResponse.ok) {
                throw new Error(`Lỗi khi lấy thông tin khách hàng: ${infoResponse.status}`);
            }
            const customerData = await infoResponse.json();
            
            if (!customerData) {
                throw new Error("Dữ liệu khách hàng trả về rỗng");
            }
    
            // Gán dữ liệu cho $scope
            $scope.dataTttk = customerData;
    
            // Kiểm tra và lấy thông tin rank nếu có idrank
            if (customerData.idrank) {
                const rankResponse = await fetch(`https://localhost:7196/api/Ranks/${customerData.idrank}`);
                if (!rankResponse.ok) {
                    console.error(`Lỗi khi lấy thông tin rank: ${rankResponse.status}`);
                    $scope.datarank = null; // Gán null nếu không lấy được rank
                } else {
                    const rankData = await rankResponse.json();
                    $scope.datarank = rankData;
                }
            } else {
                $scope.datarank = null;
            }
    
            // Kích hoạt $digest cycle để cập nhật view
            $scope.$apply();
            
            return customerData;
        } catch (error) {
            console.error("Lỗi trong hàm GetByidKH1:", error);
            
            // Xử lý lỗi cụ thể
            if (error instanceof SyntaxError) {
                console.error("Lỗi phân tích JSON từ localStorage");
            } else if (error.name === 'TypeError') {
                console.error("Lỗi kết nối hoặc API không phản hồi");
            }
            
            // Gán giá trị mặc định cho $scope nếu có lỗi
            $scope.dataTttk = null;
            $scope.datarank = null;
            $scope.$apply();
            
            return null;
        }
    }

    function formatDate(dateString) {
        const date = new Date(dateString);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        return `${day}/${month}/${year} ${hours}:${minutes}`;
    }

    async function fetchVouchers() {
        const idkh = GetByidKH();
        if (!idkh) return;

        const usedVouchers = [];
        const expiredVouchers = [];
        const activeVouchers = [];
        const preparingVouchers = [];

        try {
            // Lấy idRank từ API khách hàng
            const responseRank = await fetch(`https://localhost:7196/api/khachhangs/${idkh}`);
            if (!responseRank.ok) throw new Error(`Lỗi khi lấy idRank: ${responseRank.status}`);
            const data = await responseRank.json();
            const idRank = data.idrank;

            // Lấy danh sách id giảm giá từ API giamgia_rank
            const responseDiscountIds = await fetch(`https://localhost:7196/api/giamgia_rank/rank/${idRank}`);
            if (!responseDiscountIds.ok) {
                document.getElementById('active-vouchers').innerHTML = '<p class="text-center py-4">Rank chưa có voucher.</p>';
                return;
            }
            const discountIds = await responseDiscountIds.json();

            // Lấy danh sách voucher đã sử dụng
            const responseUsedVouchers = await fetch(`https://localhost:7196/api/Hoadons/voucher/${idkh}`);
            if (responseUsedVouchers.ok && responseUsedVouchers.status !== 204) {
                const usedVouchersData = await responseUsedVouchers.json();
                usedVouchers.push(...usedVouchersData);
            }

            // Lấy thông tin chi tiết từng voucher
            for (const id of discountIds) {
                try {
                    const responseVoucher = await fetch(`https://localhost:7196/api/giamgia/${id.iDgiamgia}`);
                    const voucher = await responseVoucher.json();
                    
                    const currentDate = new Date();
                    const startDate = new Date(voucher.ngaybatdau);
                    const endDate = new Date(voucher.ngayketthuc);

                    // Kiểm tra voucher đã sử dụng
                    const isUsed = usedVouchers.some(v => v.idgg === voucher.id);
                    
                    if (isUsed) {
                        expiredVouchers.push(voucher);
                        continue;
                    }

                    // Phân loại voucher
                    if (voucher.trangthaistring === 'Đang phát hành') {
                        if (startDate <= currentDate && currentDate <= endDate) {
                            activeVouchers.push(voucher);
                        } else if (currentDate > endDate) {
                            expiredVouchers.push(voucher);
                        }
                    } else if (voucher.trangthaistring === 'Chuẩn bị phát hành') {
                        preparingVouchers.push(voucher);
                    }

                } catch (error) {
                    console.warn(`Lỗi khi lấy voucher với id: ${id.iDgiamgia}`, error);
                }
            }

            // Sắp xếp và hiển thị voucher
            activeVouchers.sort(sortVouchers);
            displayVouchers(activeVouchers, 'active-vouchers', true);
            displayVouchers(preparingVouchers, 'preparing-vouchers', false);
            displayExpiredVouchers(expiredVouchers);

        } catch (error) {
            console.error('Lỗi khi lấy danh sách voucher:', error);
        }
    }

    function sortVouchers(a, b) {
        if (a.donvi === "%" && b.donvi !== "VNĐ") return -1;
        if (a.donvi !== "VNĐ" && b.donvi === "%") return 1;
        return a.giatri - b.giatri;
    }

    function displayVouchers(vouchers, containerId, isActive) {
        const container = document.getElementById(containerId);
        if (!container) return;
        
        container.innerHTML = vouchers.length === 0 
            ? '<p class="text-center py-4">Không có voucher nào</p>'
            : '';

        vouchers.forEach(voucher => {
            const voucherCol = document.createElement('div');
            voucherCol.className = 'col-md-6';
            voucherCol.innerHTML = `
                <div class="voucher-item border-0 rounded-4 overflow-hidden shadow-sm h-100">
                    <div class="card-body p-4">
                        <div class="d-flex align-items-center">
                            <div class="voucher-icon bg-danger bg-opacity-10 text-danger rounded-3 p-3 me-3">
                                <i class="bi bi-tag fs-2"></i>
                            </div>
                            <div class="flex-grow-1">
                                <h5 class="fw-bold mb-2">${voucher.mota}</h5>
                                <div class="voucher-info">
                                    <p class="mb-2"><span class="fw-semibold">Giá trị:</span> 
                                        ${voucher.giatri >= 1000 ? voucher.giatri.toLocaleString('vi-VN') : voucher.giatri} ${voucher.donvi}
                                    </p>
                                    <p class="mb-0"><span class="fw-semibold">${voucher.trangthaistring === 'Chuẩn bị phát hành' ? 'Kích hoạt sau:' : 'HSD:'}</span> 
                                        ${formatDate(voucher.ngayketthuc)}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer bg-white border-top-0 pt-0 pb-3 px-4 text-end">
                        ${isActive 
                            ? `<a href="#!Sanpham" class="btn btn-danger px-4 rounded-3" data-voucher-id="${voucher.iDgiamgia}">Dùng ngay</a>`
                            : `<button class="btn btn-outline-secondary px-4 rounded-3">Chờ kích hoạt</button>`}
                    </div>
                </div>
            `;
            container.appendChild(voucherCol);
        });
    }

    function displayExpiredVouchers(vouchers) {
        const container = document.getElementById('history-voucher-list');
        const notice = document.getElementById('history-voucher-notice');
        const countBadge = document.getElementById('expired-voucher-count');
        
        container.innerHTML = '';
        notice.style.display = vouchers.length === 0 ? 'block' : 'none';
        
        // Hiển thị số lượng voucher hết hiệu lực
        if (vouchers.length > 0) {
            countBadge.style.display = 'inline-block';
            countBadge.textContent = vouchers.length;
        } else {
            countBadge.style.display = 'none';
        }
    
        vouchers.forEach(voucher => {
            const voucherCol = document.createElement('div');
            voucherCol.className = 'col-md-6';
            voucherCol.innerHTML = `
                <div class="card border-0 shadow-sm h-100">
                    <div class="card-body">
                        <div class="d-flex align-items-center">
                            <div class="bg-secondary bg-opacity-10 text-secondary rounded-3 p-3 me-3">
                                <i class="bi bi-tag fs-2"></i>
                            </div>
                            <div class="flex-grow-1">
                                <h5 class="fw-bold mb-2 text-secondary">${voucher.mota}</h5>
                                <div class="text-muted">
                                    <p class="mb-1"><span class="fw-semibold">Giá trị:</span> 
                                        ${voucher.giatri >= 1000 ? voucher.giatri.toLocaleString('vi-VN') : voucher.giatri} ${voucher.donvi}
                                    </p>
                                    <p class="mb-1"><span class="fw-semibold">HSD:</span> 
                                        ${formatDate(voucher.ngayketthuc)}
                                    </p>
                                    <p class="mb-0 text-danger"><i class="bi bi-exclamation-circle-fill"></i> Đã hết hiệu lực</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            container.appendChild(voucherCol);
        });
    }

    // Khởi tạo khi trang được tải
    fetchVouchers();
});