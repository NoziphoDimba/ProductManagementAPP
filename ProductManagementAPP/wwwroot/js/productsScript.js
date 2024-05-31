$(document).ready(function () {
    $('#productForm').on('submit', function (event) {
       
        event.preventDefault();

        var categoryName = $('#CategoryId option:selected').text();
        var categoryId = $('#CategoryId').val();

        $('#CategoryName').val(categoryName);
        $('#CategoryId').val(categoryId);

        var formData = new FormData(this);

        formData.append("CategoryName", categoryName);
        formData.append("CategoryId", categoryId);

        $.ajax({
            type: 'POST',
            url: '/Products/Create',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#successModal').modal('show');
                    window.location.href = '/Products/Products';
                } else {
                    alert('Error creating product: ' + response.message);
                }
            },
            error: function (error) {
                alert('Error creating product');
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
});

function openEditModal(event, productId) {
  
    event.preventDefault();
    $.ajax({
        type: 'GET',
        url: '/Products/GetProduct/' + productId,
        success: function (data) {
            if (data) {
                $('#editProductId').val(data.productId);
                $('#editProductName').val(data.name);
                $('#editImagePreview').attr('src', '/images/uploads/' + data.image).show();
                $('#editExistingImage').val(data.image);

                $.ajax({
                    type: 'GET',
                    url: '/Products/GetAllCategories',
                    success: function (categories) {
                        $('#editCategoryId').empty(); 
                        $.each(categories, function (index, category) {
                            $('#editCategoryId').append($('<option>', {
                                value: category.categoryId,
                                text: category.name,
                                selected: (category.categoryId === data.categoryId) 
                            }));
                        });
                    },
                    error: function (error) {
                        console.log('Error fetching categories:', error);
                    }
                });

                $('#editProductModal').modal('show');
            } else {
                alert('Error loading product details');
            }
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

function deleteProduct() {
    var productId = $('#deleteProductModal').data('productId');
    $.ajax({
        type: 'POST',
        url: '/Products/Delete',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                $('#deleteProductModal').modal('hide');
                location.reload(); 
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

