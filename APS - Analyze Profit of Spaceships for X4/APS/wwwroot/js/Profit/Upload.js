$(document).ready(function() {
    $("#tabstrip").kendoTabStrip({
        animation: { open: { effects: "fadeIn" } }
    });
    $("#refreshData").click(function(e) {
        kendo.ui.progress($("body"), true);
        $.ajax({
            url: $("#urlRefreshData").val(),
            datatype: "json",
            data: {
                "filePath": $("#fileServerSide").val()
            },
            complete: function () {
                Upload.OnComplete();
                kendo.ui.progress($("body"), false);
            },
            success: function() {
                notification("Completed", "File Processed", "success");
            },
            error: errorHandler
        });
    });
});

var Upload = {
    OnComplete: function() {
        $("#pivotgrid").data("kendoPivotGrid").dataSource.read();
        $("#Grid").data("kendoGrid").dataSource.read();
    },
    ClearData: function () {
        var url = $('#urlClearData').data('url');
        $.ajax({
            url: url,
            //data: { },
            type: "POST",
            //dataType: 'json',
            success: function (result) {
                notification("Save File", "Data Erased", "info");
                Upload.OnComplete();
                kendo.ui.progress($("body"), false);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                notification("Save File", xhr.status + " " + thrownError, "error");
            }
        });
    }
};
