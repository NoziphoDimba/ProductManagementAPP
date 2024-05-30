
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
                    $('#successModal .modal-body').text(response.message);
                    $('#successModal').modal('show');
                    $('#successModal').on('hidden.bs.modal', function () {
                        window.location.href = '/Categories/Categories';
                    });
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
});

