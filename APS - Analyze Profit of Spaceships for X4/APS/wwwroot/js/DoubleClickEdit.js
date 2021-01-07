$(document).ready(function () {
    $("#Grid").on("dblclick", "tbody>tr", function (e) {
        var uid = $(this).data("uid");
        var model = $("#Grid").data("kendoGrid").dataSource.getByUid(uid);
        if (model.Actif) {
            var popup = $(".k-popup-edit-form");
            if (popup && !popup.is(':visible')) {
                $("#Grid").data('kendoGrid').editRow(this);
            }
        } else {
            notification("Enregistrement vérrouillé", "Modification Impossible", "warning");
        }
    });
});
