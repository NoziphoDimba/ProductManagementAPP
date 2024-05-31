$(document).ready(function () {
    $('#productForm').on('submit', function (event) {
        event.preventDefault();

        var formData = new FormData(this);

        var categoryId = $('#CategoryId').val();
        var categoryName = $('#CategoryId option:selected').text();

        formData.append('CategoryId', categoryId);
        formData.append('CategoryName', categoryName);

        var imageFile = $('input[name="image"]')[0].files[0];
        if (imageFile) {
            formData.append('image', imageFile);
        }

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: formData,
            contentType: false,
            processData: false,
           
            success: function (response) {
                if (response.success) {

                    $('#successModal').modal('show');
                    window.location.href = '/Products/Products';
                } else {
                    alert(response.message);
                }
            },
            error: function (error) {
                alert('Error adding product');
                console.log(error);
            }
        });
    });

    $('#uploadExcelForm').on('submit', function (event) {
        event.preventDefault();
        var formData = new FormData(this);

        $.ajax({
            type: 'POST',
            url: '/Products/UploadExcel',
            data: formData,
            contentType: false,
            processData: false,
            success: function () {
                $('#uploadExcelModal').modal('hide');
                $('#successModal').modal('show');
            },
            error: function (error) {
                alert('Error uploading Excel file');
                console.log(error);
            }
        });
    });

    $('#editProductForm').on('submit', function (event) {
        event.preventDefault();
        var formData = $(this).serialize();

        $.ajax({
            type: 'POST',
            url: '/Products/Edit',
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#editProductModal').modal('hide');
                    $('#successModal').modal('show');
                } else {
                    alert('Error updating product: ' + response.message);
                }
            },
            error: function (error) {
                alert('Error updating product');
                console.log(error);
            }
        });
    });

    $('.btn-edit-product').on('click', function (event) {
    var productId = $(this).data('product-id');
    openEditModal(event, productId);
});

   $('.btn-delete-product').on('click', function (event) {
    var productId = $(this).data('product-id');
    openDeleteModal(event, productId);
});

});

function openUploadModal(event, productId) {
    event.preventDefault();
    $('#productId').val(productId);
    $('#uploadExcelModal').modal('show');
}

function deleteProduct() {
    var productId = $('#deleteProductModal').data('productId');
    $.ajax({
        type: 'POST',
        url: '/Products/Delete/' + productId,
        success: function (response) {
            if (response.success) {
                $('#deleteProductModal').modal('hide');
                $('#successDeleteModal').modal('show');
            } else {
                alert('Error deleting product: ' + response.message);
            }
        },
        error: function (error) {
            alert('Error deleting product');
            console.log(error);
        }
    });
}

function openEditModal(event, productId) {
    event.preventDefault();
    $.ajax({
        type: 'GET',
        url: '/Products/GetProduct/' + productId,
        success: function (data) {
            $('#editProductId').val(data.ProductId);
            $('#editProductName').val(data.Name);
            $('#editCategoryId').val(data.CategoryId);
            // Populate category dropdown dynamically here if needed
            $('#editProductModal').modal('show');
        },
        error: function (error) {
            alert('Error loading product details');
            console.log(error);
        }
    });
}

function openDeleteModal(event, productId) {
    event.preventDefault();
    $('#deleteProductModal').data('productId', productId).modal('show');
}
