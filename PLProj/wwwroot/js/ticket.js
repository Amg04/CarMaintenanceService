var dataTable;

$(document).ready(function () {

    // based on status
    var url = window.location.search;
    if (url.includes("New")) {
        loadDataTable("New")
    }
    else {
        if (url.includes("Assigned")) {
            loadDataTable("Assigned")
        }
        else {
            if (url.includes("Finished")) {
                loadDataTable("Finished")
            }
            else {
                loadDataTable("all")
            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": '/Ticket/GetAll?status=' + status
        },
        "columns": [
            { "data": 'id', "width": "5%" },
            { "data": 'service.name', "width": "15%" },
            { "data": 'car.plateNumber', "width": "10%" },
            { "data": 'stateType', "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    let buttons = `
                             <div class="d-flex justify-content-center gap-2 flex-wrap" role="group">
                                 <a href="/Ticket/Details?id=${data}" class="btn btn-warning d-inline-flex align-items-center">
                                     <i class="fas fa-eye me-1"></i> Details
                                 </a>
                                `;

                    if (currentUserRole === "Admin") {
                        buttons += `
                                 <a href="/Ticket/AddAppointment?id=${data}" class="btn btn-success d-inline-flex align-items-center">
                                     <i class="fas fa-edit me-1"></i> Add Appointment
                                 </a>
                                `;
                    }

                    buttons += `</div>`;
                    return buttons;
                },
                "width": "60%"
            }
        ]
    });
}

