app.controller('trahangController', function ($scope, $http, $location, $interval, $timeout) {
    $scope.counts = {
        all: 0,
        pending: 0,
        confirmed: 0,
        shipping: 0,
        success: 0,
        failed: 0
    };
    const userInfoString = localStorage.getItem("userInfo1");
    const userInfo = JSON.parse(userInfoString);
    $scope.listTraHang = [];
    const api = "https://localhost:7196/api/Trahangs"
    $http.get(api)
        .then(function (response) {
            $scope.listTraHang = response.data.sort((a, b) => b.id - a.id);
            console.log("Data trả hàng: ", $scope.listTraHang)
            $scope.updateCounts();
        })
        .catch(function (error) {
            console.error("Lỗi API: ", error);
            Swal.fire({
                icon: 'error',
                title: 'Lỗi hệ thống',
                text: 'Đã xảy ra lỗi. Vui lòng thử lại sau!',
            });

        })

    $scope.updateCounts = function () {
        $scope.counts = {
            all: $scope.listTraHang.length,
            pending: $scope.listTraHang.filter(x => x.trangthai == 0).length,
            confirmed: $scope.listTraHang.filter(x => x.trangthai == 1).length,
            shipping: $scope.listTraHang.filter(x => x.trangthai == 2).length,
            success: 0,
            failed: 0
        };
    };

    $scope.OpenModalXacNhan = function (x) {
        Swal.fire({
            title: 'Bạn muốn xác nhận đơng hàng không?\nChọn một phương án xử lý:',
            html: `
                    <div style="text-align: left;">
                        <label><input type="radio" name="option" value="Trả hàng và hoàn tiền"> Trả hàng và hoàn tiền</label><br>
                        <label><input type="radio" name="option" value="Trả hàng không hoàn tiền"> Trả hàng không hoàn tiền</label><br>
                        <label><input type="radio" name="option" value="Không hoàn tiền và không hoàn hàng"> Không hoàn tiền và không hoàn hàng</label><br>
                        
                        <textarea id="ghichu" class="form-control" placeholder="Nhập chú thích (Nếu có)"></textarea>
                    </div>
                `,
            showCancelButton: true,
            confirmButtonText: 'Xác nhận',
            preConfirm: () => {
                const selected = document.querySelector('input[name="option"]:checked');
                const ghichu = document.getElementById('ghichu').value;
                if (!selected) {
                    Swal.showValidationMessage('Bạn phải chọn một phương án');
                    return false;
                }
                return {
                    option: selected.value,
                    note: ghichu
                };
            }
        }).then((result) => {
            if (result.isConfirmed) {
                console.log('Phương án đã chọn:', result.value.option);
                console.log('Ghi chú:', result.value.note);
                $http({
                    method: 'PUT',
                    url: 'https://localhost:7196/api/Trahangs/Xacnhan',
                    params: {
                        id: x.id,
                        hinhthucxuly: result.value.option,
                        idnv: userInfo.id,
                        chuthich: result.value.note || null
                    }
                })
                    .then(function (response) {
                        console.log("Kết quả:", response.data);
                        Swal.fire({
                            icon: 'success',
                            title: 'Xác nhận thành công!',
                            text: `Đơn hàng đã được xác nhận.`,
                        }).then(() => {
                            location.reload();
                            window.scroll(0, 0);
                        });
                    })
                    .catch(function (error) {
                        console.error("Lỗi API: ", error);
                        let msg = error.data?.message || "Đã xảy ra lỗi. Vui lòng thử lại sau!";
                        Swal.fire('Lỗi hệ thống', msg, 'error');
                    });
            }
        });

    };


    $scope.dataTrahang = {}

    $scope.OpenModalChiTiet = function (idth) {
        // Hiển thị modal
        $("#returnDetailModal").modal('show');

        // Reset dữ liệu cũ
        $scope.dataTrahang = null;
        $scope.detailLoading = true;

        // 1) Lấy thông tin chung của trả hàng
        $http.get(`https://localhost:7196/api/Trahangs/thongtinn/${idth}`)

            .then(function (res) {
                $scope.dataTrahang = res.data;
                // 2) Lấy hình ảnh
                return $http.get(`https://localhost:7196/api/Hinhanh/TraHang/${idth}`);
            })
            .then(function (imgRes) {
                // Gán hình ảnh (nếu có)
                $scope.dataTrahang.hinhanhs = imgRes.data.map(img => {
                    if (img.url && !img.url.startsWith('http')) {
                        img.url = 'https://localhost:7196/' + img.url.replace(/^\/+/, '');
                    }
                    return img;
                });
                // 3) Lấy danh sách sản phẩm trả hàng
                return $http.get(`https://localhost:7196/api/Trahangchitiets/ListSanPhamByIdth/${idth}`);
            })
            .then(function (prodRes) {
                // Gán danh sách chi tiết trả hàng
                // Giả sử ViewModel của bạn có các trường:
                //   urlHinhanh, tensp, mau, size, soluong, lydo
                $scope.dataTrahang.trahangchitiets = prodRes.data.map(item => ({
                    anh: `https://localhost:7196/picture/${item.urlHinhanh}`,
                    tensanpham: item.tensp,
                    mau: item.mau,
                    size: item.size,
                    chatlieu: item.chatlieu,
                    soluong: item.soluong,
                    giasp: item.giasp,
                }));
            })
            .catch(function (error) {
                console.error("Lỗi API: ", error);
                let msg = error.data?.message || "Đã xảy ra lỗi. Vui lòng thử lại sau!";
                Swal.fire('Lỗi hệ thống', msg, 'error');
            })
            .finally(function () {
                $scope.detailLoading = false;
                console.log("$scope.dataTrahang.trahangchitiets: ", $scope.dataTrahang.trahangchitiets);
            });
    };


    $scope.huydon = function (x) {
        var lyDoHuy = prompt("Bạn có chắc chắn muốn hủy đơn hàng này không?\nVui lòng nhập lý do hủy:");
    
        // Kiểm tra nếu không nhập hoặc nhập toàn khoảng trắng
        if (lyDoHuy == null || lyDoHuy.trim() === "") {
            Swal.fire({
                icon: 'warning',
                title: 'Lý do hủy bắt buộc',
                text: 'Vui lòng nhập lý do hủy đơn hàng để tiếp tục.',
            });
            return; // Dừng xử lý nếu không có lý do
        }
    
        Swal.fire({
            title: 'Xác nhận hủy đơn hàng?',
            text: "Bạn có chắc chắn muốn hủy đơn hàng này không?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Xác nhận',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                const id = x;
                const idnv = userInfo.id;
                const api = `https://localhost:7196/api/Trahangs/HuyDon?id=${id}&idnv=${idnv}&chuthich=${encodeURIComponent(lyDoHuy)}`;
    
                console.log("API gửi đi:", api);
    
                $http.put(api)
                    .then(function (response) {
                        console.log("Kết quả:", response.data);
                        Swal.fire({
                            icon: 'success',
                            title: 'Hủy đơn thành công!',
                            text: `Đơn hàng đã được hủy.`,
                        }).then(() => {
                            location.reload();
                            window.scroll(0, 0);
                        });
                    })
                    .catch(function (error) {
                        console.error("Lỗi API: ", error);
                        let msg = error.data?.message || "Đã xảy ra lỗi. Vui lòng thử lại sau!";
                        Swal.fire('Lỗi hệ thống', msg, 'error');
                    });
            }
        });
    };

});
