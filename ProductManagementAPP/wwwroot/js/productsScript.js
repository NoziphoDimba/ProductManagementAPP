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
            success: function () {
                $('#successModal').modal('show');
                window.location.href = '/Categories/Categories';
            },
            error: function (error) {
                alert('Error adding product');
                console.log(error);
            }
        });
    });
});

function openEditProductModal(productId) {
    $.ajax({
        url: '/Products/GetProduct/' + productId,
        type: 'GET',
        success: function (product) {
            $('#editProductId').val(product.productId);
            $('#editProductName').val(product.productName);
            $('#editCategoryId').val(product.categoryId);
            $('#editExistingImage').val(product.image);
            $('#editImagePreview').attr('src', '/images/uploads/' + product.image).show();
            $('#editProductModal').modal('show');
        },
        error: function (error) {
            alert('Error fetching product details');
            console.log(error);
        }
    });
}

function submitEditProductForm() {
    var formData = new FormData($('#editProductForm')[0]);

    $.ajax({
        url: '/Products/Edit',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            $('#editProductModal').modal('hide');
            location.reload();
        },
        error: function (error) {
            alert('Error updating product');
            console.log(error);
        }
    });
}

var deleteProductId;

function openDeleteProductModal(productId) {
    deleteProductId = productId;
    $('#deleteProductModal').modal('show');
}

function deleteProduct() {
    $.ajax({
        url: '/Products/Delete/' + deleteProductId,
        type: 'POST',
        success: function () {
            $('#deleteProductModal').modal('hide');
            $('#successDeleteModal').modal('show');
        },
        error: function (error) {
            alert('Error deleting product');
            console.log(error);
        }
    });
}
