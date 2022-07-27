$(".btn-edit-quiz").on("click", function () {
    //do some code in here
    $("#editQuiz").modal("show");
});
function deleteQuizById(id, btn) {
    $.confirm({
        title: 'Confirm!',
        content: 'Delete this quiz!',
        type: 'red',
        buttons: {
            delete: {
                btnClass: 'btn-red',
                // action: function () {
                //     $.ajax({
                //         url: `./permission?id=${id}`,
                //         type: 'DELETE',
                //         success: function (data, textStatus, jqXHR) {
                //             $(btn).closest('tr').fadeOut("slow");
                //         },
                //         error: function (jqXHR, textStatus, errorThrown) {
                //             let msg = (jqXHR.status === 403) ? "You do not have permission to delete this permission."
                //                 : "Delete permisson fail."
                //             $.alert({
                //                 type: "red",
                //                 title: "Error",
                //                 content: msg
                //             });
                //         }
                //     });
                // }
            },
            cancel: function () {
            }
        }
    });
}

$(document).ready( function () {
    $('#myTable').DataTable();
} );