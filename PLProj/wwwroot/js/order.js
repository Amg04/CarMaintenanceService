var dataTable;

$(document).ready(function () {

    // based on status
    var url = window.location.search;
    if (url.includes("Pending")) {
        loadDataTable("Pending")
    }
    else {
        if (url.includes("Proccessing")) {
            loadDataTable("Proccessing")
        }
        else {
            if (url.includes("Shipped")) {
                loadDataTable("Shipped")
            }
            else {
                if (url.includes("Approved")) {
                    loadDataTable("Approved")
                }
                else {
                    if (url.includes("Delivered")) {
                        loadDataTable("Delivered")
                    }
                    else {
                        if (url.includes("Cancelled")) {
                            loadDataTable("Cancelled")
                        }
                        else {
                            loadDataTable("all")
                        }
                    }
                }
            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/Order/GetAll?status=' + status },
        "columns": [
            { "data": 'id', "width": "5%" },
            { "data": 'name', "width": "15%" },
            { "data": 'phoneNumber', "width": "10%" },
            { "data": 'appUser.email', "width": "20%" },
            { "data": 'orderStatus', "width": "15%" },
            { "data": 'totalPrice', "width": "5%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                   <div class="w-75 btn-group" role="group">
                     <a href="/Order/Details?Id=${data}" class="btn btn-warning mx-2 d-flex align-items-center gap-2">
                         <i class="bi bi-pencil-square"></i>
                         Details
                     </a>
                  </div>

                `
                },
                "width": "15%"
            }
        ]
    });
}

