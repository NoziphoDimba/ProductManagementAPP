
//$(document).ready(function () {
//    editCategory();
//    deleteCategory();

//    $('#categoryForm').on('submit', function (event) {
//        event.preventDefault();

//        // Fetch createdBy value
//        var createdBy = $('#createdBy').val();

//        // Append createdBy value to formData
//        var formData = new FormData(this);
//        formData.append("CreatedBy", createdBy);

//        // Submit form via AJAX
//        $.ajax({
//            type: 'POST',
//            url: '/api/Categories',
//            data: formData,
//            contentType: false,
//            processData: false,
//            success: function () {
//                $('#successModal').modal('show');
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
//        var categoryId = $('#editCategoryId').val();

//        $.ajax({
//            type: 'PUT',
//            url: '/api/Categories/' + categoryId,
//            data: formData,
//            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
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
//            type: 'DELETE',
//            url: '/api/Categories/' + categoryId,
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
    loadCategories();
    setupEventHandlers();
});

function loadCategories() {
    $.ajax({
        type: 'GET',
        url: '/api/Categories',
        success: function (categories) {
            var tableBody = $('#categoryTableBody');
            tableBody.empty();
            categories.forEach(function (category) {
                var row = '<tr>' +
                    '<td>' + category.name + '</td>' +
                    '<td>' + category.categoryCode + '</td>' +
                    '<td>' + (category.isActive ? "Yes" : "No") + '</td>' +
                    '<td>' +
                    '<a href="#" class="btn btn-primary btn-sm" onclick="openEditModal(event, ' + category.categoryId + ')">Edit</a> ' +
                    '<a href="#" class="btn btn-danger btn-sm" onclick="openDeleteModal(event, ' + category.categoryId + ')">Delete</a>' +
                    '</td>' +
                    '</tr>';
                tableBody.append(row);
            });
        },
        error: function (error) {
            console.log('Error loading categories:', error);
        }
    });
}

function setupEventHandlers() {
    $('#categoryForm').on('submit', function (event) {
        event.preventDefault();
        var formData = new FormData(this);
        $.ajax({
            type: 'POST',
            url: '/api/Categories',
            data: JSON.stringify(Object.fromEntries(formData)),
            contentType: 'application/json',
            processData: false,
            success: function () {
                $('#successModal').modal('show');
                loadCategories();
            },
            error: function (error) {
                alert('Error adding category');
                console.log(error);
            }
        });
    });

    $('#editCategoryForm').on('submit', function (event) {
        event.preventDefault();
        var formData = $(this).serialize();
        var categoryId = $('#editCategoryId').val();
        $.ajax({
            type: 'PUT',
            url: '/api/Categories/' + categoryId,
            data: JSON.stringify(formData),
            contentType: 'application/json',
            processData: false,
            success: function () {
                $('#editModal').modal('hide');
                loadCategories();
            },
            error: function (error) {
                alert('Error editing category');
                console.log(error);
            }
        });
    });

    $('#deleteCategoryForm').on('submit', function (event) {
        event.preventDefault();
        var categoryId = $('#deleteCategoryId').val();
        $.ajax({
            type: 'DELETE',
            url: '/api/Categories/' + categoryId,
            success: function () {
                $('#deleteModal').modal('hide');
                loadCategories();
            },
            error: function (error) {
                alert('Error deleting category');
                console.log(error);
            }
        });
    });
}

function openEditModal(event, categoryId) {
    event.preventDefault();
    $.ajax({
        type: 'GET',
        url: '/api/Categories/' + categoryId,
        success: function (category) {
            $('#editCategoryId').val(category.categoryId);
            $('#editCategoryName').val(category.name);
            $('#editCategoryCode').val(category.categoryCode);
            $('#editIsActive').prop('checked', category.isActive);
            $('#editModal').modal('show');
        },
        error: function (error) {
            console.log('Error fetching category:', error);
        }
    });
}

function openDeleteModal(event, categoryId) {
    event.preventDefault();
    $('#deleteCategoryId').val(categoryId);
    $('#deleteModal').modal('show');
}

