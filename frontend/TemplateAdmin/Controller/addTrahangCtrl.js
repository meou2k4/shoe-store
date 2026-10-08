app.controller("addTrahangCtrl", function ($http, $scope, $location, $routeParams, $timeout) {
    $scope.idhd = $routeParams.id;
    $http.get(`https://localhost:7196/api/Hoadons/${$scope.idhd}`)
        .then(function (response) {
            $scope.hoadonmua = response.data;
            console.log($scope.hoadon);
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy thông tin hóa đơn:", error);
        });
    window.scrollTo(0, 0);
    console.log($scope.idhd);
    const userInfoString = localStorage.getItem("userInfo1");
    const userInfo = JSON.parse(userInfoString);
    console.log(userInfo);

    // Initialize variables
    $scope.selectedProducts = []; // Danh sách sản phẩm đã chọn
    $scope.images = []; // Mảng lưu trữ ảnh
    $scope.imageCount = 0;
    $scope.provinces = [];
    $scope.districts = [];
    $scope.wards = [];
    $scope.fullAddress = "";
    $scope.estimatedDeliveryDate = null;
    $scope.ListNganHang = [];
    const apiProvince = "https://localhost:7196/api/ghn/master-data/province";
    const apiDistrict = "https://localhost:7196/api/ghn/master-data/district";
    const apiWard = "https://localhost:7196/api/ghn/master-data/ward";
    const apiAddressList = "https://localhost:7196/api/Diachi";
    const apiKHUrl = "https://localhost:7196/api/Khachhangs";

    // Thông tin địa chỉ đã chọn
    $scope.selectedInfo = {
        provinceId: null,
        provinceName: "",
        districtId: null,
        districtName: "",
        wardCode: null,
        wardName: ""
    };

    // Gọi API để lấy danh sách sản phẩm
    $http.get("https://localhost:7196/api/Trahangchitiets/ListSanPhamByIdhd/" + $scope.idhd)
        .then(function (response) {
            $scope.dataSp = response.data.map(sp => ({
                ...sp,
                maxsoluong: sp.soluong || 0, // Lưu số lượng tối đa từ API
                soluong: 1, // Gán số lượng mặc định là 1
                thanhtien: sp.giasp || 0, // Tính thành tiền ban đầu
                selected: false // Mặc định chưa chọn
            }));
            $scope.tinhTongTien();
            console.log($scope.dataSp)
        })
        .catch(error => console.error(error));

    // Hàm tăng giảm số lượng
    $scope.increase = function (sp) {
        if (sp.soluong < sp.maxsoluong) {
            sp.soluong++;
            sp.thanhtien = sp.soluong * sp.giasp;
            $scope.updateSelectedProduct(sp); // Cập nhật số lượng trong danh sách đã chọn
            $scope.tinhTongTien();
        }
    };

    $scope.decrease = function (sp) {
        if (sp.soluong > 1) {
            sp.soluong--;
            sp.thanhtien = sp.soluong * sp.giasp;
            $scope.updateSelectedProduct(sp); // Cập nhật số lượng trong danh sách đã chọn
            $scope.tinhTongTien();
        }
    };

    // Hàm tính tổng tiền
    $scope.tinhTongTien = function () {
        let total = 0;
        $scope.selectedProducts.forEach(p => {
            let sp = $scope.dataSp.find(item => item.id === p.id);
            if (sp) {
                p.soluong = sp.soluong; // Cập nhật số lượng trong danh sách đã chọn
                total += sp.soluong * sp.giasp;
            }
        });
        $scope.tongtien = total;
        $scope.updateSelectAll(); // Gọi để cập nhật trạng thái "Chọn tất cả"
    };

    $scope.toggleProductSelection = function (sp) {
        if (sp.selected) {
            let existingProduct = $scope.selectedProducts.find(p => p.id === sp.id);
            if (!existingProduct) {
                $scope.selectedProducts.push({ id: sp.id, soluong: sp.soluong });
            }
        } else {
            $scope.selectedProducts = $scope.selectedProducts.filter(p => p.id !== sp.id);
        }
        console.log("Danh sách sản phẩm đã chọn:", $scope.selectedProducts);
        $scope.tinhTongTien();
    };
    $scope.validateQuantity = function(sp) {
        const min = 1;
        const max = sp.maxsoluong;
    
        if (!sp.soluong || sp.soluong < min) {
            sp.soluong = min;
        } else if (sp.soluong > max) {
            sp.soluong = max;
        }
    };
    $scope.updateQuantity = function (sp) {
        if (sp.soluong < 1) {
            sp.soluong = 1; // Không cho số lượng nhỏ hơn 1
        } else if (sp.soluong > sp.maxsoluong) {
            sp.soluong = sp.maxsoluong; // Không cho vượt quá số lượng tối đa
        }

        sp.thanhtien = sp.soluong * sp.giasp; // Cập nhật thành tiền
        $scope.updateSelectedProduct(sp); // Cập nhật số lượng trong danh sách đã chọn
        $scope.tinhTongTien(); // Cập nhật tổng tiền
    };

    $scope.toggleAll = function () {
        $scope.selectedProducts = []; // Reset danh sách sản phẩm đã chọn

        $scope.dataSp.forEach(sp => {
            sp.selected = $scope.selectAll;
            if ($scope.selectAll) {
                $scope.selectedProducts.push({ id: sp.id, soluong: sp.soluong });
            }
        });

        console.log("Danh sách sản phẩm đã chọn:", $scope.selectedProducts);
        $scope.tinhTongTien();
    };

    // Cập nhật trạng thái "Chọn tất cả"
    $scope.updateSelectedProduct = function (sp) {
        let selectedProduct = $scope.selectedProducts.find(p => p.id === sp.id);
        if (selectedProduct) {
            selectedProduct.soluong = sp.soluong; // Cập nhật số lượng
            console.log($scope.selectedProducts)
        }
    };

    $scope.updateSelectAll = function () {
        $scope.selectAll = $scope.dataSp.every(sp => sp.selected);
    };




    // ========== Image Handling Functions ==========
    // Mở file picker khi click vào box "Thêm hình ảnh"
    $scope.openFileInput = function () {
        document.getElementById("fileInput").click();
    };

    // Xử lý chọn ảnh từ thư viện
    $scope.selectImages = function (event) {
        if (!$scope.images) $scope.images = [];
        const files = event.target.files;

        if ($scope.images.length >= 3) {
            alert("Bạn chỉ có thể chọn tối đa 3 ảnh!");
            event.target.value = '';
            return;
        }

        for (let i = 0; i < files.length; i++) {
            if ($scope.images.length >= 3) break;

            const file = files[i];
            const objectURL = URL.createObjectURL(file); // Lấy URL trực tiếp

            $scope.$apply(() => {
                $scope.images.push({
                    url: objectURL,
                    file: file
                });
                $scope.imageCount = $scope.images.length;
            });
        }

        event.target.value = '';
    };

    // Xóa ảnh đã chọn
    $scope.removeImage = function (index) {
        URL.revokeObjectURL($scope.images[index].url); // Giải phóng URL blob
        $scope.images.splice(index, 1);
        $scope.imageCount = $scope.images.length;
    };

    // Bật camera
    let videoStream = null;
    $scope.startCamera = function () {
        let video = document.getElementById("cameraFeed");

        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                video.srcObject = stream;
                videoStream = stream;
            })
            .catch(function (error) {
                console.error("Không thể truy cập camera!", error);
            });
    };

    // Chụp ảnh từ camera
    $scope.capturePhoto = function () {
        if ($scope.images.length >= 3) {
            alert("Bạn chỉ có thể chọn tối đa 3 ảnh!");
            return;
        }

        let video = document.getElementById("cameraFeed");
        let canvas = document.getElementById("cameraCanvas");
        let context = canvas.getContext("2d");

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        canvas.toBlob((blob) => {
            const objectURL = URL.createObjectURL(blob);

            $scope.$apply(() => {
                $scope.images.push({
                    url: objectURL,
                    file: blob
                });
                $scope.imageCount = $scope.images.length;
            });
        }, 'image/png', 0.9); // Lưu ảnh dưới dạng PNG

        // Tắt camera sau khi chụp
        if (videoStream) {
            let tracks = videoStream.getTracks();
            tracks.forEach(track => track.stop());
            video.srcObject = null;
        }

        // Đóng modal sau khi chụp
        setTimeout(() => {
            document.querySelector("#cameraModal .btn-close").click();
        }, 500);
    };

    // Hàm upload ảnh
    function uploadImages(idtrahang) {
        if (!$scope.images || $scope.images.length === 0) {
            alert("Vui lòng chọn ít nhất một ảnh!");
            return Promise.reject("Không có ảnh để upload");
        }

        $scope.isUploading = true;
        let successCount = 0;
        let failedCount = 0;
        const uploadResults = [];

        // Xử lý tuần tự từng ảnh
        const processImagesSequentially = async () => {
            for (let index = 0; index < $scope.images.length; index++) {
                const imageData = $scope.images[index];

                try {
                    const result = await processSingleImage(imageData, index, idtrahang);
                    uploadResults.push(result);
                    successCount++;
                    console.log(`Upload ảnh ${index + 1}/${$scope.images.length} thành công`);
                } catch (error) {
                    failedCount++;
                    console.error(`Lỗi khi upload ảnh ${index + 1}:`, error);
                    uploadResults.push({ success: false, index: index, error: error });
                }
            }

            return { total: $scope.images.length, success: successCount, failed: failedCount, results: uploadResults };
        };

        // Hàm xử lý upload từng ảnh
        const processSingleImage = (imageData, index, idtrahang) => {
            return new Promise((resolve, reject) => {
                const canvas = document.createElement('canvas');
                const img = new Image();

                img.onload = function () {
                    canvas.width = img.width;
                    canvas.height = img.height;
                    const ctx = canvas.getContext('2d');
                    ctx.drawImage(img, 0, 0);

                    // Chuyển đổi sang PNG
                    canvas.toBlob((blob) => {
                        const formData = new FormData();
                        const fileName = `image_${idtrahang}_${Date.now()}_${index}.png`;

                        formData.append('file', blob, fileName);
                        formData.append('idtrahang', idtrahang);
                        formData.append('Iddanhgia', 0);
                        formData.append('Urlhinhanh', fileName);

                        $http.post('https://localhost:7196/api/Hinhanh', formData, {
                            headers: { 'Content-Type': undefined },
                            transformRequest: angular.identity
                        })
                            .then(response => {
                                resolve({ success: true, index: index, fileName: fileName, response: response.data });
                            })
                            .catch(error => {
                                reject(error);
                            });
                    }, 'image/png', 0.9); // Lưu ảnh dưới dạng PNG
                };

                img.onerror = () => reject(new Error("Không thể tải ảnh"));
                img.src = imageData.url;
            });
        };

        return processImagesSequentially().finally(() => {
            $scope.$apply(() => {
                $scope.isUploading = false;
            });
        });
    }

    // ========== Bank List ==========
    $scope.ListNganHang = [];
    $scope.selectedBank = "";
    $scope.accountNumber = "";
    $scope.accountName = "";
    $http.get("https://api.vietqr.io/v2/banks")
        .then(function (response) {
            if (response.data && response.data.data) {
                $scope.ListNganHang = response.data.data;
                console.log("Danh sách ngân hàng:", $scope.ListNganHang);
            } else {
                console.log("Dữ liệu ngân hàng không hợp lệ:", response);
            }
        })
        .catch(function (error) {
            console.error("Lỗi khi lấy danh sách ngân hàng:", error);
        });
    // ========== Submit Function ==========
    $scope.btnAdd = function () {
        let errorMessages = [];
    
        // === 1. Kiểm tra hợp lệ dữ liệu đầu vào ===
        if (!$scope.selectedProducts || $scope.selectedProducts.length === 0) {
            errorMessages.push("Vui lòng chọn ít nhất một sản phẩm.");
        }
        if (!$scope.returnReason) {
            errorMessages.push("Vui lòng nhập lý do trả hàng.");
        }
        if (!$scope.images || $scope.images.length === 0) {
            errorMessages.push("Vui lòng tải lên ít nhất một hình ảnh làm bằng chứng.");
        }
        if ($scope.refundMethod === 'bank' && (!$scope.bankName || !$scope.accountNumber)) {
            errorMessages.push("Vui lòng nhập ngân hàng & số tài khoản.");
        }
    
        if (errorMessages.length > 0) {
            Swal.fire("Lỗi!", errorMessages.join("<br>"), "error");
            return;
        }
    
        // === 2. Hiển thị xác nhận gửi yêu cầu ===
        Swal.fire({
            title: "Xác nhận trả hàng?",
            text: "Bạn có chắc chắn muốn gửi yêu cầu trả hàng không?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Có, gửi yêu cầu!",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (!result.isConfirmed) return;

            if ($scope.refundMethod === "Thẻ tín dụng/ghi nợ/Tài khoản ngân hàng") {
                stk = $scope.accountNumber || "";
                tentaikhoan = ($scope.accountName || "").toUpperCase();
                nganhang = $scope.selectedBank ? $scope.selectedBank.name + ' (' + $scope.selectedBank.shortName + ')' : "";
            }

            const data = {
                tenkhachhang: $scope.tenkhachang || "Khách vãng lai",
                idnv: userInfo?.id || 0,
                idkh: $scope.hoadonmua.idkh||0,
                sotienhoan: $scope.tongtien ?? 0,
                lydotrahang: $scope.returnReason || "Không có lý do",
                trangthai: 1,
                phuongthuchoantien: "Thẻ tín dụng/ghi nợ/Tài khoản ngân hàng",
                ngaytrahangthucte: new Date(),
                chuthich: $scope.mota || "Không có chú thích",
                hinhthucxuly: $scope.hinhthucxuly || "Không xác định",
                tennganhang: $scope.selectedBank.shortName,
                sotaikhoan: $scope.accountNumber,
                tentaikhoan: $scope.accountName,
                trangthaihoantien: 1,
                diachiship: "8 Lê Quang Đạo - Phú Đô - Nam Từ Liêm - Hà Nội",
                ngaytaodon: new Date()
            };
            console.log("Dữ liệu gửi lên Trahang:", data);
            // Gửi dữ liệu
            $http.post("https://localhost:7196/api/Trahangs", data)
                .then(response => {
                    const maxId = response.data.id; // Lấy trực tiếp từ kết quả POST

                    const promises = $scope.selectedProducts.map(element => {
                        const datathct = {
                            idth: maxId,
                            soluong: element.soluong,
                            tinhtrang: 0,
                            idhdct: element.id
                        };
                        console.log("Dữ liệu gửi lên Trahangchitiet:", datathct);
                        return $http.post("https://localhost:7196/api/Trahangchitiets", datathct);
                    });

                    return Promise.all(promises).then(() => maxId);
                })
                .then(maxId => {
                    if ($scope.images && $scope.images.length > 0) {
                        return uploadImages(maxId).then(() => maxId);
                    }
                    return maxId;
                })
                .then(() => {
                    return $http.put(`https://localhost:7196/api/Trahangs/UpdateTrangThaiHd/${$scope.idhd}`);
                })
                .then(() => {
                    Swal.fire("Đã gửi!", "Yêu cầu trả hàng của bạn đã được gửi thành công.", "success")
                        .then(() => {
                            $timeout(() => {
                                $location.path("/quanlyhoadon");
                            });
                        });
                })
                .catch(error => {
                    console.error("Lỗi trong quá trình xử lý:", error);
                    Swal.fire("Thất bại!", "Đã xảy ra lỗi khi gửi yêu cầu. Vui lòng thử lại sau.", "error");
                });

        });
    };
    

    let addressTrangThai0 = null;
    let currentAddressId = null

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

    const loadAddressesByIdKH = async () => {
        const idKH = GetByidKH(); // Lấy ID khách hàng
        const addressSelect = document.getElementById("addressSelect");

        if (!idKH) {
            addressSelect.innerHTML = '<option disabled selected value="">Không tìm thấy mã khách hàng</option>';
            addressSelect.disabled = true;
            return;
        }

        try {
            const response = await fetch(`${apiAddressList}/khachhang/${idKH}`);
            if (!response.ok) throw new Error("Không thể lấy dữ liệu từ server.");

            const data = await response.json();

            if (!data || data.length === 0) {
                addressSelect.innerHTML = '<option disabled selected value="">Tài khoản này chưa có địa chỉ, vui lòng thêm địa chỉ</option>';
                addressSelect.disabled = true;
                return;
            }

            addressSelect.innerHTML = '<option disabled selected value="" required>Chọn địa chỉ...</option>';

            // Lặp qua danh sách địa chỉ
            for (const address of data) {
                // Lấy tên khu vực
                const tenThanhPho = await getProvinceName(address.thanhpho);
                const tenQuanHuyen = await getDistrictName(address.thanhpho, address.quanhuyen);
                const tenPhuongXa = await getWardName(address.quanhuyen, address.phuongxa);

                if (address.trangthai === "1") {
                    addressSelect.innerHTML += `
                        <option value="${address.id}" 
                                data-ten="${address.tennguoinhan || 'Chưa cập nhật'}" 
                                data-sdt="${address.sdtnguoinhan || 'Chưa cập nhật'}" 
                                data-diachi="${address.diachicuthe || ''} - ${tenPhuongXa} - ${tenQuanHuyen} - ${tenThanhPho}">
                            ${address.tennguoinhan} - ${address.sdtnguoinhan}, ${address.diachicuthe} - ${tenPhuongXa} - ${tenQuanHuyen} - ${tenThanhPho}
                        </option>`;
                }

                if (address.trangthai === "0" && addressTrangThai0 === null) {
                    currentAddressId = address;
                    addressTrangThai0 = { ...address, tenThanhPho, tenQuanHuyen, tenPhuongXa };
                }
            }

            addressSelect.disabled = false;

            // Nếu có địa chỉ trạng thái = 0, hiển thị thông tin bên ngoài
            if (addressTrangThai0) {
                getProvinceName(addressTrangThai0.thanhpho);
                getDistrictName(addressTrangThai0.thanhpho, addressTrangThai0.quanhuyen);
                getWardName(addressTrangThai0.quanhuyen, addressTrangThai0.phuongxa);

                document.getElementById("hovaten").innerText = addressTrangThai0.tennguoinhan || "Chưa cập nhật";
                document.getElementById("sdt").innerText = addressTrangThai0.sdtnguoinhan || "Chưa cập nhật";
                document.getElementById("diachi").innerText =
                    `${addressTrangThai0.diachicuthe || ''} - ${addressTrangThai0.tenPhuongXa} - ${addressTrangThai0.tenQuanHuyen} - ${addressTrangThai0.tenThanhPho}`.trim() || "Chưa cập nhật";

                // ✅ Lưu biến addressTrangThai0 ra ngoài để xử lý tiếp
                window.addressTrangThai0 = addressTrangThai0;
            }

        } catch (error) {
            console.error("Lỗi khi tải địa chỉ:", error);
            addressSelect.innerHTML = '<option disabled selected value="">Lỗi khi tải dữ liệu</option>';
            addressSelect.disabled = true;
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
                body: JSON.stringify({}) // Thêm body nếu cần
            });

            const data = await response.json();
            if (data.code === 200) {
                const province = data.data.find(p => p.ProvinceID == id);
                return province ? province.NameExtension[1] : "Không xác định";
            }
        } catch (error) {
            console.error("Lỗi lấy tỉnh/thành phố:", error);
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
                body: JSON.stringify({ province_id: Number(province_id) }) // Sửa lại biến truyền đúng
            });

            const data = await response.json();
            if (data.code === 200) {
                const district = data.data.find(d => d.DistrictID == district_id);
                return district ? district.DistrictName : "Không xác định";
            }
        } catch (error) {
            console.error("Lỗi lấy quận/huyện:", error);
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
                body: JSON.stringify({ district_id: Number(district_id) }) // Sửa lại biến truyền đúng
            });

            const data = await response.json();
            if (data.code === 200) {
                const ward = data.data.find(w => w.WardCode == ward_id);
                return ward ? ward.WardName : "Không xác định";
            }
        } catch (error) {
            console.error("Lỗi lấy phường/xã:", error);
        }
        return "Không xác định";
    }


    document.getElementById("btnSaveAddress").addEventListener("click", async function () {
        var addressSelect = document.getElementById("addressSelect");
        var selectedAddressId = addressSelect.value; // Lấy id của địa chỉ đã chọn

        // Kiểm tra xem người dùng có chọn địa chỉ không
        if (!selectedAddressId) {
            Swal.fire("Lỗi", "Vui lòng chọn một địa chỉ", "error");
            return;
        }

        // Lấy thông tin địa chỉ chi tiết từ API hoặc mảng địa chỉ
        try {
            const response = await axios.get(`${apiAddressList}/${selectedAddressId}`);

            if (response && response.data) {

                currentAddressId = response.data;

                const tenThanhPho = await getProvinceName(response.data.thanhpho);
                const tenQuanHuyen = await getDistrictName(response.data.thanhpho, response.data.quanhuyen);
                const tenPhuongXa = await getWardName(response.data.quanhuyen, response.data.phuongxa);

                // Tạo địa chỉ mới từ thông tin chi tiết của địa chỉ
                var newAddress = response.data.diachicuthe + ", " +
                    tenPhuongXa + ", " +
                    tenQuanHuyen + ", " +
                    tenThanhPho;

                // Cập nhật thông tin địa chỉ vào phần tử có id "diachi"
                document.getElementById("diachi").textContent = newAddress;
                document.getElementById("sdt").textContent = response.data.sdtnguoinhan;
                document.getElementById("hovaten").textContent = response.data.tennguoinhan;

                // Xóa phần tử "Mặc định" nếu có
                var defaultBadge = document.querySelector(".badge.bg-primary-subtle.text-success");
                if (defaultBadge) {
                    defaultBadge.remove(); // Xóa "Mặc định"
                }

                // Hiển thị nút "Khôi phục" nếu địa chỉ đã thay đổi
                document.getElementById("btnRestoreAddress").style.display = 'inline-block';

                // Ẩn modal sau khi lưu địa chỉ
                var modal = bootstrap.Modal.getInstance(document.getElementById("exampleModal"));
                modal.hide();

                // Hiển thị kết quả sau khi lưu thành công
                printResult();

                Swal.fire("Thành Công", "Thay đổi địa chỉ thành công.", "success");
            } else {
                Swal.fire("Lỗi", "Không tìm thấy thông tin địa chỉ.", "error");
            }
        } catch (error) {
            Swal.fire("Lỗi", "Không thể tải thông tin địa chỉ", "error");
            console.error(error);
        }
    });

    /// Lắng nghe sự kiện "Khôi phục" địa chỉ mặc định
    document.getElementById("btnRestoreAddress").addEventListener("click", function () {
        // Gọi API để lấy lại địa chỉ mặc định
        fetchkhachangById()
            .then(data => {
                // Kiểm tra xem dữ liệu trả về có chứa địa chỉ không
                if (data && addressTrangThai0) {
                    currentAddressId = addressTrangThai0;
                    var defaultAddress = `${addressTrangThai0.diachicuthe}, ${addressTrangThai0.tenPhuongXa}, ${addressTrangThai0.tenQuanHuyen}, ${addressTrangThai0.tenThanhPho}`; // Cập nhật theo cấu trúc dữ liệu thực tế

                    // Cập nhật lại địa chỉ mặc định vào phần tử "diachi"
                    var diachiElement = document.getElementById("diachi");
                    if (diachiElement) {
                        diachiElement.textContent = defaultAddress;
                        document.getElementById("sdt").textContent = data.sdt;
                        document.getElementById("hovaten").textContent = data.ten;

                        // Kiểm tra và hiển thị lại phần tử "Mặc định"
                        var badge = document.querySelector(".badge.bg-primary-subtle.text-success");
                        if (badge) {
                            badge.remove(); // Xóa phần tử "Mặc định" cũ nếu có
                        }

                        // Tạo phần tử "Mặc định" mới
                        var newBadge = document.createElement("span");
                        newBadge.classList.add("badge", "bg-primary-subtle", "text-success", "border", "border-success", "me-2");
                        newBadge.style.marginLeft = "10px";
                        newBadge.textContent = "Mặc định";
                        diachiElement.appendChild(newBadge);
                    }

                    // Ẩn nút khôi phục sau khi đã khôi phục địa chỉ mặc định
                    document.getElementById("btnRestoreAddress").style.display = 'none';
                    // Ẩn modal sau khi lưu địa chỉ
                    var modal = bootstrap.Modal.getInstance(document.getElementById("exampleModal"));
                    modal.hide();

                    // Hiển thị thông báo lỗi giả
                    Swal.fire("Thành Công", "Khôi phục địa chỉ mặc định thành công.", "success");
                } else {
                    // Nếu không có địa chỉ trong dữ liệu trả về, thông báo lỗi
                    Swal.fire("Lỗi", "Không tìm thấy địa chỉ mặc định.", "error");
                }
            })
            .catch(error => {
                // Thông báo lỗi khi gọi API
                Swal.fire("Lỗi", "Lỗi khi khôi phục địa chỉ: " + error.message, "error");
            });
    });

    var printResult = () => {
        let province = document.querySelector("#province") ? document.querySelector("#province").value : '';
        let district = document.querySelector("#district") ? document.querySelector("#district").value : '';
        let ward = document.querySelector("#ward") ? document.querySelector("#ward").value : '';

        // Nếu tất cả các dropdown đều có giá trị đã chọn, hiển thị kết quả
        if (province && district && ward) {
            let result = `${document.querySelector("#province").selectedOptions[0].text} | ` +
                `${document.querySelector("#district").selectedOptions[0].text} | ` +
                `${document.querySelector("#ward").selectedOptions[0].text}`;
            if (document.querySelector("#result")) {
                document.querySelector("#result").textContent = result; // Hiển thị kết quả
            }
        }
    };

    async function fetchkhachangById() {
        // Lấy ID khách hàng
        const idkh = GetByidKH();
        if (!idkh) {
            console.warn("Không thể lấy ID khách hàng.");
            return;
        }

        try {
            // Gửi yêu cầu đến API với idkh
            const response = await fetch(`${apiKHUrl}/${idkh}`);

            // Kiểm tra nếu response không ok, vứt lỗi
            if (!response.ok) {
                throw new Error(`Lỗi API: ${response.status}`);
            }

            // Chuyển đổi dữ liệu JSON
            const khachHangData = await response.json();

            // Kiểm tra xem dữ liệu có hợp lệ hay không
            if (!khachHangData) {
                throw new Error("Dữ liệu khách hàng không hợp lệ.");
            }
            // Trả về dữ liệu khách hàng
            return khachHangData;

        } catch (error) {
            // Hiển thị thông báo lỗi khi có vấn đề xảy ra
            console.error("Lỗi khi lấy thông tin khách hàng:", error);
            alert("Có lỗi xảy ra khi tải thông tin khách hàng. Vui lòng thử lại.");
        }
    }

    loadAddressesByIdKH();
});