// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {
    $(document).on('submit', 'form.swal-delete-form', function (e) {
        e.preventDefault();
        var form = this;
        Swal.fire({
            title: '¿Desea eliminar esto?',
            text: 'Esta acción no se puede deshacer',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí',
            cancelButtonText: 'No'
        })
            .then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
            })
    });
});


