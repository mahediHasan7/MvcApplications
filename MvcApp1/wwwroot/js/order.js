$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phone', "width": "20%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: null,
                "render": function (data, type, row) {
                    return `
                    <div  class="w-75 btn-group" role="group">
                      <a class="btn btn-primary mx-2" href="/admin/order/details?orderId=${data.id}"><i class="bi bi-pencil-square"></i></a>
                    </div>
                ` },
                "width": "10%"
            }
        ]
    });
}