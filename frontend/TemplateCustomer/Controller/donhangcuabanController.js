
app.controller('donhangcuabanController', function ($scope, $http, $location) {
    const apiProvince = "https://localhost:7196/api/ghn/master-data/province";
    const apiDistrict = "https://localhost:7196/api/ghn/master-data/district";
    const apiWard = "https://localhost:7196/api/ghn/master-data/ward";
    
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
    // select hóa đơn theo mã khách hàng
    
    const userInfo = JSON.parse(localStorage.getItem('userInfo'));
    
    let idkh = userInfo.id;
    $scope.idkh =userInfo.id;
    // Hàm gọi API lấy dữ liệu hóa đơn
    $scope.loadHoaDon = function () {
        let api = "https://localhost:7196/api/Hoadons/hoa-don-theo-ma-kh-" + idkh;
        if ($scope.searchText && $scope.searchText.trim() !== "") {
            api += `?search=${encodeURIComponent($scope.searchText)}`;
        }
    
        $http.get(api)
            .then(function (response) {
                // Thêm thuộc tính checktrahang vào mỗi hóa đơn
                $scope.dataHoaDon = response.data.map(function(hd) {
                    // Kiểm tra xem có trả hàng nào thuộc hóa đơn này không
                    hd.checktrahang = $scope.dataTraHang.some(function(item) {
                        return item.idhd === hd.id;
                    });
                    console.log(`Hóa đơn ${hd.id} - checktrahang: ${hd.checktrahang}`);
                    return hd;
                });
                
                console.log("Danh sách hóa đơn sau khi thêm checktrahang:", $scope.dataHoaDon);
                
                // Gọi API lấy danh sách phương thức thanh toán
                return $http.get("https://localhost:7196/api/PhuongThucThanhToans");
            })
            .then(function (response) {
                const dataPTTT = response.data;
                
                // Gán tên phương thức thanh toán tương ứng vào từng hóa đơn
                $scope.dataHoaDon.forEach(async function (hd) {
                    const pttt = dataPTTT.find(pt => pt.id === hd.idpttt);
                    if (pttt && pttt.tenpttt === "Chuyển khoản ngân hàng") {
                        hd.tenpttt = 1;
                    } else {
                        hd.tenpttt = 0;
                    }
    
                    if (hd.diachiship) {
                        const parts = hd.diachiship.split(' - ');
                        if (parts.length === 4) {
                            const [diachicuthe, phuongxaIdStr, quanhuyenIdStr, thanhphoIdStr] = parts;
                            const idphuongxa = parseInt(phuongxaIdStr);
                            const idquanhuyen = parseInt(quanhuyenIdStr);
                            const idthanhpho = parseInt(thanhphoIdStr);
        
                            // Lưu lại các id địa chỉ
                            hd.idphuongxa = idphuongxa;
                            hd.idquanhuyen = idquanhuyen;
                            hd.idthanhpho = idthanhpho;
                            hd.diachicuthe = diachicuthe;
        
                            // Gọi API lấy tên địa chỉ
                            const [phuongxa, quanhuyen, thanhpho] = await Promise.all([
                                getWardName(idquanhuyen, idphuongxa),
                                getDistrictName(idthanhpho, idquanhuyen),
                                getProvinceName(idthanhpho)
                            ]);
        
                            hd.diachiship_display = `${diachicuthe || ''} - ${phuongxa} - ${quanhuyen} - ${thanhpho}`;
                        } else {
                            hd.diachiship_display = "Địa chỉ không hợp lệ";
                        }
                    } else {
                        hd.diachiship_display = "Không có địa chỉ";
                    }
                });
            })
            .catch(function (error) {
                console.error("Lỗi khi lấy dữ liệu:", error);
            });
    
        if ($scope.dataHoaDon == null) {
            return $scope.thongbaotimkiem = "Không tìm thấy mã hóa đơn " + $scope.searchText;
        }
    };


    // Theo dõi sự thay đổi của searchText
    $scope.$watch("searchText", function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.loadHoaDon();
        }
    });

    $scope.files = []; // Lưu file upload
    $scope.imagePreview = []; // Lưu link preview ảnh
    $scope.review = {}; // Đối tượng đánh giá

    // Mở modal đánh giá
    $scope.openRatingModal = function (product,) {
        $scope.selectedProduct = product;
        $scope.review = {
            content: '',
            rating: 5,
            ngaydanhgia: new Date().toISOString(),
            idhdct: product.id,
            idkh: idkh
        };
        $scope.totalSelectedFiles = 0;
        try {
            const myModal = new bootstrap.Modal(document.getElementById('ratingModal'), { keyboard: false });
            myModal.show();
        } catch (err) {
            console.error('Error while opening modal:', err);
        }
    };

    $scope.AnhDanhGia = function (id) {
        $http.get(`https://localhost:7196/api/Hinhanh/DanhGia/${id}`)
            .then(function (response) {
                // Lấy ảnh từ API và thêm vào danh sách xem trước
                $scope.imagePreview = response.data.map(item => ({
                    id: item.id,       // ID của ảnh từ API
                    url: item.url      // URL của ảnh
                }));

                // Cập nhật tổng số file
                $scope.totalSelectedFiles = $scope.imagePreview.length;
                
            })
            .catch(function (error) {
                console.error("Lỗi khi lấy ảnh đánh giá:", error);
            });
    };




    $scope.openEditRatingModal = function (product) {
        const currentRating = $scope.danhgiaById[product.id];
        if (!currentRating) {
            alert("Không tìm thấy đánh giá để sửa.");
            return;
        }

        // Thiết lập thông tin sản phẩm và đánh giá
        $scope.selectedProduct = product;
        $scope.review = {
            content: currentRating.noidungdanhgia,
            rating: currentRating.sosao,
            ngaydanhgia: new Date().toISOString(),
            idhdct: product.id,
            idkh: currentRating.idkh,
            id: currentRating.id // ID đánh giá để sửa
        };
        $scope.files = []; // Lưu file upload
        $scope.imagePreview = []; // Lưu link preview ảnh
        $scope.totalSelectedFiles = 0; // Tổng số file đã chọn

        $scope.AnhDanhGia(currentRating.id);
        // Xử lý ảnh tải lên (nếu có file input thay đổi)
        const inputElement = document.getElementById("imageUpload");
        if (inputElement && inputElement.files.length > 0) {
            const mockEvent = { target: { files: inputElement.files } };
            $scope.handleFileSelect(mockEvent);
        }
        $scope.totalSelectedFiles = $scope.imagePreview.length;
        


        // Hiển thị modal
        try {
            const editModal = new bootstrap.Modal(document.getElementById('ratingModal'), { keyboard: false });
            editModal.show();
        } catch (err) {
            console.error('Error while opening edit modal:', err);
        }
    };



    $scope.submitReview = function () {
        // Kiểm tra thông tin bắt buộc đã được nhập hay chưa
        if (!$scope.review.rating || !$scope.review.content) {
            alert('Vui lòng điền đầy đủ thông tin trước khi gửi!');
            return;
        }

        // Tạo đối tượng FormData để gửi dữ liệu
        var formData = new FormData();
        formData.append('Idkh', $scope.review.idkh); // ID khách hàng
        formData.append('Noidungdanhgia', $scope.review.content); // Nội dung đánh giá
        formData.append('Ngaydanhgia', $scope.review.ngaydanhgia); // Ngày đánh giá
        formData.append('Idhdct', $scope.review.idhdct); // ID hóa đơn chi tiết
        formData.append('Sosao', $scope.review.rating); // Số sao đánh giá

        if ($scope.imagePreview && $scope.imagePreview.length > 0) {
            for (let i = 0; i < $scope.imagePreview.length; i++) {
                const image = $scope.imagePreview[i];

                // Nếu là ảnh mới tải lên (File thật, `id` là null), thêm vào 'files'
                if (image.id === null) {
                    formData.append('files', image.file); // `file` là đối tượng File
                }

                // Nếu là ảnh đã có (URL từ API, có `id`), gửi ID ảnh
                if (image.id !== null) {
                    formData.append('existingFileIds', image.id); // Gửi ID của ảnh đã tồn tại
                }
            }
        }

        // Xác định API và phương thức HTTP (POST cho thêm mới, PUT cho sửa)
        const isEditing = !!$scope.review.id; // Kiểm tra nếu có ID thì đang sửa
        const apiUrl = isEditing
            ? `https://localhost:7196/api/Danhgia/DanhGia/${$scope.review.id}` // API sửa
            : 'https://localhost:7196/api/Danhgia'; // API thêm mới
        const method = isEditing ? 'PUT' : 'POST'; // Phương thức HTTP

        // Gửi yêu cầu HTTP
        $http({
            method: method,
            url: apiUrl,
            data: formData,
            headers: { 'Content-Type': undefined }, // Để Angular tự xử lý header cho FormData
            transformRequest: angular.identity // Đảm bảo dữ liệu FormData không bị thay đổi
        })
            .then(function (response) {

                // Hiển thị thông báo thành công
                alert(isEditing ? 'Cập nhật đánh giá thành công!' : 'Cảm ơn bạn đã gửi đánh giá!');

                // Cập nhật dữ liệu sau khi thêm hoặc sửa
                const updatedRating = {
                    id: isEditing ? $scope.review.id : response.data.id, // Lấy ID từ phản hồi nếu thêm mới
                    idkh: $scope.review.idkh,
                    idhdct: $scope.review.idhdct,
                    sosao: $scope.review.rating,
                    noidungdanhgia: $scope.review.content,
                    ngaydanhgia: $scope.review.ngaydanhgia
                };

                // Lưu hoặc cập nhật vào danh sách đánh giá
                $scope.danhgiaById[$scope.review.idhdct] = updatedRating;

                // Đóng modal và reset dữ liệu
                $("#ratingModal").modal("hide");
                $scope.files = [];
                $scope.imagePreview = [];
                $scope.review = {};
            })
            .catch(function (error) {
                console.error('Lỗi khi gửi đánh giá:', error);

                // Hiển thị thông báo lỗi
                alert('Đã xảy ra lỗi khi gửi đánh giá. Vui lòng thử lại!');
            });
    };



    $scope.deleteRating = function (id) {
        if (!confirm("Bạn có chắc chắn muốn xóa đánh giá này?")) {
            return;
        }
        $http.delete(`https://localhost:7196/api/Danhgia/${id}`)
            .then(function (response) {
                alert("Đã xóa đánh giá!");

                // Xóa đánh giá khỏi danh sách
                Object.keys($scope.danhgiaById).forEach(function (key) {
                    if ($scope.danhgiaById[key] && $scope.danhgiaById[key].id === id) {
                        delete $scope.danhgiaById[key];
                    }
                });
            })
            .catch(function (error) {
                console.error("Lỗi khi xóa đánh giá:", error);
                alert("Đã xảy ra lỗi khi xóa đánh giá. Vui lòng thử lại!");
            });
    };



    // Thiết lập đánh giá sao
    $scope.setRating = function (star) {
        $scope.review.rating = star;
    };

    $scope.handleFileSelect = function (event) {
        const files = event.target.files;

        // Tính tổng số ảnh (bao gồm cả ảnh đã có và ảnh đang chọn mới)
        const totalFiles = $scope.imagePreview.length + files.length;

        // Kiểm tra nếu tổng số ảnh vượt quá 5
        if (totalFiles > 5) {
            alert("Bạn chỉ được chọn tối đa 5 ảnh.");
            return;
        }

        for (let i = 0; i < files.length; i++) {
            const file = files[i];

            // Chỉ chấp nhận file ảnh
            if (!file.type.startsWith("image/")) {
                alert("Chỉ hỗ trợ định dạng ảnh.");
                continue;
            }

            // Thêm file vào danh sách
            $scope.files.push(file);


            // Tạo URL xem trước ảnh
            const reader = new FileReader();
            reader.onload = function (e) {
                $scope.$apply(function () {
                    $scope.imagePreview.push({
                        id: null,          // Ảnh mới không có ID
                        url: e.target.result,
                        file: file         // Gắn đối tượng File thật
                    });

                    // Cập nhật số lượng ảnh
                    $scope.totalSelectedFiles = $scope.imagePreview.length;
                });
            };
            reader.readAsDataURL(file);

        }
    };




    // Xóa ảnh khỏi danh sách
    $scope.removeImage = function (index) {
        // Xóa file khỏi danh sách upload nếu là ảnh mới
        if (index < $scope.files.length) {
            $scope.files.splice(index, 1);
        }

        // Xóa ảnh khỏi danh sách xem trước
        $scope.imagePreview.splice(index, 1);

        // Cập nhật lại tổng số ảnh đã chọn
        $scope.totalSelectedFiles = $scope.imagePreview.length;
    };

    // Hàm tải đánh giá cho HDCT
    $scope.getRatingForHdct = function (id) {
        // Khởi tạo danh sách nếu chưa có
        if (!$scope.danhgiaById) {
            $scope.danhgiaById = {};
            $scope.isLoadingById = {}; // Cờ để kiểm tra trạng thái đang tải
        }

        if ($scope.danhgiaById[id] || $scope.isLoadingById[id]) {
            return $scope.danhgiaById[id];
        }

        $scope.isLoadingById[id] = true;

        // Gọi API để lấy đánh giá
        $http.get(`https://localhost:7196/byIDhdct/${id}`)
            .then(function (response) {
                // Lưu dữ liệu vào danh sách
                $scope.danhgiaById[id] = response.data;

            })
            .catch(function (error) {
                console.error(`Lỗi khi tải đánh giá cho HDCT ${id}:`, error);
                $scope.danhgiaById[id] = null; // Gán null nếu không có dữ liệu
            })
            .finally(function () {
                // Kết thúc trạng thái đang tải
                $scope.isLoadingById[id] = false;
            });

        // Trả về mặc định ban đầu cho đến khi API trả về kết quả
        return null;
    };

    $scope.loadAllDanhGia = function (listHdct) {
        if (!listHdct || !Array.isArray(listHdct)) return;

        listHdct.forEach(function (hdct) {
            // Gọi API để lấy đánh giá của từng hóa đơn chi tiết
            $scope.getRatingForHdct(hdct.id);
        });
    };
    $scope.deleteRating = function (id) {
        if (!confirm("Bạn có chắc chắn muốn xóa đánh giá này?")) {
            return;
        }
        $http.delete(`https://localhost:7196/api/Danhgia/_KhachHang/${id}`)
            .then(function (response) {
                alert("Đã xóa đánh giá!");

                // Tìm idhdct liên quan để cập nhật
                for (let hdctId in $scope.danhgiaById) {
                    if ($scope.danhgiaById[hdctId] && $scope.danhgiaById[hdctId].id === id) {
                        // Cập nhật trạng thái đánh giá thành false
                        $scope.danhgiaById[hdctId].success = false;
                        break;
                    }
                }
            })
            .catch(function (error) {
                console.error("Lỗi khi xóa đánh giá:", error);
                alert("Đã xảy ra lỗi khi xóa đánh giá. Vui lòng thử lại!");
            });
    };

    // Gọi loadHoaDonCT trong hàm init
    $scope.init = function () {
        $scope.loadHoaDonCT();
    };


    $http.get('https://localhost:7196/api/Hoadonchitiets')
        .then(function (response) {
            $scope.dataHoaDonCT = response.data
            $scope.loadAllDanhGia($scope.dataHoaDonCT);

        })
        .catch(function (error) {
            console.error(error)
        })




    $scope.chitietdh = function (id) {
        $scope.idhd = id;
        $("#chitietModal").modal("show");
    };
    $scope.sanphamctById = {};
    $http.get("https://localhost:7196/api/Sanphamchitiets/")
        .then(function (response) {
            $scope.sanphamct = response.data;
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy dữ liệu:", error);
        });
    $http.get("https://localhost:7196/api/Sanphams")
        .then(function (response) {
            $scope.sanphamList = response.data;
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy dữ liệu:", error);
        });

    $http.get("https://localhost:7196/api/Thuonghieu")
        .then(function (response) {
            $scope.ThuonghieuList = response.data;
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy dữ liệu:", error);
        });
    $scope.SelecttenSp = function (id) {
        if (!$scope.sanphamList || $scope.sanphamList.length === 0) {
            return "Đang tải...";
        }

        let sanpham = $scope.sanphamList.find(x => x.id == id);
        return sanpham ? sanpham.tenSanpham : "Không tìm thấy sản phẩm";
    };
    $scope.SelectTenTH = function (id) {
        if (!$scope.sanphamList || $scope.sanphamList.length === 0 ||
            !$scope.ThuonghieuList || $scope.ThuonghieuList.length === 0) {
            return "Đang tải...";
        }

        let sanpham = $scope.sanphamList.find(x => x.id == id);
        if (!sanpham) {
            return "Không tìm thấy sản phẩm";
        }

        let thuonghieu = $scope.ThuonghieuList.find(x => x.id == sanpham.idth);
        return thuonghieu ? thuonghieu.tenthuonghieu : "Không tìm thấy thương hiệu";
    };
    $http.get("https://localhost:7196/api/Phuongthucthanhtoans/")
        .then(function (response) {
            $scope.ListPttt = response.data;
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy dữ liệu:", error);
            $scope.ListPttt = "Lỗi tải dữ liệu";
        });
    $http.get("https://localhost:7196/api/Khachhangs/" + idkh)
        .then(function (response) {
            $scope.dataTttk = response.data;
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy dữ liệu:", error);
            Swal.fire("Lỗi!", "Không thể lấy thông tin tài khoản. Vui lòng thử lại!", "error");
        });

    $http.get("https://localhost:7196/api/Size")
        .then(function (response) {
            $scope.listSize = response.data;
        })
        .catch(function (error) {
            console.error(error);
        })
    $http.get("https://localhost:7196/api/Color")
        .then(function (response) {
            $scope.listColor = response.data;
        })
        .catch(function (error) {
            console.error(error);
        })

    $http.get("https://localhost:7196/api/ChatLieu")
        .then(function (response) {
            $scope.listChatlieu = response.data;
        })
        .catch(function (error) {
            console.error(error);
        })
    $scope.idhd = null;
    $scope.huydh = function (id) {
        $scope.idhd = id;
        $("#HuyDonModal").modal("show");
    };

    $scope.huydonhang = function () {
        if (!$scope.lydohuy) {
            Swal.fire({
                icon: "error",
                title: "Vui lòng chọn lý do hủy đơn!",
            });
            return;
        }

        let lydohuydon = ($scope.lydohuy === "khác") ? $scope.lydokhac : $scope.lydohuy;

        Swal.fire({
            title: "Bạn có chắc muốn hủy đơn hàng?",
            text: "Sau khi hủy, bạn không thể khôi phục lại đơn hàng!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Hủy đơn hàng",
            cancelButtonText: "Quay lại"
        }).then((result) => {
            if (result.isConfirmed) {
                let api = "https://localhost:7196/api/Hoadons/" + $scope.idhd;

                // ✅ Gọi API GET trước
                $http.get(api)
                    .then(function (response) {
                        let dataHoaDon = response.data;

                        // ✅ Chuẩn bị dữ liệu cập nhật
                        let data = {
                            id: dataHoaDon.id,
                            idnv: dataHoaDon.idnv,
                            idkh: dataHoaDon.idkh,
                            trangthaithanhtoan: dataHoaDon.trangthaithanhtoan,
                            trangthaidonhang: dataHoaDon.trangthaidonhang,
                            thoigiandathang: dataHoaDon.thoigiandathang,
                            diachiship: dataHoaDon.diachiship,
                            ngaygiaothucte: dataHoaDon.ngaygiaothucte,
                            tongtiencantra: dataHoaDon.tongtiencantra,
                            tongtiensanpham: dataHoaDon.tongtiensanpham,
                            sdt: dataHoaDon.sdt,
                            tonggiamgia: dataHoaDon.tonggiamgia,
                            idgg: dataHoaDon.idgg,
                            trangthaidonhang: 4, // ✅ Cập nhật trạng thái
                            phivanchuyen: dataHoaDon.phivanchuyen,
                            idpttt: dataHoaDon.idpttt,
                            ghichu: "Huỷ đơn hàng với lý do: " + lydohuydon
                        };
                        console.log(data);
                        // ✅ Gọi API PUT để cập nhật trạng thái đơn hàng
                        return $http.put(api, data);
                    })
                    .then(function () {
                        // ✅ Hủy đơn thành công → Ẩn modal, reset form
                        $("#HuyDonModal").modal("hide");

                        // ✅ Reset radio & lý do hủy đơn
                        $scope.lydohuy = null;
                        $scope.lydokhac = "";

                        setTimeout(() => {
                            document.querySelectorAll('input[name="flexRadioDefault"]').forEach(radio => {
                                radio.checked = false;
                            });
                        }, 300);

                        // ✅ Hiển thị thông báo thành công
                        Swal.fire({
                            icon: "success",
                            title: "Hủy đơn hàng thành công!",
                            text: "Đơn hàng của bạn đã được hủy.",
                            timer: 2000,
                            showConfirmButton: false
                        });

                        // ✅ Reload lại trang
                        setTimeout(() => {
                            location.reload();
                            window.scroll(0, 0);
                        }, 2000);
                    })
                    .catch(function (error) {
                        console.error("Lỗi khi hủy đơn hàng:", error);

                        Swal.fire({
                            icon: "error",
                            title: "Hủy đơn không thành công!",
                            text: "Vui lòng thử lại sau.",
                        });
                    });
            }
        });
    };
    
    $scope.thanhtoan = function (idHoaDon) {
        if (!idHoaDon) {
            console.warn("ID hóa đơn không hợp lệ!");
            return;
        }
    
        const apiUrl = `https://localhost:7196/api/Checkout/${idHoaDon}`;
    
        $http.get(apiUrl)
            .then(function (response) {
                // Lấy ID trả về và chuyển hướng đến trang thanh toán
                const checkoutId = response.data.id;
                if (checkoutId) {
                    const paymentUrl = `https://pay.payos.vn/web/${checkoutId}`;
                    window.location.href = paymentUrl;
                } else {
                    alert("Không lấy được thông tin thanh toán!");
                }
            })
            .catch(function (error) {
                console.error("Lỗi khi thanh toán:", error);
                alert("Thanh toán thất bại!");
            });
    };    
    // Load danh sách trả hàng
    $http.get("https://localhost:7196/api/Trahangs")
        .then(function(response) {
            $scope.dataTraHang = response.data;
            console.log("$scope.dataTraHang", $scope.dataTraHang );
            $scope.loadHoaDon();
        })
        .catch(function(error) {
            console.error("Lỗi lấy danh sách trả hàng: ", error);
        });

    // Load chi tiết trả hàng theo ID
    $http.get("https://localhost:7196/api/Trahangchitiets/SanphamByThct")
                .then(function(response) {
                    $scope.chitietTraHang = response.data;
                    
                })
                .catch(function(error) {
                    console.error("Lỗi chi tiết trả hàng: ", error);
                });
    $scope.itemsPerPage = 5;
    $scope.currentPage = 1;

    $scope.getPageNumbers = function () {
        const filtered = $scope.dataHoaDon.filter(hd => hd.idkh == $scope.idkh);
        $scope.totalPages = Math.ceil(filtered.length / $scope.itemsPerPage);
        return Array.from({ length: $scope.totalPages }, (_, i) => i + 1);
    };
    $scope.getPageNumberTraHangs = function () {
        const filtered = $scope.dataTraHang.filter(hd => hd.idkh == $scope.idkh);
        $scope.totalPages = Math.ceil(filtered.length / $scope.itemsPerPage);
        return Array.from({ length: $scope.totalPages }, (_, i) => i + 1);
    };
    $scope.setPage = function (page) {
        if (page >= 1 && page <= $scope.totalPages) {
            $scope.currentPage = page;
            window.scroll(0,0);
        }
    };

    $scope.prevPage = function () {
        if ($scope.currentPage > 1) {
            $scope.currentPage--;
            window.scroll(0,0);
        }
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.totalPages) {
            $scope.currentPage++;
            window.scroll(0,0);
        }
    };

    // Hàm lấy tên tỉnh/thành phố
