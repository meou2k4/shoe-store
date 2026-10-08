app.controller('timkiemController', function ($scope, $routeParams, $http, $timeout) {
    // Khởi tạo biến
    $scope.searchKey = decodeURIComponent($routeParams.search || '');
    $scope.filteredResults = [];
    $scope.displayResults = [];
    $scope.isLoading = true;
    $scope.errorMessage = '';
    $scope.currentPage = 1;
    $scope.pageSize = 9;
    $scope.totalPages = 0;
    $scope.sortOption = 'newest';
    $scope.filters = {
        priceRange: null,
        minPrice: null,
        maxPrice: null,
        brands: {},
        sizes: {}
    };

    // Hàm tải ảnh sản phẩm
    function loadHinhAnh(idSPCT, callback) {
        fetch(`https://localhost:7196/api/Sanphamchitiets/GetImageById/${idSPCT}`)
            .then(response => response.ok ? response.blob() : Promise.reject())
            .then(blob => callback(URL.createObjectURL(blob)))
            .catch(() => callback('assets/images/default-product.jpg'));
    }

    // Gọi API lấy thương hiệu và size
    $http.get('https://localhost:7196/api/Thuonghieu').then(res => {
        $scope.dataThuonghieu = res.data;
    });
    $http.get('https://localhost:7196/api/Size').then(res => {
        $scope.availableSizes = res.data;
    });

    // Hàm xử lý dữ liệu sản phẩm
    function processSanphamList(list) {
        return list
            .filter(sp => sp.sanphamchitiets?.length > 0)
            .map(sp => {
                sp.hinhAnh = "assets/images/default-product.jpg";
                sp.soLuongBan = sp.sanphamchitiets.reduce((total, spct) => total + (spct.soLuongBan || 0), 0);

                let spct = sp.sanphamchitiets[0];
                if (spct?.id) {
                    loadHinhAnh(spct.id, imgUrl => {
                        sp.hinhAnh = imgUrl;
                        $timeout(() => $scope.$apply(), 0);
                    });
                }

                return sp;
            });
    }

    // Gọi API tìm kiếm chính
    $http.get(`https://localhost:7196/api/Sanphams/search?name=${encodeURIComponent($scope.searchKey)}`)
        .then(res => {
            if (res.data?.length) {
                $scope.filteredResults = processSanphamList(res.data);
                $scope.displayResults = [...$scope.filteredResults];
            }
            $scope.totalPages = Math.ceil($scope.displayResults.length / $scope.pageSize);
            $scope.isLoading = false;
        })
        .catch(() => {
            $scope.errorMessage = 'Không thể tải dữ liệu từ server. Vui lòng thử lại!';
            $scope.isLoading = false;
        });

    // Tìm kiếm dự phòng
    $timeout(() => {
        if ($scope.displayResults.length === 0 && !$scope.isLoading) {
            $http.get(`https://localhost:7196/api/Sanphams/GetALLSanPham`)
                .then(res => {
                    const keyword = $scope.searchKey.toLowerCase();
                    const matched = res.data.filter(p => p.tensp?.toLowerCase().includes(keyword));
                    $scope.filteredResults = processSanphamList(matched);
                    $scope.displayResults = [...$scope.filteredResults];
                    $scope.totalPages = Math.ceil($scope.displayResults.length / $scope.pageSize);
                });
        }
    }, 1000);

    // Sinh sao đánh giá
    $scope.generateStars = function (rating) {
        rating = parseFloat(rating) || 0;
        const full = Math.floor(rating);
        const half = rating % 1 >= 0.5;
        const empty = 5 - Math.ceil(rating);
        return [
            ...Array(full).fill('full'),
            ...(half ? ['half'] : []),
            ...Array(empty).fill('empty')
        ];
    };

    // Tổng sản phẩm đã bán
    $scope.getTotalSPCT = product =>
        product?.sanphamchitiets?.reduce((total, spct) => total + (spct.soLuongBan || 0), 0) || 0;

    // Áp dụng bộ lọc
    $scope.applyAllFilters = function () {
        const { priceRange, brands, sizes } = $scope.filters;

        if (priceRange) {
            const ranges = {
                'under1m': [0, 1000000],
                '1m-2m': [1000000, 2000000],
                '2m-3m': [2000000, 3000000],
                '3m-4m': [3000000, 4000000],
                'over4m': [4000000, null]
            };
            [$scope.filters.minPrice, $scope.filters.maxPrice] = ranges[priceRange];
        }

        const selectedBrands = Object.keys(brands).filter(k => brands[k]).map(Number);
        const selectedSizes = Object.keys(sizes).filter(k => sizes[k]);
        console.log('selectedSizes', selectedSizes);
        $scope.displayResults = $scope.filteredResults.filter(item => {
            const price = item.giasale || item.giaban;
            const matchesPrice = (!price || !$scope.filters.minPrice && !$scope.filters.maxPrice) ||
                (price >= ($scope.filters.minPrice || 0) && price <= ($scope.filters.maxPrice ?? Infinity));

            const brandId = item.idth || item.idThuongHieu || item.idthuonghieu;
            const matchesBrand = selectedBrands.length === 0 || selectedBrands.includes(+brandId);
           
            const converted = selectedSizes.map(s => parseInt(s) - 30);
            const matchesSize = selectedSizes.length === 0 ||
                item.sanphamchitiets?.some(spct => converted.includes(spct.idSize));
                console.log('convert',converted);
            return matchesPrice && matchesBrand && matchesSize;
        });

        $scope.currentPage = 1;
        $scope.totalPages = Math.ceil($scope.displayResults.length / $scope.pageSize);
    };

    // Xóa bộ lọc
    $scope.resetFilters = function () {
        $scope.filters = { priceRange: null, minPrice: null, maxPrice: null, brands: {}, sizes: {} };
        $scope.displayResults = [...$scope.filteredResults];
        $scope.currentPage = 1;
        $scope.totalPages = Math.ceil($scope.displayResults.length / $scope.pageSize);
    };

    // Phân trang
    $scope.getPagedProducts = () => {
        const start = ($scope.currentPage - 1) * $scope.pageSize;
        return $scope.displayResults.slice(start, start + $scope.pageSize);
    };
    $scope.previousPage = () => { if ($scope.currentPage > 1) $scope.currentPage--; };
    $scope.nextPage = () => { if ($scope.currentPage < $scope.totalPages) $scope.currentPage++; };
    $scope.goToPage = page => { if (page >= 1 && page <= $scope.totalPages) $scope.currentPage = page; };

    // Sắp xếp
    $scope.sortProducts = function () {
        const getPrice = p => p.giasale || p.giaban;
        switch ($scope.sortOption) {
            case 'newest':
                $scope.displayResults.sort((a, b) => new Date(b.ngayThemSanPham) - new Date(a.ngayThemSanPham));
                break;
            case 'oldest':
                $scope.displayResults.sort((a, b) => new Date(a.ngayThemSanPham) - new Date(b.ngayThemSanPham));
                break;
            case 'lowToHigh':
                $scope.displayResults.sort((a, b) => getPrice(a) - getPrice(b));
                break;
            case 'highToLow':
                $scope.displayResults.sort((a, b) => getPrice(b) - getPrice(a));
                break;
            case 'bestseller':
                $scope.displayResults.sort((a, b) => $scope.getTotalSPCT(b) - $scope.getTotalSPCT(a));
                break;
        }
        $scope.currentPage = 1;
    };
});
