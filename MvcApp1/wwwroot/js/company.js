$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'streetAddress', "width": "20%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "10%" },
            {
                data: 'id',
                "render": function (data, type, row) {
                    return `
                    <div  class="w-75 btn-group" role="group">
                      <a class="btn btn-primary mx-2" href="/admin/company/createOrupdate?id=${data}">Edit</a>

                      <a class="btn btn-danger mx-2" onClick=deleteCompany("/admin/company/delete?id=${data}")>Delete</a>

                    </div>
                ` },
                "width": "25%"
            }
        ]
    });
}


function deleteCompany(url) {
    Swal.fire({
        title: "Do you want to remove this company?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}