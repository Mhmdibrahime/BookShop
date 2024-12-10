var dtble;
$(document).ready(function () {

    loadData()
});
function loadData() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Publisher/GetAllPublishers"
        },
        "columns": [
            { "data": "name" },
            { "data": "address" },
            { "data": "phone" },
            { "data": "email" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                           <a href="/Admin/Publisher/Edit/${data}" class="btn btn-info btn-sm" >
                                 <i class="fa-solid fa-pen-to-square"></i> 
                            </a> |

                            <a  href="/Admin/Publisher/Details/${data}" class="btn btn-success btn-sm" >
                                <i class="fas fa-eye"></i>
                            </a> |

                            <a class="btn btn-danger btn-sm" onClick=DeleteItem("/Admin/Publisher/Delete/${data}")>
                                <i class="fas fa-trash"></i>
                            </a>
                           `
                }
            }
        ]

    });
}



function DeleteItem(url) {

    Swal.fire({
        title: "Are you sure?",
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
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        toastr["success"](data.message)

                    } else {
                        toastr["error"](data.message)
                    }
                }

            });
        }
    });
}


function Lock(url, button) {
    console.log("URL: ", url);

    Swal.fire({
        title: "Do you want to continue?",
        icon: "question",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        showCancelButton: true,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "POST",
                success: function (data) {
                    console.log("Response Data: ", data);
                    if (data.success) {
                        dtble.ajax.reload();

                        if (data.isLocked) {

                            $('#').find('i').removeClass('fa-lock-open').addClass('fa-lock');
                            $(button).removeClass('btn-success').addClass('btn-danger');
                            toastr["success"](data.message);
                        } else {
                            $(button).find('i').removeClass('fa-lock').addClass('fa-lock-open');
                            $(button).removeClass('btn-danger').addClass('btn-success');
                            toastr["success"](data.message);
                        }
                    } else {
                        toastr["error"](data.message);
                    }
                },
                error: function () {
                    toastr["error"]("An error occurred while processing the request.");
                }
            });
        }
    });
}

