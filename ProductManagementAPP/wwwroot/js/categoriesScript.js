
$(document).ready(function () {
    $('#categoryForm').on('submit', function (event) {
        event.preventDefault();

        var createdBy = $('#createdBy').val();
        var formData = new FormData(this);
        formData.append("CreatedBy", createdBy);

        $.ajax({
            type: 'POST',
            url: '/Categories/Create',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                  
                    $('#successModal').modal('show');
                    window.location.href = '/Categories/Categories';
                } else {
                    alert(response.message);
                }
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

    $.ajax({
        type: 'POST',
        url: '/Categories/Edit',
        data: formData,
        success: function (response) {
            if (response.success) {
                $('#editCategoryModal').modal('hide');
                $('#successModal').modal('show');
            } else {
                alert('Error updating category: ' + response.message);
            }
        },
        error: function (error) {
            alert('Error updating category');
            console.log(error);
        }
    });
});
});

function openEditModal(event, categoryId) {
    debugger;
    event.preventDefault();

    $.ajax({
        type: 'GET',
        url: '/Categories/GetCategory/' + categoryId,
        success: function (data) {
            if (data) {
                $('#editCategoryId').val(data.categoryId);
                $('#editName').val(data.name);
                $('#editCategoryCode').val(data.categoryCode);

                if (data.isActive !== undefined) {
                    $('#editIsActive').val(data.isActive.toString());
                }

                $('#editCategoryModal').modal('show');
            } else {
                alert('Error loading category details');
            }
        },
        error: function (error) {
            alert('Error loading category details');
            console.log(error);
        }
    });
}

function openDeleteModal(event, categoryId) {
    event.preventDefault();
    $('#deleteModal').data('categoryId', categoryId).modal('show');
}

function deleteCategory() {
    var categoryId = $('#deleteModal').data('categoryId');

    $.ajax({
        type: 'POST',
        url: '/Categories/Delete/' + categoryId,
        success: function (response) {
            if (response.success) {
                $('#deleteModal').modal('hide');
                $('#successDeleteModal').modal('show');
            } else {
                alert('Error deleting category: ' + response.message);
            }
        },
        error: function (error) {
            alert('Error deleting category');
            console.log(error);
        }
    });
}
