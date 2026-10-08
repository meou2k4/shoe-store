app.controller('QuanLyThuocTinhController', function ($scope, $http, $timeout) {
    $scope.colors = [];
    $scope.getALlMau = function () {
        $http.get("https://localhost:7196/api/Color")
            .then(function (response) {
                $scope.colors = response.data.map(function (color) {
                    console.log(response.data);
                    console.log(color);

                    return {
                        id: color.id,
                        name: color.tenmau,
                        colorCode: color.mamau,
                        status: color.trangthai,
                        isUsedInProduct: color.isUsedInProduct
                    };
                });
            }, function (error) {
                console.error("Error fetching colors:", error);
            });
    }
    // Mẫu mới
    $scope.newColor = {
        name: '',
        colorCode: '',
        status: 0
    };
    $scope.editingColor = false;
    //mở modal thêm màu
    $scope.showAddColorModal = function () {
        $scope.newColor = { name: '', colorCode: '', status: 0 };
        $scope.editingColor = false;
        $scope.duplicateName = false;
        $scope.duplicateColorCode = false;
        $scope.isModalVisible = false;
    
        $timeout(function () {
            $scope.isModalVisible = true;
    
            // Dùng Bootstrap 5 để hiển thị modal
            const modalElement = document.getElementById('addColorModal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
    
            // Reset form sau khi modal đã hiển thị
            setTimeout(() => {
                $scope.$apply(function () {
                    if ($scope.colorForm) {
                        $scope.colorForm.$setPristine();
                        $scope.colorForm.$setUntouched();
                    }
                });
            }, 200); // Delay 200ms đảm bảo form đã khởi tạo
        });
    };
    
    //Thêm màu
    $scope.addColor = function () {
        if ($scope.editingColor) {
            $scope.updateColor();
        } else {
            // Gửi yêu cầu POST đến API để thêm màu mới
            $http.post("https://localhost:7196/api/Color", {
                mamau: $scope.newColor.colorCode,
                tenmau: $scope.newColor.name,
                trangthai: $scope.newColor.status
            }).then(function (response) {
                $scope.getALlMau();
                // Reset lại dữ liệu nhập màu
                $scope.newColor = { name: '', colorCode: '', status: 0 };
            }).catch(function (error) {
                console.error("Error adding color:", error);
                alert("Đã có lỗi xảy ra khi thêm màu.");
            });
        }
    };
    //Chỉnh sửa màu
    $scope.editColor = function (color) {
        $scope.newColor = angular.copy(color);
        $scope.editingColor = true;
        $scope.duplicateName = false;
        $scope.duplicateColorCode = false;
        $scope.isModalVisible = false;
    
        $timeout(function () {
            $scope.isModalVisible = true;
    
            const modalElement = document.getElementById('addColorModal');
            const modal = new bootstrap.Modal(modalElement);
            modal.show();
    
            setTimeout(() => {
                $scope.$apply(function () {
                    if ($scope.colorForm) {
                        $scope.colorForm.$setPristine();
                        $scope.colorForm.$setUntouched();
                    }
                });
            }, 200);
        });
    };
    
    $scope.duplicateName = false;
    $scope.duplicateColorCode = false;
    $scope.checkDuplicateColor = function () {
        $scope.duplicateName = false;
        $scope.duplicateColorCode = false;
    
        var currentId = $scope.newColor.id; // Nếu đang update, sẽ có ID
    
        $scope.colors.forEach(function (color) {
            // Bỏ qua chính màu đang chỉnh sửa
            if (currentId && color.id === currentId) return;
    
            // So sánh tên (không phân biệt hoa thường, bỏ khoảng trắng)
            if (color.name.trim().toLowerCase() === $scope.newColor.name.trim().toLowerCase()) {
                $scope.duplicateName = true;
            }
    
            // So sánh mã màu
            if (color.colorCode.trim().toLowerCase() === $scope.newColor.colorCode.trim().toLowerCase()) {
                $scope.duplicateColorCode = true;
            }
        });
    };
    
    //Cập nhật màu
    $scope.updateColor = function () {
        $http.put("https://localhost:7196/api/Color/" + $scope.newColor.id, {
            mamau: $scope.newColor.colorCode,
            tenmau: $scope.newColor.name,
            trangthai: $scope.newColor.status
        }).then(function (response) {
            // Reset lại dữ liệu nhập màu
            $scope.getALlMau();
            $scope.newColor = { name: '', colorCode: '', status: 0 };
            $scope.editingColor = false;
            // Đóng modal sau khi cập nhật
            var modal = bootstrap.Modal.getInstance(document.getElementById('addColorModal'));
            if (modal) {
                modal.hide();
            }
            alert("Cập nhật màu thành công!");
        }).catch(function (error) {
            console.error("Error updating color:", error);
            alert("Đã có lỗi xảy ra khi cập nhật màu.");
        });


        $scope.editingColor = false;
    };
    //Xóa màu
    $scope.deleteColor = function (color) {
        console.log(color);
        // Hiển thị hộp thoại xác nhận trước khi xóa
        if (!confirm("Bạn có chắc chắn muốn xóa màu này không?")) {
            return;
        }

        $http.delete("https://localhost:7196/api/Color/" + color.id)
            .then(function (response) {
                alert("Xóa màu thành công!");
                $scope.getALlMau();
            }).catch(function (error) {
                console.error("Error deleting color:", error);
                alert("Đã có lỗi xảy ra khi xóa màu.");
            });


    };
    $scope.getALlMau();

    $scope.chatLieus = [];
    $scope.newChatLieu = { tenchatlieu: '', trangthai: 0 };
    $scope.editingChatLieu = false;
    $scope.formError = {}; // Lưu lỗi validation

    //Lấy danh sách chất liệu
    $scope.getChatLieus = function () {
        $http.get("https://localhost:7196/api/ChatLieu").then(function (response) {
            $scope.chatLieus = response.data.map(function (cl) {
                return {
                    id: cl.id,
                    tenchatlieu: cl.tenchatlieu, 
                    trangthai: cl.trangthai,
                    isUsedInProduct: cl.isUsedInProduct
                };
            });
        }, function (error) {
            alert("Lỗi khi lấy danh sách chất liệu!");
            console.error(error);
        });
    };

    //Reset modal
    $scope.resetChatLieu = function () {
        $scope.newChatLieu = { tenchatlieu: '', trangthai: 0 };
        $scope.editingChatLieu = false;
        $scope.formError = {}; // <- RESET lỗi validate ở đây
    };    

    //Kiểm tra dữ liệu hợp lệ
    $scope.validateChatLieu = function () {
        let ten = ($scope.newChatLieu.tenchatlieu || "").trim(); // Kiểm tra null/undefined
        if (!ten) {
            $scope.formError.tenchatlieu = "Tên chất liệu không được để trống!";
        } else if (ten.length < 2) {
            $scope.formError.tenchatlieu = "Tên chất liệu phải có ít nhất 2 ký tự!";
        } else if (ten.length > 50) {
            $scope.formError.tenchatlieu = "Tên chất liệu không được vượt quá 50 ký tự!";
        }else if ($scope.chatLieus.some(cl => cl.tenchatlieu.toLowerCase() === ten.toLowerCase() && cl.id !== $scope.newChatLieu.id)) {
            $scope.formError.tenchatlieu = "Tên chất liệu đã tồn tại!";
        } else {
            $scope.formError.tenchatlieu = null;
        }
    };
    //Thêm chất liệu
    $scope.addChatLieu = function () {
        $scope.validateChatLieu();
        if ($scope.formError.tenchatlieu) return;

        $http.post("https://localhost:7196/api/ChatLieu", {
            tenchatlieu: $scope.newChatLieu.tenchatlieu,
            trangthai: $scope.newChatLieu.trangthai
        }).then(function () {
            $scope.getChatLieus(); // Lấy lại danh sách mới
            $scope.resetChatLieu();
        }, function (error) {
            alert("Lỗi khi thêm chất liệu!");
            console.error(error);
        });
    };
    //Sửa chất liệu
    $scope.editChatLieu = function (cl) {
        $scope.newChatLieu = angular.copy(cl);
        $scope.editingChatLieu = true;
        $scope.formError = {}; // Reset lỗi validate
        $scope.isModalVisible = false;
    };
    //Cập nhật chất liệu
    $scope.updateChatLieu = function () {
        $scope.validateChatLieu();
        if ($scope.formError.tenchatlieu) return;

        $http.put("https://localhost:7196/api/ChatLieu/" + $scope.newChatLieu.id, {
            tenchatlieu: $scope.newChatLieu.tenchatlieu,
            trangthai: $scope.newChatLieu.trangthai
        }).then(function () {
            $scope.getChatLieus(); // Lấy lại danh sách mới
            $scope.resetChatLieu();
        }, function (error) {
            alert("Lỗi khi cập nhật chất liệu!");
            console.error(error);
        });
    };
    //Xóa chất liệu
    $scope.deleteChatLieu = function (id) {
        if (confirm("Bạn có chắc muốn xóa chất liệu này?")) {
            $http.delete("https://localhost:7196/api/ChatLieu/" + id).then(function () {
                $scope.getChatLieus(); // Lấy lại danh sách mới
            }, function (error) {
                alert("Lỗi khi xóa chất liệu!");
                console.error(error);
            });
        }
    };
    $scope.getChatLieus();
    
    $scope.sizes = [];
    $scope.newSize = { sosize: '', trangthai: 0 };
    $scope.editingSize = false;
    $scope.formError1 = {};
    $scope.getSizes = function () {
        $http.get("https://localhost:7196/api/Size").then(function (response) {
            $scope.sizes = response.data.map(function (size) {
                return {
                    id: size.id,
                    sosize: size.sosize,
                    trangthai: size.trangthai,
                    isUsedInProduct: size.isUsedInProduct
                };
            });
        }, function (error) {
            console.error("Lỗi khi lấy danh sách size:", error);
        });
    };
    $scope.resetSize = function () {
        $scope.newSize = { sosize: '', trangthai: 0 };
        $scope.editingSize = false;
        $scope.formError1 = {};
    };
    $scope.validateSize = function () {
        let size = $scope.newSize.sosize;
        if (!size) {
            $scope.formError.sosize = "Số size không được để trống!";
        } else if (size < 30 || size > 50) {
            $scope.formError1.sosize = "Số size phải từ 30 đến 50!";
        } else if ($scope.sizes.some(s => s.sosize == size && s.id !== $scope.newSize.id)) {
            $scope.formError1.sosize = "Số size đã tồn tại!";
        } else {
            $scope.formError1.sosize = null;
        }
    };
    $scope.addSize = function () {
        $scope.validateSize();
        if ($scope.formError1.sosize) return;

        $http.post("https://localhost:7196/api/Size", {
            sosize: $scope.newSize.sosize,
            trangthai: $scope.newSize.trangthai
        }).then(function (response) {
            $scope.sizes.push(response.data);
            alert("Thêm size thành công!");
            $scope.getSizes(); // Lấy lại danh sách mới
            $scope.resetSize();
            // Đóng modal sau khi thêm
            var modal = bootstrap.Modal.getInstance(document.getElementById('sizeModal'));
            if (modal) {
                modal.hide();
            }
        }, function (error) {
            console.error("Lỗi khi thêm size:", error);
        });
    };
    $scope.editSize = function (size) {
        $scope.newSize = angular.copy(size);
        $scope.editingSize = true;
        console.log("Size sửa", size);
        $scope.formError1 = {}; // Reset lỗi validate
        
    };
    $scope.updateSize = function () {
        $scope.validateSize();
        if ($scope.formError1.sosize) return;

        $http.put("https://localhost:7196/api/Size/" + $scope.newSize.id, {
            sosize: $scope.newSize.sosize,
            trangthai: $scope.newSize.trangthai
        }).then(function () {
            alert("Cập nhật size thành công!");
            $scope.getSizes(); // Lấy lại danh sách mới
            $scope.editingSize = false;
            $scope.resetSize();
            // Đóng modal sau khi cập nhật
            var modal = bootstrap.Modal.getInstance(document.getElementById('sizeModal'));
            if (modal) {
                modal.hide();
            }

        }, function (error) {
            console.error("Lỗi khi cập nhật size:", error);
        });
    };
    $scope.deleteSize = function (id) {
        if (confirm("Bạn có chắc muốn xóa size này?")) {
            $http.delete("https://localhost:7196/api/Size/" + id).then(function () {
                alert("Xóa size thành công!");
                $scope.getSizes(); // Lấy lại danh sách mới
            }, function (error) {
                console.error("Lỗi khi xóa size:", error);
            });
        }
    };
    $scope.getSizes();
    $scope.thuongHieus = [];
    $scope.newThuongHieu = { tenthuonghieu: '', tinhtrang: 0 };
    $scope.editingThuongHieu = false;
    $scope.formError2 = {};
    
    // Lấy danh sách thương hiệu
    $scope.getThuongHieus = function () {
        $http.get("https://localhost:7196/api/Thuonghieu").then(function (response) {
            $scope.thuongHieus = response.data.map(function (thuonghieu) {
                return {
                    id: thuonghieu.id,
                    tenthuonghieu: thuonghieu.tenthuonghieu,
                    tinhtrang: thuonghieu.tinhtrang,
                    isUsedInProduct: thuonghieu.isUsedInProduct
                };
            });
        }, function (error) {
            console.error("Lỗi khi lấy danh sách Thương Hiệu:", error);
        });
    };
    
    // Reset dữ liệu khi mở modal
    $scope.resetThuongHieu = function () {
        $scope.newThuongHieu = { tenthuonghieu: '', tinhtrang: 0 };
        $scope.editingThuongHieu = false;
        $scope.formError2 = {};
    };
    
    // Validate tên thương hiệu
    $scope.validateThuongHieu = function () {
        let ten = $scope.newThuongHieu.tenthuonghieu.trim();
        if (!ten) {
            $scope.formError2.tenthuonghieu = "Tên thương hiệu không được để trống!";
        } else if (ten.length < 2) {
            $scope.formError2.tenthuonghieu = "Tên thương hiệu phải có ít nhất 2 ký tự!";
        } else if (ten.length > 50) {
            $scope.formError2.tenthuonghieu = "Tên thương hiệu không được vượt quá 50 ký tự!";
        } else if ($scope.thuongHieus.some(th => th.tenthuonghieu.toLowerCase() === ten.toLowerCase() && th.id !== $scope.newThuongHieu.id)) {
            $scope.formError2.tenthuonghieu = "Tên thương hiệu đã tồn tại!";
        } else {
            $scope.formError2.tenthuonghieu = null;
        }
    };
    
    // Thêm thương hiệu
    $scope.addThuongHieu = function () {
        $scope.validateThuongHieu();
        if ($scope.formError2.tenthuonghieu) return;
        $http.post("https://localhost:7196/api/Thuonghieu", {
            tenthuonghieu: $scope.newThuongHieu.tenthuonghieu,
            tinhtrang: $scope.newThuongHieu.tinhtrang
        }).then(function (response) {
            alert("Thêm thương hiệu thành công!");
            $scope.getThuongHieus()
            $scope.resetThuongHieu();
            //dong modal sau khi thêm
            var modal = bootstrap.Modal.getInstance(document.getElementById('addThuongHieuModal'));
            if (modal) {
                modal.hide();
            }
        }, function (error) {
            console.error("Lỗi khi thêm thương hiệu:", error);
        });
    };
    
    // Cập nhật thương hiệu
    $scope.updateThuongHieu = function () {
        $scope.validateThuongHieu();
        if ($scope.formError2.tenthuonghieu) return;
        $http.put("https://localhost:7196/api/Thuonghieu/" + $scope.newThuongHieu.id, {
            tenthuonghieu: $scope.newThuongHieu.tenthuonghieu,
            tinhtrang: $scope.newThuongHieu.tinhtrang
        }).then(function () {
            alert("Cập nhật thương hiệu thành công!");
            $scope.editingThuongHieu = false;
            //dong modal sau khi cập nhật
            var modal = bootstrap.Modal.getInstance(document.getElementById('addThuongHieuModal'));
            if (modal) {
                modal.hide();
            }
            $scope.getThuongHieus();
            $scope.resetThuongHieu();
        }, function (error) {
            console.error("Lỗi khi cập nhật thương hiệu:", error);
        });
    };
    
    // Xóa thương hiệu
    $scope.deleteThuongHieu = function (id) {
        if (confirm("Bạn có chắc muốn xóa thương hiệu này?")) {
            $http.delete("https://localhost:7196/api/Thuonghieu/" + id).then(function () {
                $scope.getThuongHieus();
                alert("Xóa thương hiệu thành công!");
            }, function (error) {
                console.error("Lỗi khi xóa thương hiệu:", error);
            });
        }
    };
    // Gán dữ liệu vào modal khi chỉnh sửa
    $scope.editThuongHieu = function (thuonghieu) {
        $scope.newThuongHieu = angular.copy(thuonghieu);
        $scope.editingThuongHieu = true;
    };
    // Gọi danh sách thương hiệu khi trang tải
    $scope.getThuongHieus();
    

});