
//$(document).ready(function () {
//    debugger;
//    editCategory();
//    deleteCategory();
//    $('#categoryForm').on('submit', function (event) {
//        debugger;
//        event.preventDefault();

//        // Fetch createdBy value
//        var createdBy = $('#createdBy').val();

//        // Append createdBy value to formData
//        var formData = new FormData(this);
//        formData.append("CreatedBy", createdBy);

//        // Submit form via AJAX
//        $.ajax({
//            type: 'POST',
//            url: $(this).attr('action'),
//            data: formData,
//            contentType: false,
//            processData: false,
//            success: function () {
//                location.reload(); // Reload page on success
//            },
//            error: function (error) {
//                alert('Error adding category');
//                console.log(error);
//            }
//        });
//    });
//});





//// Function to edit an existing category
//function editCategory() {
//    $('#editCategoryForm').on('submit', function (event) {
//        event.preventDefault();
//        var formData = $(this).serialize();

//        $.ajax({
//            type: 'POST',
//            url: $(this).attr('action'),
//            data: formData,
//            success: function () {
//                location.reload();
//            },
//            error: function (error) {
//                alert('Error editing category');
//                console.log(error);
//            }
//        });
//    });
//}

//function openDeleteCategoryModal(event, categoryId) {
//    event.preventDefault();
//    $('#deleteCategoryId').val(categoryId);
//    $('#deleteCategoryModal').modal('show');
//}

//function deleteCategory() {
//    $('#deleteCategoryForm').on('submit', function (event) {
//        event.preventDefault();
//        var categoryId = $('#deleteCategoryId').val();

//        $.ajax({
//            type: 'POST',
//            url: '/Categories/DeleteCategory',
//            data: { id: categoryId },
//            success: function () {
//                location.reload();
//            },
//            error: function (error) {
//                alert('Error deleting category');
//                console.log(error);
//            }
//        });
//    });
//}
$(document).ready(function () {
    editCategory();
    deleteCategory();

    $('#categoryForm').on('submit', function (event) {
        event.preventDefault();

        // Fetch createdBy value
        var createdBy = $('#createdBy').val();

        // Append createdBy value to formData
        var formData = new FormData(this);
        formData.append("CreatedBy", createdBy);

        // Submit form via AJAX
        $.ajax({
            type: 'POST',
            url: '/api/Categories',
            data: formData,
            contentType: false,
            processData: false,
            success: function () {
                $('#successModal').modal('show');
            },
            error: function (error) {
                alert('Error adding category');
                console.log(error);
            }
        });
    });
});

// Function to edit an existing category
function editCategory() {
    $('#editCategoryForm').on('submit', function (event) {
        event.preventDefault();
        var formData = $(this).serialize();
        var categoryId = $('#editCategoryId').val();

        $.ajax({
            type: 'PUT',
            url: '/api/Categories/' + categoryId,
            data: formData,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function () {
                location.reload();
            },
            error: function (error) {
                alert('Error editing category');
                console.log(error);
            }
        });
    });
}

function openDeleteCategoryModal(event, categoryId) {
    event.preventDefault();
    $('#deleteCategoryId').val(categoryId);
    $('#deleteCategoryModal').modal('show');
}

function deleteCategory() {
    $('#deleteCategoryForm').on('submit', function (event) {
        event.preventDefault();
        var categoryId = $('#deleteCategoryId').val();

        $.ajax({
            type: 'DELETE',
            url: '/api/Categories/' + categoryId,
            success: function () {
                location.reload();
            },
            error: function (error) {
                alert('Error deleting category');
                console.log(error);
            }
        });
    });
}
