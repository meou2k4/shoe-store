app.controller('RankController', function ($scope, $http) {
    $scope.ranks = [];
    $scope.isLoading = true;
    $scope.errorMessage = '';
    $scope.statusFilter = 'all';
    
    // Form models
    $scope.newRank = {
        tenrank: '',
        minMoney: 0,
        maxMoney: 0,
        trangthai: 0  // Mặc định là hoạt động (0)
    };
    
    $scope.editRank = {
        id: null,
        tenrank: '',
        minMoney: 0,
        maxMoney: 0,
        trangthai: 0
    };
    
    $scope.deleteRankId = null;
    
    // Format tiền tệ VND
    $scope.formatCurrency = function(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    };
    
    // Lấy trạng thái text - ĐÃ CẬP NHẬT
    $scope.getStatusText = function(status) {
        switch(parseInt(status)) {
            case 0: return "Hoạt động";
            case 1: return "Không hoạt động";
            case 2: return "Đã xóa";
            default: return "Không xác định";
        }
    };
    
    // Lấy badge class cho rank
    $scope.getRankBadgeClass = function(rankName) {
        switch(rankName) {
            case "Bronze": return "badge-bronze";
            case "Silver": return "badge-silver";
            case "Gold": return "badge-gold";
            case "Platinum": return "badge-platinum";
            case "Diamond": return "badge-diamond";
            default: return "bg-secondary";
        }
    };
    
    // Lấy danh sách ranks từ API
    $scope.loadRanks = function() {
        $scope.isLoading = true;
        $http.get('https://localhost:7196/api/Ranks')
            .then(function(response) {
                $scope.ranks = response.data;
                $scope.isLoading = false;
                $scope.applyFilter();
            })
            .catch(function(error) {
                console.error('Error loading ranks:', error);
                $scope.errorMessage = 'Không thể tải dữ liệu từ server. Vui lòng thử lại!';
                $scope.isLoading = false;
            });
    };
    
    // Áp dụng bộ lọc trạng thái
    $scope.applyFilter = function() {
        if ($scope.statusFilter === 'all') {
            $scope.filteredRanks = [...$scope.ranks];
        } else {
            $scope.filteredRanks = $scope.ranks.filter(rank => 
                rank.trangthai == $scope.statusFilter
            );
        }
    };
    
    // Mở modal thêm mới
    $scope.openAddModal = function() {
        $scope.newRank = {
            tenrank: '',
            minMoney: 0,
            maxMoney: 0,
            trangthai: 0  // Mặc định là hoạt động (0)
        };
        $('#addRankModal').modal('show');
    };
    
    // Mở modal sửa
    $scope.openEditModal = function(rank) {
        $scope.editRank = {
            id: rank.id,
            tenrank: rank.tenrank,
            minMoney: rank.minMoney,
            maxMoney: rank.maxMoney,
            trangthai: rank.trangthai
        };
        $('#editRankModal').modal('show');
    };
    
    // Mở modal xóa
    $scope.openDeleteModal = function(rankId) {
        $scope.deleteRankId = rankId;
        $('#deleteRankModal').modal('show');
    };
    
    // Thêm rank mới
    $scope.addRank = function() {
        if (!$scope.newRank.tenrank || $scope.newRank.minMoney === null || $scope.newRank.maxMoney === null) {
            alert('Vui lòng điền đầy đủ thông tin bắt buộc');
            return;
        }
        
        $http.post('https://localhost:7196/api/Ranks', $scope.newRank)
            .then(function(response) {
                $scope.ranks.push(response.data);
                $scope.applyFilter();
                $('#addRankModal').modal('hide');
                
                // Reset form
                $scope.newRank = {
                    tenrank: '',
                    minMoney: 0,
                    maxMoney: 0,
                    trangthai: 0  // Mặc định là hoạt động (0)
                };
                
                // Hiển thị thông báo thành công
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Thêm rank mới thành công!'
                });
            })
            .catch(function(error) {
                console.error('Error adding rank:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã xảy ra lỗi khi thêm rank mới!'
                });
            });
    };
    
    // Cập nhật rank
    $scope.updateRank = function() {
        if (!$scope.editRank.tenrank || $scope.editRank.minMoney === null || $scope.editRank.maxMoney === null) {
            alert('Vui lòng điền đầy đủ thông tin bắt buộc');
            return;
        }
        
        $http.put('https://localhost:7196/api/Ranks/' + $scope.editRank.id, $scope.editRank)
            .then(function(response) {
                // Cập nhật rank trong mảng
                const index = $scope.ranks.findIndex(r => r.id === $scope.editRank.id);
                if (index !== -1) {
                    $scope.ranks[index] = response.data;
                }
                $scope.applyFilter();
                $('#editRankModal').modal('hide');
                
                // Hiển thị thông báo thành công
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Cập nhật rank thành công!'
                });
            })
            .catch(function(error) {
                console.error('Error updating rank:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã xảy ra lỗi khi cập nhật rank!'
                });
            });
    };
    
    // Xóa rank
    $scope.deleteRank = function() {
        $http.delete('https://localhost:7196/api/Ranks/' + $scope.deleteRankId)
            .then(function() {
                // Cập nhật trạng thái rank trong mảng
                const index = $scope.ranks.findIndex(r => r.id === $scope.deleteRankId);
                if (index !== -1) {
                    $scope.ranks[index].trangthai = 2; // Đánh dấu là đã xóa
                }
                $scope.applyFilter();
                $('#deleteRankModal').modal('hide');
                
                // Hiển thị thông báo thành công
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Xóa rank thành công!'
                });
            })
            .catch(function(error) {
                console.error('Error deleting rank:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã xảy ra lỗi khi xóa rank!'
                });
            });
    };
    
    // Khởi tạo controller
    $scope.init = function() {
        $scope.loadRanks();
    };
    
    // Gọi hàm khởi tạo
    $scope.init();
});