async function getProvinceName(id) {
    try {
        const response = await fetch(apiProvince, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({})
        });

        const data = await response.json();
        if (data.code === 200) {
            const province = data.data.find(p => p.ProvinceID == id);
            return province ? province.NameExtension[1] : "Không xác định";
        }
    } catch (error) {
        // Không log lỗi ra console
    }
    return "Không xác định";
}

// Hàm lấy tên quận/huyện
async function getDistrictName(province_id, district_id) {
    try {
        const response = await fetch(apiDistrict, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ province_id: Number(province_id) })
        });

        const data = await response.json();
        if (data.code === 200) {
            const district = data.data.find(d => d.DistrictID == district_id);
            return district ? district.DistrictName : "Không xác định";
        }
    } catch (error) {
        // Không log lỗi ra console
    }
    return "Không xác định";
}

// Hàm lấy tên phường/xã
async function getWardName(district_id, ward_id) {
    try {
        const response = await fetch(apiWard, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ district_id: Number(district_id) })
        });

        const data = await response.json();
        if (data.code === 200) {
            const ward = data.data.find(w => w.WardCode == ward_id);
            return ward ? ward.WardName : "Không xác định";
        }
    } catch (error) {
        // Không log lỗi ra console
    }
    return "Không xác định";
}

    $scope.doitra = function(x) {
        if (!x.ngaygiaothucte) {
            return false;
        }
    
        const ngayGiao = new Date(x.ngaygiaothucte);
        const homNay = new Date();
    
        // Đặt cả hai ngày về đầu ngày để so sánh chính xác theo số ngày
        homNay.setHours(0, 0, 0, 0);
        ngayGiao.setHours(0, 0, 0, 0);
    
        const bayNgay = 7 * 24 * 60 * 60 * 1000;
        const khoangThoiGian = homNay - ngayGiao;
        if((khoangThoiGian <= bayNgay && khoangThoiGian >= 0)&&x.trangthai==3){
            return true;
        }
        return false;
    }

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
});

