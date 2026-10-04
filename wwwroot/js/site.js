// TechStore - JavaScript global

$(function () {

    $(document).on("submit", "form.swal-delete-form", function (e) {

        e.preventDefault();

        const form = this;

        Swal.fire({
            title: "¿Estás seguro?",
            text: "El elemento será desactivado.",
            icon: "warning",

            showCancelButton: true,

            confirmButtonColor: "#dc3545",
            cancelButtonColor: "#6c757d",

            confirmButtonText: "Sí, desactivar",
            cancelButtonText: "Cancelar",

            reverseButtons: true
        }).then(function (result) {

            if (result.isConfirmed) {
                form.submit();
            }

        });

    });

});