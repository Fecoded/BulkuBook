let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll",
            "type": "GET"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10" },
            { "data": "postalCode", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked />`;
                    }
                    else {
                        return `<input type="checkbox" disabled />`;
                    }
                },
                "width": "10%"
            },
            
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Company/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a onclick=onDelete("/Admin/Company/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                   `;
                }, "width": "25%"
            }
        ],
        "language": {
            "emptyTable": "No data available in table"
        },
        "width": "100%"
    });
}

function onDelete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You won't be able to retrieve the data",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }

                }
            });
        }
    })
} 