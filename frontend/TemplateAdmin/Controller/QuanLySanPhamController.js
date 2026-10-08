app.controller('QuanLySanPhamController', function ($scope, $http, $location, $timeout, $routeParams, $interval) {
    $scope.categories = [];
    $scope.colors = [];
    $scope.sizes = [];
    $scope.chatlieus = [];
    $scope.product = {
        tenSanpham: "",
        mota: "",
        trangthai: 0,
        soluong: 0,
        giaBan: null,
        ngayThemMoi: new Date().toISOString(),
        chieudai: null,
        chieurong: null,
        trongluong: null,
        chieucao: null,
        idth: 0
    };
    $scope.products = [];
    $scope.product.variants = [];
    $scope.variantIndexCounter = 1;
    function getVietnameseDay(dayIndex) {
        const days = ["Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"];
        return days[dayIndex];
    }
    // Hàm cập nhật thời gian
    function updateTime() {
        var now = new Date();
        var dayOfWeek = getVietnameseDay(now.getDay());
        var day = String(now.getDate()).padStart(2, '0');
        var month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
        var year = now.getFullYear();
        var hours = String(now.getHours()).padStart(2, '0');
        var minutes = String(now.getMinutes()).padStart(2, '0');
        var seconds = String(now.getSeconds()).padStart(2, '0');

        $scope.time = `${dayOfWeek}, ${day}/${month}/${year} - ${hours}:${minutes}:${seconds}`;
    }

    // Cập nhật mỗi giây
    $interval(updateTime, 1000);

    // Chạy ngay khi trang tải
    updateTime();
    $scope.loadProducts = function () {
        $http.get("https://localhost:7196/api/Sanphams")
            .then(function (response) {
                $scope.products = response.data;
                $scope.productsALL = [];
                $scope.productsStatus0 = [];
                $scope.productsStatus1 = [];
                $scope.productsStatus2 = [];

                $scope.products.forEach(function (product) {
                    product.trangThaiSwitch = product.trangthai === 0 || product.trangthai === 1;

                    // Gọi API lấy biến thể từng sản phẩm
                    $http.get("https://localhost:7196/api/Sanphamchitiets/sanpham/" + product.id)
                        .then(function (res) {
                            // 🔍 Lọc biến thể trạng thái khác 3
                            const filteredVariants = res.data.filter(v => v.trangthai !== 3);
                            product.variants = filteredVariants;

                            // Nếu không có biến thể hợp lệ, bỏ qua (nếu bạn muốn loại sản phẩm không có biến thể)
                            if (product.variants.length === 0) return;

                            // Lấy ảnh đầu tiên hoặc ảnh mặc định
                            product.image = product.variants.length > 0
                                ? product.variants[0].urlHinhanh
                                : 'default-image.jpg';

                            // Thêm vào danh sách theo trạng thái
                            if (product.trangthai === 0 || product.trangthai === 1) {
                                $scope.productsStatus0.push(product);
                            }
                            if (product.trangthai === 1 || product.soluong === 0) {
                                $scope.productsStatus1.push(product);
                            }
                            if (product.trangthai === 2) {
                                $scope.productsStatus2.push(product);
                            }
                            if (product.trangthai !== 3) {
                                $scope.productsALL.push(product);
                            } // Nếu bạn muốn danh sách tổng
                        })
                        .catch(function () {
                            product.image = 'default-image.jpg';
                        });
                });

                console.log("sp 0", $scope.productsStatus0);
            });
    };

    $scope.TatSanPham = function (product) {
        $http.put(`https://localhost:7196/api/Sanphams/${product.id}/cancel`)
            .then(function (response) {
                console.log("✅ Sản phẩm đã được tắt:", response.data);

                $scope.loadProducts(); // Tải lại danh sách sản phẩm
                alert("Sản phẩm đã được tắt thành công!");
            })
            .catch(function (error) {
                console.error("❌ Lỗi khi tắt sản phẩm:", error);
                alert("Có lỗi xảy ra khi tắt sản phẩm.");
            });
    }
    $scope.KichHoatSanPham = function (product) {
        $http.put(`https://localhost:7196/api/Sanphams/${product.id}/update-status-load`)
            .then(function (response) {
                console.log("✅ Sản phẩm đã được kích hoạt:", response.data);
                alert("Sản phẩm đã được kích hoạt thành công!");
                $scope.loadProducts(); // Tải lại danh sách sản phẩm
            })
            .catch(function (error) {
                console.error("❌ Lỗi khi kích hoạt sản phẩm:", error);
                alert("Có lỗi xảy ra khi kích hoạt sản phẩm.");
            });
    }
    $scope.toggleProductStatus = function (product) {
        if (product.trangThaiSwitch) {
            // Nếu bật switch => kích hoạt sản phẩm
            $scope.KichHoatSanPham(product);
        } else {
            // Nếu tắt switch => tắt sản phẩm
            $scope.TatSanPham(product);
        }
    };
    $scope.loadCategories = function () {
        $http.get("https://localhost:7196/api/Thuonghieu")
            .then(function (response) {
                //Thương hiệu hoạt động
                $scope.categories0 = []
                $scope.categories = response.data;
                $scope.categories.forEach(function (category) {
                    if (category.tinhtrang === 0) {
                        $scope.categories0.push(category);
                    }
                    console.log("thanh", $scope.categories0);


                });
                console.log($scope.categories);

            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách thương hiệu:", error);
            }
            );
    }
    $scope.loadColors = function () {
        $http.get("https://localhost:7196/api/Color")
            .then(function (response) {
                //Color hoạt động
                $scope.colors0 = []
                $scope.colors = response.data;
                $scope.colors.forEach(function (color) {
                    if (color.trangthai === 0) {
                        $scope.colors0.push(color);
                    }
                });
                console.log("Thành", $scope.colors0);

                console.log($scope.colors);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách màu sắc:", error);
            });
    };
    $scope.loadSizes = function () {
        $http.get("https://localhost:7196/api/Size")
            .then(function (response) {
                //Size hoạt động
                $scope.sizes0 = []
                $scope.sizes = response.data;
                $scope.sizes.forEach(function (size) {
                    if (size.trangthai === 0) {
                        $scope.sizes0.push(size);
                    }
                });
                console.log($scope.sizes);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách kích thước:", error);
            });
    }
    $scope.loadChatlieus = function () {
        $http.get("https://localhost:7196/api/Chatlieu")
            .then(function (response) {
                //Chất liệu hoạt động
                $scope.chatlieus0 = []
                $scope.chatlieus = response.data;
                $scope.chatlieus.forEach(function (chatlieu) {
                    if (chatlieu.trangthai === 0) {
                        $scope.chatlieus0.push(chatlieu);
                    }
                });
                console.log($scope.chatlieus);
            })
            .catch(function (error) {
                console.error("Lỗi khi tải danh sách chất liệu:", error);
            });
    }
    $scope.loadColors();
    $scope.loadSizes();
    $scope.loadChatlieus();
    $scope.loadCategories();
    $scope.loadProducts();
    // Tạo biến thể mới khi mở modal
    $scope.newVariant = {};
    $scope.getColorName = function (id) {
        let color = $scope.colors.find(c => c.id === id);
        return color ? color.tenmau : 'Không xác định';
    };
    $scope.getSizeName = function (id) {
        let size = $scope.sizes.find(s => s.id === id);
        return size ? size.sosize : 'Không xác định';
    };
    $scope.getCategoriesName = function (id) {
        let categories = $scope.categories.find(s => s.id === id);
        return categories ? categories.tenthuonghieu : 'Không xác định';
    };
    $scope.getMaterialName = function (id) {
        let material = $scope.chatlieus.find(cl => cl.id === id);
        return material ? material.tenchatlieu : 'Không xác định';
    };
    // Xử lý ảnh
    $scope.handleFileSelect = function (event, variantType) {
        var file = event.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.$applyAsync(() => {
                    $scope[variantType].UrlHinhanh = e.target.result; // Hiển thị ảnh base64
                    $scope[variantType].file = file; // File gốc để gửi lên API
                    console.log("Đã chọn file:", file); // Debug log
                });
            };
            reader.readAsDataURL(file);
        }
    };
    $scope.isPriceFieldVisible = false;

    $scope.showEditVariantModal = function (variant, index) {
        $scope.editingIndex = index + 1;

        let variantToEdit = {
            Id: variant.Id,
            IdMau: variant.IdMau,
            IdSize: variant.IdSize,
            IdChatLieu: variant.IdChatLieu,
            Soluong: variant.Soluong,
            Giathoidiemhientai: variant.Giathoidiemhientai,
            Trangthai: variant.Trangthai,
            UrlHinhanh: variant.UrlHinhanh,
            file: variant.file || null // ✅ giữ lại file nếu có
        };
        $scope.newVariant = variantToEdit
        $scope.variantEditingIndex = index; // ✅ Ghi nhớ vị trí cần cập nhật
        if ($scope.newVariant.Giathoidiemhientai !== $scope.product.giaBan) {
            $scope.isPriceFieldVisible = true;
        } else {
            $scope.isPriceFieldVisible = false;
        }
        $scope.isPriceFieldVisible = true;
        $scope.isEditing = true;

        $timeout(function () {
            if ($scope.variantForm) {
                $scope.variantForm.$setPristine();
                $scope.variantForm.$setUntouched();
                $scope.variantForm.$submitted = false;
            }
        });
        console.log("Biến thể cần chỉnh sửa:", $scope.newVariant);


        var modalEl = document.getElementById('addVariantModal');
        modalEl.removeAttribute('aria-hidden');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();

    };
    $scope.$watch('product.giaBan', function (newGia, oldGia) {
        if (newGia !== oldGia) {
            $scope.product.variants.forEach(function (variant) {
                if (variant.Giathoidiemhientai === oldGia) {
                    variant.Giathoidiemhientai = newGia;
                }
            });

            // Nếu đang thêm hoặc sửa biến thể thì cũng cập nhật luôn:
            if ($scope.newVariant && $scope.newVariant.Giathoidiemhientai === oldGia) {
                $scope.newVariant.Giathoidiemhientai = newGia;
            }
        }
    });
    // Mở modal THÊM mới biến thể
    $scope.showAddVariantModal = function () {
        $scope.newVariant = { Trangthai: 0 };
        $scope.variantEditingIndex = -1; // ✅ Đặt lại chế độ về "thêm mới"
        $scope.isPriceFieldVisible = false;
        $scope.isEditing = false;

        $timeout(function () {
            if ($scope.variantForm) {
                $scope.variantForm.$setPristine();
                $scope.variantForm.$setUntouched();
                $scope.variantForm.$submitted = false;
            }
        });

        var modal = new bootstrap.Modal(document.getElementById('addVariantModal'));
        modal.show();
    };
    // Reset form khi đóng modal (gắn vào nút đóng)
    $scope.resetVariantForm = function () {
        $scope.newVariant = {};
        $scope.isEditing = false;
        $scope.isPriceFieldVisible = false;

        if ($scope.variantForm) {
            $scope.variantForm.$setPristine();
            $scope.variantForm.$setUntouched();
        }
    };
    $scope.removeImage = function (variantType) {
        $scope[variantType].UrlHinhanh = null;
        $scope[variantType].file = null;

        // Reset luôn thẻ input file nếu có
        const input = document.getElementById('imageUpload');
        if (input) input.value = null;
    };

    $scope.saveVariant = function () {
        if ($scope.variantForm.$invalid) {
            angular.forEach($scope.variantForm.$error, function (field) {
                angular.forEach(field, function (errorField) {
                    errorField.$setTouched();
                });
            });
            $scope.variantForm.$setSubmitted();
            return;
        }
        if (!($scope.newVariant.file || $scope.newVariant.UrlHinhanh)) {
            alert("❌ Vui lòng chọn ảnh cho biến thể!");
            return;
        }

        if ($scope.variantEditingIndex === -1) {
            // ✅ Kiểm tra trùng khi thêm mới
            var existingVariant = $scope.product.variants.find(v =>
                v.IdMau === $scope.newVariant.IdMau && v.IdSize === $scope.newVariant.IdSize && v.IdChatLieu === $scope.newVariant.IdChatLieu && v.Trangthai !== 3
            );
            if (existingVariant) {
                alert("❌ Biến thể đã tồn tại!");
                //reset form
                $scope.newVariant = { Trangthai: 0 };
                $scope.isPriceFieldVisible = false;
                $scope.variantEditingIndex = -1;
                if ($scope.variantForm) {
                    $scope.variantForm.$setPristine();
                    $scope.variantForm.$setUntouched();
                    $scope.variantForm.$submitted = false;
                }
                return;
            }

            if (!$scope.newVariant.Giathoidiemhientai) {
                $scope.newVariant.Giathoidiemhientai = $scope.product.giaBan;
            }

            let variantToAdd = {

                IdMau: $scope.newVariant.IdMau,
                IdSize: $scope.newVariant.IdSize,
                IdChatLieu: $scope.newVariant.IdChatLieu,
                Soluong: $scope.newVariant.Soluong,
                Giathoidiemhientai: $scope.newVariant.Giathoidiemhientai,
                Trangthai: $scope.newVariant.Trangthai,
                UrlHinhanh: $scope.newVariant.UrlHinhanh,
                file: $scope.newVariant.file
            };

            $scope.product.variants.push(variantToAdd);
            alert("✅ Đã thêm biến thể mới!");
            $scope.variantIndexCounter++;
            console.log("Biến thể mới:", $scope.newVariant);
            console.log("Danh sách biến thể hiện tại:", $scope.product.variants);


        } else {
            let oldVariant = $scope.product.variants[$scope.variantEditingIndex];

            // Cập nhật thông tin nhưng giữ lại Id và Idsp từ biến thể cũ
            let updatedVariant = angular.copy(oldVariant);
            updatedVariant.IdMau = $scope.newVariant.IdMau;
            updatedVariant.IdSize = $scope.newVariant.IdSize;
            updatedVariant.IdChatLieu = $scope.newVariant.IdChatLieu;
            updatedVariant.Soluong = $scope.newVariant.Soluong;
            updatedVariant.Giathoidiemhientai = $scope.newVariant.Giathoidiemhientai || $scope.product.giaBan;
            updatedVariant.Trangthai = $scope.newVariant.Trangthai;
            updatedVariant.UrlHinhanh = $scope.newVariant.UrlHinhanh;
            updatedVariant.file = $scope.newVariant.file;

            $scope.product.variants[$scope.variantEditingIndex] = updatedVariant;

            alert("✅ Đã cập nhật biến thể!");
            console.log("Biến thể đã cập nhật:", updatedVariant);
        }

        // Reset lại modal
        $scope.newVariant = { Trangthai: 0 };
        $scope.isPriceFieldVisible = false;
        $scope.variantEditingIndex = -1;

        if ($scope.variantForm) {
            $scope.variantForm.$setPristine();
            $scope.variantForm.$setUntouched();
            $scope.variantForm.$submitted = false;
        }

        var modalElement = document.getElementById('addVariantModal');
        if (modalElement) {
            var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
            modal.hide();
        }
    };
    // Xóa biến thể
    $scope.removeVariant = function (index) {
        const confirmDelete = window.confirm("Bạn có chắc chắn muốn xóa biến thể này?");
    if (confirmDelete) {
        $scope.product.variants.splice(index, 1);
        $scope.product.variants[0].Giathoidiemhientai = $scope.product.giaBan;
        console.log("Danh sách sau xóa" + $scope.product.variants);
    }

    };
    // Hàm để chuyển đổi trạng thái hiển thị trường nhập liệu
    $scope.togglePriceField = function () {
        $scope.isPriceFieldVisible = true;
        $scope.newVariant.Giathoidiemhientai = null; // Đặt lại giá trị
    };
    // Hàm để ẩn trường nhập liệu khi nhấn "Xóa"
    $scope.hidePriceField = function () {
        $scope.isPriceFieldVisible = false;
        $scope.newVariant.Giathoidiemhientai = null; // Đặt lại giá trị
    };
    // Xóa sản phẩm
    $scope.deleteProduct = function (product) {
        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Sản phẩm sẽ bị xóa và không thể khôi phục!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                $http.delete("https://localhost:7196/api/Sanphams/" + product.id)
                    .then(function (response) {
                        Swal.fire({
                            title: "Thành công",
                            text: response.data.message || "Xóa sản phẩm thành công!",
                            icon: "success"
                        }).then(() => {
                            $scope.loadProducts(); // Tải lại danh sách sản phẩm
                        });
                    })
                    .catch(function (error) {
                        console.error("❌ Lỗi khi xóa sản phẩm:", error);
                        Swal.fire({
                            title: "Lỗi",
                            text: "Có lỗi xảy ra khi xóa sản phẩm.",
                            icon: "error"
                        });
                    });
            }
        });
    };


    $scope.addThuongHieu = function () {
        if (!$scope.newThuongHieu || !$scope.newThuongHieu.trim()) {
            alert("Tên thương hiệu không được để trống.");
            return;

        } else if ($scope.newThuongHieu.length < 2) {
            alert("Tên thương hiệu phải có ít nhất 2 ký tự.");
            return;
        }
        else if ($scope.newThuongHieu.length > 50) {
            alert("Tên thương hiệu không được quá 50 ký tự.");
            return;
        }
        const isDuplicate = $scope.categories0.some(th =>
            th.tenthuonghieu.toLowerCase().trim() === $scope.newThuongHieu.toLowerCase().trim());
        if (isDuplicate) {
            alert("Tên thương hiệu đã tồn tại.");
            return;
        }
        const newTH = {
            tenthuonghieu: $scope.newThuongHieu.trim(),
            trangthai: 0
        };
        $http.post("https://localhost:7196/api/Thuonghieu", newTH)
            .then(function (response) {
                console.log("✅ Thương hiệu đã được thêm:", response.data);
                alert("Thương hiệu đã được thêm thành công!");
                $scope.loadCategories();
            })
            .catch(function (error) {
                console.error("❌ Lỗi khi thêm thương hiệu:", error);
                alert("Có lỗi xảy ra khi thêm thương hiệu.");
            });
        // Gọi API thêm hoặc xử lý tiếp...
        $scope.categories0.push(newTH);
        $scope.newThuongHieu = '';
    };
    $scope.addChatLieu = function () {
        if (!$scope.newChatLieu || !$scope.newChatLieu.trim()) {
            alert("Tên chất liệu không được để trống.");
            return;
        } else if ($scope.newChatLieu.length < 2) {
            alert("Tên chất liệu phải có ít nhất 2 ký tự.");
            return;
        }
        else if ($scope.newChatLieu.length > 50) {
            alert("Tên chất liệu không được quá 50 ký tự.");
            return;
        }


        const isDuplicate = $scope.chatlieus0.some(cl =>
            cl.tenchatlieu.toLowerCase().trim() === $scope.newChatLieu.toLowerCase().trim());

        if (isDuplicate) {
            alert("Tên chất liệu đã tồn tại.");
            return;
        }

        const newCL = {
            tenchatlieu: $scope.newChatLieu.trim(),
            trangthai: 0
        };
        $http.post("https://localhost:7196/api/Chatlieu", newCL)
            .then(function (response) {
                console.log("✅ Chất liệu đã được thêm:", response.data);
                alert("Chất liệu đã được thêm thành công!");
                $scope.loadChatlieus();
            })
            .catch(function (error) {
                console.error("❌ Lỗi khi thêm chất liệu:", error);
                alert("Có lỗi xảy ra khi thêm chất liệu.");
            });

        $scope.chatlieus0.push(newCL);
        $scope.newChatLieu = '';
    };
    $scope.addColor = function () {
        if (!$scope.newColor || !$scope.newColor.tenmau || !$scope.newColor.tenmau.trim()) {
            alert("Tên màu không được để trống.");
            return;
        } else if ($scope.newColor.tenmau.length < 2) {
            alert("Tên màu phải có ít nhất 2 ký tự.");
            return;
        }
        else if ($scope.newColor.tenmau.length > 50) {
            alert("Tên màu không được quá 50 ký tự.");
            return;
        }
        else if (!$scope.newColor.mamau) {
            alert("Mã màu không được để trống.");
            return;
        }

        const isDuplicate = $scope.colors0.some(color =>
            color.tenmau.toLowerCase().trim() === $scope.newColor.tenmau.toLowerCase().trim());

        if (isDuplicate) {
            alert("Tên màu đã tồn tại.");
            return;
        }

        const newColor = {
            tenmau: $scope.newColor.tenmau.trim(),
            mamau: $scope.newColor.mamau || '#000000',
            trangthai: 0
        };
        $http.post("https://localhost:7196/api/Color", newColor)
            .then(function (response) {
                console.log("✅ Màu đã được thêm:", response.data);
                alert("Màu đã được thêm thành công!");
                $scope.loadColors();
            })
            .catch(function (error) {
                console.error("❌ Lỗi khi thêm màu:", error);
                alert("Có lỗi xảy ra khi thêm màu.");
            });

        $scope.colors0.push(newColor);
        $scope.newColor = {};
    };
    $scope.addSize = function () {
        if ($scope.newSize == null || $scope.newSize === '') {
            alert("Vui lòng nhập số size.");
            return;
        }
        const sizeValue = parseInt($scope.newSize);
        if (isNaN(sizeValue)) {
            alert("Số size phải là số nguyên.");
            return;
        } if (sizeValue <= 30 || sizeValue >= 50) {
            alert("Số size phải nằm trong khoảng từ 30 đến 50.");
            return;
        }
        const isDuplicate = $scope.sizes0.some(sz => sz.sosize === sizeValue);
        if (isDuplicate) {
            alert("Số size đã tồn tại.");
            return;
        }

        const newSizeObj = {
            sosize: sizeValue,
            trangthai: 0
        };

        // Nếu bạn có gọi API:
        $http.post("https://localhost:7196/api/Size", newSizeObj)
            .then(function (response) {
                $scope.sizes0.push(response.data);
                $scope.newSize = '';
                alert("Thêm size thành công!");
                $scope.loadSizes();
            })
            .catch(function (error) {
                console.error("Lỗi khi thêm size:", error);
                alert("Đã xảy ra lỗi khi thêm size.");
            });


        $scope.sizes0.push(newSizeObj);
        $scope.newSize = '';
    };
    $scope.resetSize = function () {
        $scope.newSize = null;
    };
    $scope.resetColor = function () {
        $scope.newColor = null;
    };
    $scope.resetChatLieu = function () {
        $scope.newChatLieu = null;
    };
    $scope.resetThuongHieu = function () {
        $scope.newThuongHieu = null;
    };
    $scope.blockInvalidKeys = function (event) {
        const invalidKeys = [190, 188, 110]; // . , trên các layout khác nhau

        if (invalidKeys.includes(event.keyCode)) {
            event.preventDefault();
        }
    };
    $scope.getProductById = function (id) {
        $http.get("https://localhost:7196/api/Sanphams/" + id)
            .then(function (response) {
                $scope.product = response.data;
                console.log("📦 Thông tin sản phẩm:", $scope.product);
                // Gọi tiếp API lấy biến thể
                return $http.get("https://localhost:7196/api/Sanphamchitiets/sanpham/" + id);
            })
            .then(function (response) {
                const variantsRaw = response.data;
                const filteredVariants = variantsRaw.filter(variant => variant.trangthai !== 3);
                $scope.product.variants = filteredVariants.map(variant => ({
                    Id: variant.id,
                    Idsp: variant.idsp,
                    IdMau: variant.idMau,
                    IdSize: variant.idSize,
                    IdChatLieu: variant.idChatLieu,
                    Soluong: variant.soluong,
                    Giathoidiemhientai: variant.giathoidiemhientai,
                    Trangthai: variant.trangthai,
                    UrlHinhanh: variant.urlHinhanh,
                    file: variant.file || null
                }));
                $scope.variantIndexCounter = $scope.product.variants.length + 1;
                console.log("🎨 Biến thể đã xử lý:", $scope.product.variants);
                console.log("Số biến thể hiện tại:", $scope.variantIndexCounter);

            })
            .catch(function (error) {
                console.error("❌ Lỗi khi tải sản phẩm hoặc biến thể:", error);
            });
    };
    $scope.saveProduct = function () {
        if ($scope.frm.$valid) {
            Swal.fire({
                title: "Xác nhận",
                text: "Bạn có chắc chắn muốn thêm sản phẩm này?",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Đồng ý",
                cancelButtonText: "Hủy"
            }).then((result) => {
                if (result.isConfirmed) {
                    $http.post("https://localhost:7196/api/Sanphams", $scope.product)
                        .then(function (response) {
                            const createdProduct = response.data;
                            const productId = createdProduct.id;
                            console.log("✅ Sản phẩm đã được lưu:", createdProduct);

                            // Gửi từng biến thể dưới dạng FormData
                            let promises = [];
                            for (let variant of $scope.product.variants) {
                                variant.Idsp = productId;

                                let formData = new FormData();
                                for (let key in variant) {
                                    if (key !== 'file' && key !== 'UrlHinhanh') { // Không gửi base64
                                        formData.append(key, variant[key]);
                                    }
                                }
                                if (variant.file) {
                                    formData.append("file", variant.file); // Gửi file thật
                                }
                                let promise = $http.post("https://localhost:7196/api/Sanphamchitiets", formData, {
                                    headers: { 'Content-Type': undefined }
                                })
                                    .then(function (response) {
                                        console.log("✅ Biến thể đã được lưu:", response.data);
                                    })
                                    .catch(function (error) {
                                        console.error("❌ Lỗi khi lưu biến thể:", error);
                                        alert("Có lỗi xảy ra khi lưu biến thể.");
                                    });

                                promises.push(promise);
                            }
                            Promise.all(promises)
                                .then(function () {
                                    Swal.fire({
                                        title: "Thành công",
                                        text: result.Message || "Thêm Sản phẩm thành công!",
                                        icon: "success"
                                    });

                                    $scope.loadProducts(); // Tải lại danh sách sản phẩm
                                    $location.url('/sanpham');
                                })
                                .catch(function () {
                                    Swal.fire({
                                        title: "Lỗi",
                                        text: error.data?.Message || error.Message || "Có lỗi xảy ra",
                                        icon: "error"
                                    });
                                });

                        })
                        .catch(function (error) {
                            console.error("❌ Lỗi khi lưu sản phẩm:", error);
                            alert("Có lỗi xảy ra khi lưu sản phẩm.");
                        });
                }
            });
        }
    };
    //Cập nhật sản phẩm và biến thể
    $scope.updateProduct = function () {
        if ($scope.frm.$valid) {
            if ($scope.product.variants.every(v => v.Trangthai === 2)) {
                $scope.product.trangthai = 2;
            } else {
                $scope.product.trangthai = 0;
            }
            // 1. Cập nhật sản phẩm chính
            $http.put("https://localhost:7196/api/Sanphams/" + $scope.product.id, $scope.product)
                .then(function (response) {
                    //nếu toàn bộ trạng thái biến thể là 2 thì cập nhật trạng thái sản phẩm là 2

                    console.log("✅ Sản phẩm đã được cập nhật:", response.data);

                    // 2. Lấy danh sách biến thể cũ từ server
                    $http.get("https://localhost:7196/api/Sanphamchitiets/sanpham/" + $scope.product.id)
                        .then(function (res) {
                            const oldVariants = res.data; // biến thể cũ
                            const currentVariantIds = $scope.product.variants.map(v => v.Id).filter(id => id); // ID biến thể hiện tại

                            let promises = [];

                            // 3. Xóa biến thể không còn trong danh sách hiện tại
                            for (let old of oldVariants) {
                                if (!currentVariantIds.includes(old.id)) {
                                    let deletePromise = $http.delete("https://localhost:7196/api/Sanphamchitiets/" + old.id)
                                        .then(() => {
                                            console.log("🗑️ Đã xóa biến thể:", old.id);
                                        })
                                        .catch(error => {
                                            console.error("❌ Lỗi khi xóa biến thể:", error);
                                        });
                                    promises.push(deletePromise);
                                }
                            }

                            // 4. Cập nhật và thêm mới biến thể
                            for (let variant of $scope.product.variants) {
                                let formData = new FormData();
                                for (let key in variant) {
                                    if (key !== 'file' && key !== 'UrlHinhanh') {
                                        formData.append(key, variant[key]);
                                    }
                                }
                                if (variant.file) {
                                    formData.append("file", variant.file);
                                }

                                // Cập nhật
                                if (variant.Id) {
                                    let updatePromise = $http.put("https://localhost:7196/api/Sanphamchitiets/" + variant.Id, formData, {
                                        headers: { 'Content-Type': undefined }
                                    }).then(response => {
                                        console.log("✅ Biến thể cập nhật:", response.data);
                                    }).catch(error => {
                                        console.error("❌ Lỗi cập nhật biến thể:", error);
                                    });
                                    promises.push(updatePromise);
                                }
                                // Thêm mới
                                else {
                                    formData.append("Idsp", $scope.product.id);
                                    let addPromise = $http.post("https://localhost:7196/api/Sanphamchitiets", formData, {
                                        headers: { 'Content-Type': undefined }
                                    }).then(response => {
                                        console.log("✅ Biến thể mới:", response.data);
                                    }).catch(error => {
                                        console.error("❌ Lỗi thêm biến thể:", error);
                                    });
                                    promises.push(addPromise);
                                }
                            }

                            Promise.all(promises)
                            .then(() => {
                                Swal.fire({
                                    title: "Thành công",
                                    text: "Cập nhật sản phẩm thành công!",
                                    icon: "success"
                                }).then(() => {
                                    $scope.loadProducts();
                                    $location.url('/sanpham');
                                });
                            })
                            .catch(error => {
                                Swal.fire({
                                    title: "Lỗi",
                                    text: error?.data?.Message || error?.Message || "Có lỗi xảy ra khi xử lý biến thể.",
                                    icon: "error"
                                });
                            });

                        })
                        .catch(function (error) {
                            console.error("❌ Lỗi khi tải biến thể cũ:", error);
                        });

                })
                .catch(function (error) {
                    console.error("❌ Lỗi khi cập nhật sản phẩm:", error);
                    Swal.fire({
                        title: "Lỗi",
                        text: error?.data?.Message || error?.Message || "Có lỗi xảy ra khi cập nhật sản phẩm.",
                        icon: "error"
                    });
                });
        }
    };

    if ($routeParams.id) {
        let id = $routeParams.id;
        $scope.getProductById(id);
    }

    $scope.currentPage = 1;
    $scope.entriesPerPage = '10';
    $scope.searchText = '';

    // Khi thay đổi số lượng hiển thị
    $scope.updatePagination = function () {
        $scope.currentPage = 1;
    };

    // Lấy danh sách số trang
    $scope.getPageNumbers = function () {
        if (!$scope.filteredProducts) return [];
        let totalPages = Math.ceil($scope.filteredProducts.length / $scope.entriesPerPage);
        return Array.from({ length: totalPages }, (_, i) => i + 1);
    };
    // Chuyển trang
    $scope.changePage = function (page) {
        if (page < 1 || page > $scope.getPageNumbers().length) return;
        $scope.currentPage = page;
    };
    $scope.getToIndex = function () {
        let page = parseInt($scope.currentPage) || 1;
        let perPage = parseInt($scope.entriesPerPage) || 10;
        let total = Array.isArray($scope.filteredProducts) ? $scope.filteredProducts.length : 0;
        return Math.min(page * perPage, total);
    };
    $scope.goToProductDetail = function (productId) {
        window.location.href = '#!/chiTietSanPham/' + productId;
    };

});
app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            let model = $parse(attrs.fileModel);
            element.bind('change', function () {
                scope.$apply(function () {
                    model.assign(scope, element[0].files[0]);
                });
            });
        }
    };
}]);
app.filter('searchByName', function () {
    return function (items, searchText) {
        if (!searchText) return items;
        searchText = searchText.toLowerCase();
        return items.filter(function (item) {
            return item.tenSanpham && item.tenSanpham.toLowerCase().includes(searchText);
        });
    };
});
app.filter('startFrom', function () {
    return function (input, start) {
        if (!input || !input.length) return [];
        return input.slice(+start);
    };
});




