var dataTable;

$(document).ready(function () {
    const roles = ["Customer", "Admin", "Technician", "Driver"];
    const url = window.location.search;
    const role = roles.find(r => url.includes(r)) || "all";
    loadDataTable(role);
});


function loadDataTable(role) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/user/GetAll?role=' + role
        },
        "columns": [
            { "data": 'name', "width": "15%" },
            { "data": 'name', "width": "15%" },
            { "data": 'email', "width": "15%" },
            { "data": 'role', "width": "15%" },
            {
                "data": null,
                "render": function (data) {
                    let lockBtn = "";

                    if (data.isLocked) {
                        lockBtn = `<a onClick=LockUnlock('${data.id}') 
              class="btn btn-danger mx-2 d-inline-flex align-items-center">
              <i class="fas fa-lock"></i>
           </a>`;
                    } else {
                        lockBtn = `<a onClick=LockUnlock('${data.id}') 
              class="btn btn-success mx-2 d-inline-flex align-items-center">
              <i class="fas fa-lock-open"></i>
           </a>`;
                    }

                    let roleForLinks = (data.role === "Admin") ? "Customer" : data.role;

                    let detailsUrl = `/user/${roleForLinks}Details?id=${data.id}`;
                    let editUrl = `/user/${roleForLinks}Edit?id=${data.id}`;

                    return `
        <div class="w-100 btn-group" role="group">
            <a href="${detailsUrl}" class="btn btn-warning mx-2 d-inline-flex align-items-center">
                <i class="fas fa-eye me-1"></i> Details
            </a>
            <a href="${editUrl}" class="btn btn-success mx-2 d-inline-flex align-items-center">
                <i class="fas fa-edit"></i> Edit
            </a>
            ${lockBtn}
            <a onClick=Delete('/user/delete?id=${data.id}') class="btn btn-danger mx-2 d-inline-flex align-items-center">
                <i class="fas fa-trash"></i> Delete
            </a>
        </div>
    `;
                }
,
                "width": "40%"
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

function LockUnlock(id) {
    $.ajax({
        url: '/user/LockUnlock',
        type: 'POST',
        data: { id: id },
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        },
        error: function () {
            toastr.error("Something went wrong.");
        }
    });
}
