var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/color/getall'
        },
        "columns": [
            { "data": 'id', "width": "25%" },
            { "data": 'name', "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="w-75 btn-group" role="group">
                    	  <a href="/color/Edit?id=${data}" class="btn btn-success mx-2 d-inline-flex align-items-center">
                                    <i class="fas fa-edit"></i> Edit</a>

                        <a onClick=Delete('/color/delete?id=${data}') class="btn btn-danger mx-2 d-inline-flex align-items-center">
                                  <i class="fas fa-trash"></i> Delete</a>
                    </div>
                    
                `
                },
                "width": "50%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr) {
                    toastr.error("Something went wrong.");
                }
            })
        }
    });
}
